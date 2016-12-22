using Ever_Afters.common.Listeners;
using Ever_Afters.common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ever_Afters.admin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DataRequestHandler db = null;

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
            beginGrid.Visibility = Visibility.Collapsed;
            upTagGrid.Visibility = Visibility.Collapsed;
            upVideoGrid.Visibility = Visibility.Collapsed;
            bindTagGrid.Visibility = Visibility.Collapsed;
            removeGrid.Visibility = Visibility.Collapsed;

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
        private void SubmitUpTag(object sender, PointerRoutedEventArgs e)
        {
            String returntext = "Unknown Error";
            String tagId = (String)upTagName.Text;
            if (!String.IsNullOrEmpty(tagId))
            {
                bool exists = Ever_Afters.common.Models.Tag.tagExists(tagId);
                if (!exists)
                {
                    db.SaveTag(tagId);
                    returntext = "Tag Saved Successfully";
                } else
                {
                    returntext = "The tag already exist!";
                }
            } else returntext = "The tagstring cannot be empty!";

            upTagTitle.Text = returntext;
            upTagName.Text = "";
        }
        #endregion

        #region upVideoGrid

        StorageFile baseVideoPath = null;
        StorageFile offscreenVideoPath = null;
        StorageFile onscreenVideoPath = null;

        private void UpVideoSubmit(object sender, PointerRoutedEventArgs e)
        {
            if(baseVideoPath != null && offscreenVideoPath != null && onscreenVideoPath != null)
            {

            }
        }

        private async void OpenDialogBaseVideo(object sender, PointerRoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
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

        private async void OpenDialogOnscreenEnding(object sender, PointerRoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
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

        private async void OpenDialogOffscreenEnding(object sender, PointerRoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
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
    }
}
