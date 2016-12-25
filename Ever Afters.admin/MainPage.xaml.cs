using Ever_Afters.common.Listeners;
using Ever_Afters.common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Ever_Afters.common.DatabaseLayer;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ever_Afters.admin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DataRequestHandler _db;

        public DataRequestHandler Db => _db ?? (_db = new SQLiteService());


        public MainPage()
        {
            this.InitializeComponent();
        }

        #region GridNavigation
        private void ToTagGrid(object sender, PointerRoutedEventArgs e)
        {
            ShowGrids(0);
        }

        private void ToVideoGrid(object sender, PointerRoutedEventArgs e)
        {
            ShowGrids(1);
        }

        private void ToBindGrid(object sender, PointerRoutedEventArgs e)
        {
            PopulateBindTags();
            PopulateBindVideo();
            ShowGrids(2);
        }

        private void ToRemoveGrid(object sender, PointerRoutedEventArgs e)
        {
            PopulateRemoveTags();
            PopulateRemoveVideos();
            ShowGrids(3);
        }

        private void ShowGrids(int gridNr)
        {
            //Set Everything Hidden
            beginGrid.Visibility = Visibility.Collapsed;
            upTagGrid.Visibility = Visibility.Collapsed;
            upVideoGrid.Visibility = Visibility.Collapsed;
            bindTagGrid.Visibility = Visibility.Collapsed;
            removeGrid.Visibility = Visibility.Collapsed;

            //Show the requested Grid
            switch (gridNr)
            {
                case 0:
                    upTagGrid.Visibility = Visibility.Visible;
                    break;
                case 1:
                    upVideoGrid.Visibility = Visibility.Visible;
                    break;
                case 2:
                    bindTagGrid.Visibility = Visibility.Visible;
                    break;
                case 3:
                    removeGrid.Visibility = Visibility.Visible;
                    break;
            }
        }
        #endregion

        #region upTagGrid
        private void SubmitUpTag(object sender, RoutedEventArgs e)
        {
            //Check if tag is valid
            upTagSubmit.IsEnabled = false;
            String returntext = "Unknown Error";
            String tagId = (String)upTagName.Text;
            if (!String.IsNullOrEmpty(tagId))
            {
                bool exists = common.Models.Tag.tagExists(tagId);
                if (!exists)
                {
                    //Save valid tag
                    Db.SaveTag(tagId);
                    returntext = "Tag Saved Successfully";
                } else
                {
                    returntext = "The tag already exists!";
                }
            } else returntext = "The tagstring cannot be empty!";

            upTagTitle.Text = returntext;
            upTagName.Text = "";

            //Reset the display
            ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                ChangeText(upTagTitle, "Scan Tag");
                ReEnable(upTagSubmit);
            }, TimeSpan.FromSeconds(3));
        }
        #endregion

        #region upVideoGrid

        StorageFile baseVideoPath = null;
        StorageFile offscreenVideoPath = null;
        StorageFile onscreenVideoPath = null;

        private async void UpVideoSubmit(object sender, RoutedEventArgs e)
        {
            upVideoSubmit.IsEnabled = false;
            String returntext = "Unknown Error";

            //Check if the radiobutton choice is made
            if (rbtOnScreen.IsChecked == null || rbtOffScreen.IsChecked == null) return;
            if (rbtOffScreen.IsChecked.Value == false && rbtOnScreen.IsChecked.Value == false)
            {
                returntext = "Select the beginning state radiobutton!";
            }
            else
            {
                //Check if all the videos were uploaded
                if (baseVideoPath != null && offscreenVideoPath != null && onscreenVideoPath != null)
                {
                    try
                    {
                        //Make sure that the resources directory exists
                        String path = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Resources");
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                        //Check if one of the videos doesn't exist yet
                        bool baseExists = File.Exists(Path.Combine(path, baseVideoPath.Name));
                        bool onExists = File.Exists(Path.Combine(path, onscreenVideoPath.Name));
                        bool offExists = File.Exists(Path.Combine(path, offscreenVideoPath.Name));

                        if (baseExists == false && onExists == false && offExists == false)
                        {
                            //Check if the videos are unique!
                            if (baseVideoPath.Name == onscreenVideoPath.Name || baseVideoPath.Name == offscreenVideoPath.Name || onscreenVideoPath.Name == offscreenVideoPath.Name)
                            {
                                returntext = "Videos cannot be the same!";
                            }
                            else
                            {
                                //Copy the videos to the resource directory
                                StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);
                                await baseVideoPath.CopyAsync(folder);
                                await offscreenVideoPath.CopyAsync(folder);
                                await onscreenVideoPath.CopyAsync(folder);

                                //Get the information about the beginning of the video
                                bool startsOnScreen = rbtOnScreen.IsChecked.Value;

                                //Save the video to the database
                                Db.SaveVideo(startsOnScreen, baseVideoPath.Name, onscreenVideoPath.Name,
                                    offscreenVideoPath.Name);

                                //Clear the input fields
                                rbtOnScreen.IsChecked = false;
                                rbtOffScreen.IsChecked = false;
                                baseVideoPath = null;
                                onscreenVideoPath = null;
                                offscreenVideoPath = null;
                                BaseVideoPath.Text = "No Basevideo Selected";
                                OnScreenEndingPath.Text = "No Onscreen Ending Selected";
                                OffScreenEndingPath.Text = "No Offscreen Ending Selected";

                                //Feedback to user
                                returntext = "Uploading Succeeded!";
                            }
                        }
                        else
                        {
                            if (baseExists) returntext = "The basevideo already exists";
                            if (onExists) returntext = "The onscreen ending already exists";
                            if (offExists) returntext = "The offscreen ending already exists";
                            if (baseExists && onExists && offExists) returntext = "all already exist!";
                        }
                    }
                    catch (Exception ex)
                    {
                        returntext = "Whoops. Error on copying files.";
                    }
                }
            }

            upVideoTitle.Text = returntext;

            //Reset the display
            ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                ChangeText(upVideoTitle, "Upload Video");
                ReEnable(upVideoSubmit);
            }, TimeSpan.FromSeconds(4));
        }

        private async void OpenDialogBaseVideo(object sender, RoutedEventArgs e)
        {
            //Show the dialog for selecting the base video
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary
            };
            picker.FileTypeFilter.Add(".mp4");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                this.BaseVideoPath.Text = file.Name;
                baseVideoPath = file;
            } else
            {
                this.BaseVideoPath.Text = "No BaseVideo Selected.";
            }
        }

        private async void OpenDialogOnscreenEnding(object sender, RoutedEventArgs e)
        {
            //Show the dialog for selecting an onscreen ending
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary
            };
            picker.FileTypeFilter.Add(".mp4");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                this.OnScreenEndingPath.Text = file.Name;
                onscreenVideoPath = file;
            } else
            {
                this.OnScreenEndingPath.Text = "No OnscreenVideo Selected.";
            }
        }

        private async void OpenDialogOffscreenEnding(object sender, RoutedEventArgs e)
        {
            //Show the dialog for selecting an offscreen ending
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary
            };
            picker.FileTypeFilter.Add(".mp4");

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                this.OffScreenEndingPath.Text = file.Name;
                offscreenVideoPath = file;
            } else
            {
                this.OffScreenEndingPath.Text = "No Offscreen Selected.";
            }
        }

        #endregion

        #region upBindingGrid

        private void PopulateBindTags()
        {
            //Clear the list
            cboTags.ItemsSource = null;

            //Retrieve the unbound tags
            IEnumerable<Tag> tags = Db.GetUnboundTags();

            //Set the list
            cboTags.ItemsSource = tags;
        }

        private void PopulateBindVideo()
        {
            //Clear the list
            cboVideo.ItemsSource = null;

            //Retrieve the videos
            IEnumerable<Video> videos = Db.GetAllVideos();

            //Set the list
            cboVideo.ItemsSource = videos;
        }

        private void BindTagSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            String returntext = "Unknown Error";
            bindTagSubmit.IsEnabled = false;

            //Check if a selection has been made
            Tag selectedTag = (Tag) cboTags.SelectedItem;
            Video selectedVideo = (Video) cboVideo.SelectedItem;

            if (selectedTag == null)
            {
                returntext = "Select a tag";
            } else if (selectedVideo == null)
            {
                returntext = "Select a video";
            } else
            {
                //Check if the tag is still unbound
                if (common.Models.Tag.isBound(selectedTag.name))
                {
                    returntext = "The selected tag is already bound";
                }
                else
                {
                    //Bind the tag and the video
                    Db.BindVideoToTag(selectedVideo, selectedTag);

                    //Reset the display
                    PopulateBindTags();
                    PopulateBindVideo();
                    returntext = "Binding successful";
                }
            }

            bindTagTitle.Text = returntext;

            //Reset the display
            ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                ChangeText(bindTagTitle, "Bind Tag To Video");
                ReEnable(bindTagSubmit);
            }, TimeSpan.FromSeconds(3));
        }

        #endregion

        #region removeGrid

        private void PopulateRemoveTags()
        {
            //Clear the list
            cboAllTags.ItemsSource = null;

            //Retrieve the unbound tags
            IEnumerable<Tag> tags = Db.GetAllTags();

            //Set the list
            cboAllTags.ItemsSource = tags;
        }

        private void PopulateRemoveVideos()
        {
            //Clear the list
            cboAllVideo.ItemsSource = null;

            //Retrieve the videos
            IEnumerable<Video> videos = Db.GetAllVideos();

            //Set the list
            cboAllVideo.ItemsSource = videos;
        }

        private void RemoveTagSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            String returntext = "Unknown Error";
            removeTagSubmit.IsEnabled = false;
            removeVideoSubmit.IsEnabled = false;
            Tag selectedTag = (Tag) cboAllTags.SelectedItem;

            //Check if the user selected a tag
            if (selectedTag == null)
            {
                returntext = "Select a Tag";
            }
            else
            {
                //Check if the tag still exists
                if (common.Models.Tag.tagExists(selectedTag.name))
                {
                    //Check if the tag is bound. -> If yes, unbound
                    if (common.Models.Tag.isBound(selectedTag.name))
                    {
                        Db.DeleteBinding(selectedTag);
                    }

                    //Delete the tag
                    Db.DeleteTag(selectedTag.id);

                    //Feedback to the user
                    returntext = "Tag Successfully Removed";
                }
                else
                {
                    returntext = "The tag doesn't exist";
                }
            }

            PopulateRemoveTags();
            PopulateRemoveVideos();
            removeTitle.Text = returntext;

            //Reset the display
            ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                ChangeText(bindTagTitle, "Remove Tags/Videos");
                ReEnable(removeTagSubmit);
                ReEnable(removeVideoSubmit);
            }, TimeSpan.FromSeconds(3));
        }

        private void RemoveVideoSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            String returntext = "Unknown Error";
            removeTagSubmit.IsEnabled = false;
            removeVideoSubmit.IsEnabled = false;
            Video selectedVideo = (Video) cboAllVideo.SelectedItem;

            //Check if the user selected a video
            if (selectedVideo == null)
            {
                returntext = "Select a Video";
            }
            else
            {
                //Check if the video still exists
                if (Video.pathExists(selectedVideo.BasePath))
                {
                    //Check if the video still has any bindings -> If Yes, unbind
                    IEnumerable<Tag> allTags = Db.GetAllTags();
                    foreach (Tag tag in allTags)
                    {
                        if (selectedVideo.id == Db.LoadVideoFromTag(tag).id)
                        {
                            Db.DeleteBinding(tag);
                        }
                    }

                    //Delete the video
                    Db.DeleteVideo(selectedVideo.id);

                    //Feedback to the user
                    returntext = "Video successfully removed";
                }
                else
                {
                    returntext = "The Video doesn't exist";
                }
            }

            PopulateRemoveTags();
            PopulateRemoveVideos();
            removeTitle.Text = returntext;

            //Reset the display
            ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                ChangeText(bindTagTitle, "Remove Tags/Videos");
                ReEnable(removeTagSubmit);
                ReEnable(removeVideoSubmit);
            }, TimeSpan.FromSeconds(3));
        }

        #endregion

        private async void ChangeText(TextBlock target, String text)
        {
            //Run the 'reset display' code (that was waiting asynchronously) on the UI thread.
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                if (String.IsNullOrEmpty(text)) return;
                target.Text = text;
            });
        }

        private async void ReEnable(Button target)
        {
            //Run the 're-enable' code (that was waiting asynchronously) on the UI thread.
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                target.IsEnabled = true;
            });
        }
    }
}
