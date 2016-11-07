using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Ever_Afters.common.Core;
using Ever_Afters.common.Enums;
using Ever_Afters.common.Listeners;
using Ever_Afters.common.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Ever_Afters
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IVisualisationHandler
    {
        private InputChangedListener il;
        private bool toggle = false;

        public MainPage()
        {
            this.InitializeComponent();
            InitialiseEngine();
        }

        public void InitialiseEngine()
        {
            //Register the screen in the engine.
            Engine.NewInstance().Screen = this;
            il = Engine.NewInstance();
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //DUMMY: Pointer Released servers as DAL.
            if (toggle == false)
            {
                il.OnTagAdded(Sensors.NFC_RIGHT_MIDDLE, "SKEL");
                toggle = true;
            }
            else
            {
                il.OnTagRemoved(Sensors.NFC_RIGHT_MIDDLE);
                toggle = false;
            }
        }

        public double GetRemainingDuration()
        {
            if (mediaPlayer.Source == null) return 0.1;
            Double total = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            Double progress = mediaPlayer.Position.TotalSeconds;
            return total - progress;
        }

        public void PlayVideo(Uri uri)
        {
            mediaPlayer.Stop();
            mediaPlayer.Source = uri;
            mediaPlayer.Play();
            mediaPlayer.AutoPlay = true;
        }

        public void StopVideo()
        {
            mediaPlayer.Pause();
        }

        public void ClearVideo()
        {
            mediaPlayer.Stop();
            mediaPlayer.Source = null;
            mediaPlayer.AutoPlay = false;
        }

        public void DisplayError(string errormessage)
        {
            ChangeErrorText(errormessage);
            Task.Delay(TimeSpan.FromSeconds(6));
            ChangeErrorText("");
        }

        public void ChangeErrorText(String text)
        {
            txtError.Text = text;
        }

        private void state(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.CurrentState == MediaElementState.Playing)
            {
                Engine.CurrentEngine.VideoStarted();
            }
        }
    }
}
