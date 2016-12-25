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
            ShowGrids(2);
        }

        private void ToRemoveGrid(object sender, PointerRoutedEventArgs e)
        {
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
                    returntext = "The tag already exist!";
                }
            } else returntext = "The tagstring cannot be empty!";

            upTagTitle.Text = returntext;
            upTagName.Text = "";

            //Reset the display
            ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                ChangeText(upTagTitle, "Scan Tag");
                upTagSubmit.IsEnabled = true;
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
                        //Copy the videos to the resource directory
                        String path = Directory.GetCurrentDirectory() + "\\Ever Afters.common\\Resources";
                        StorageFolder folder = await StorageFolder.GetFolderFromPathAsync(path);

                        await baseVideoPath.CopyAsync(folder);
                        await offscreenVideoPath.CopyAsync(folder);
                        await onscreenVideoPath.CopyAsync(folder);

                        //Get the information about the beginning of the video
                        bool startsOnScreen = rbtOnScreen.IsChecked.Value;

                        //Save the video to the database
                        Db.SaveVideo(startsOnScreen, baseVideoPath.Name, onscreenVideoPath.Name, offscreenVideoPath.Name);

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
                    catch (Exception ex)
                    {
                        returntext = "file already exists.";
                    }
                }
            }

            upVideoTitle.Text = returntext;

            //Reset the display
            ThreadPoolTimer.CreatePeriodicTimer((t) =>
            {
                ChangeText(upVideoTitle, "Upload Video");
                upVideoSubmit.IsEnabled = true;
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

        private async void ChangeText(TextBlock target, String text)
        {
            //Run the 'reset display' code (that was waiting asynchronously) on the UI thread.
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                if (String.IsNullOrEmpty(text)) return;
                target.Text = text;
            });
        }
    }
}
