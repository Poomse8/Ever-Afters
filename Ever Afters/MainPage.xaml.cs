using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Ever_Afters.common.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ever_Afters
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {       
        public MainPage()
        {
            this.InitializeComponent();
            String parentDir = Directory.GetCurrentDirectory();
            mediaPlayer.Source = new Uri(parentDir + @"/Ever Afters.common/Resources/a.mp4");

            //Init the dummies
            DummyDAL dd = new DummyDAL();
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (mediaPlayer.CurrentState == MediaElementState.Playing)
            {
                mediaPlayer.Pause();
            }
            else
            {
                mediaPlayer.Play();
                Debug.WriteLine(mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds);
                Debug.WriteLine(mediaPlayer.Position.TotalSeconds);
            }
        }
    }
}
