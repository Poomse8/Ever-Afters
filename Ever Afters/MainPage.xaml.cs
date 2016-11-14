using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Chat;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.System;
using Windows.UI;
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
using Ever_Afters.DAL;

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

        //Nico's vars
        private bool _overrideKeyboard = true;
        private NfcEngine _nfcEngine = NfcEngine.Instance;
        private string _currentRead = String.Empty;
        private long prevCharMilliseconds;

        public MainPage()
        {
            this.InitializeComponent();
            InitialiseEngine();
            InitializeNfc();
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

        #region Buttons (Nico)

        private void btnReader_Click(object sender, RoutedEventArgs e)
        {
            UpdateBtnColors(sender);

        }

        private void UpdateBtnColors(object sender)
        {
            btnReader1.Background = new SolidColorBrush(Color.FromArgb(51, 00, 116, 255));
            btnReader2.Background = new SolidColorBrush(Color.FromArgb(51, 00, 116, 255));
            btnReader3.Background = new SolidColorBrush(Color.FromArgb(51, 00, 116, 255));
            btnReader4.Background = new SolidColorBrush(Color.FromArgb(51, 00, 116, 255));
            btnReader5.Background = new SolidColorBrush(Color.FromArgb(51, 00, 116, 255));

            var button = sender as Button;
            if (button != null) button.Background = new SolidColorBrush(Color.FromArgb(51, 00, 255, 139));
        }

        private void UserInputFocusGained(object sender, RoutedEventArgs e)
        {
            _overrideKeyboard = false;
        }

        private void UserInputFocusLost(object sender, RoutedEventArgs e)
        {
            txtNfcListener.Focus(FocusState.Keyboard);
            _overrideKeyboard = true;
        }
        
        private void InitializeNfc()
        {
            txtNfcListener.LostFocus += TxtNfcListener_LostFocus;
        }

        private void TxtNfcListener_LostFocus(object sender, RoutedEventArgs e)
        {
            if(_overrideKeyboard) txtNfcListener.Focus(FocusState.Keyboard);
        }

        #endregion

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (_overrideKeyboard)
            {
                long currentCharMilliseconds = DateTime.Now.Millisecond;
                if(currentCharMilliseconds - prevCharMilliseconds > 500)
                {
                    Debug.WriteLine(txtNfcListener.Text);
                }

                prevCharMilliseconds = currentCharMilliseconds;
            }
        }

        private void InitialiseNfcRead(char letter)
        {
            Debug.WriteLine("char ok:" + letter);
        }

    }
}
