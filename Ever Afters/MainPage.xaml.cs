﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Ever_Afters.common.Core;
using Ever_Afters.common.Enums;
using Ever_Afters.common.Listeners;
using Ever_Afters.common.DatabaseLayer;
using Ever_Afters.common.DAL;

namespace Ever_Afters
{
    public sealed partial class MainPage : Page, IVisualisationHandler
    {
        private InputChangedListener il;
        private bool toggle = false;

        //Math Pack Vars
        private bool _mathPackActive = false;

        //Nico's vars
        private bool _overrideKeyboard = true;
        private bool _queueClearQueued = false;
        private NfcEngine _nfcEngine = NfcEngine.Instance;
        private string _currentRead = String.Empty;
        private long prevCharMilliseconds;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            InitialiseEngine();
            InitializeNfc();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_overrideKeyboard) txtNfcInput.Focus(FocusState.Programmatic);
        }

        #region Render Engine

        public void InitialiseEngine()
        {
            //Register the screen & database in the engine.
            Engine.CurrentEngine.Screen = this;
            Engine.CurrentEngine.Database = SQLiteService.CurrentInstance;
            il = Engine.CurrentEngine;

            //MathEngine
            MathEngine.CurrentInstance.Screen = this;
            MathEngine.CurrentInstance.Database = MathPackDatapool.CurrentInstance;
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //DUMMY: Pointer Released servers as DAL.
            if (toggle == false)
            {
                il.OnTagAdded(Sensors.NFC_RIGHT_MIDDLE, "TEST06");
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

        public async void PlayVideo(Uri uri)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
                //Start the video
                mediaPlayer.Stop();
                mediaPlayer.Source = uri;
                mediaPlayer.Play();
                mediaPlayer.AutoPlay = true;

                //Log to console
                Debug.WriteLine(uri.Segments.Last());
            });
        }

        public async void StopVideo()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
                mediaPlayer.Pause();
            });
        }

        public async void ClearVideo()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
                mediaPlayer.Stop();
                mediaPlayer.Source = null;
                mediaPlayer.AutoPlay = false;
            });
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

        #endregion

        #region NFcInput & Buttons (Nico)

        private void btnReader_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            UpdateBtnColors(button);
            UpdateCurrentReader(int.Parse((string)button.Tag));
        }

        private void UpdateBtnColors(Button btn)
        {
            btnReader0.Background = new SolidColorBrush(Color.FromArgb(51, 00, 116, 255));
            btnReader1.Background = new SolidColorBrush(Color.FromArgb(51, 00, 116, 255));
            btnReader2.Background = new SolidColorBrush(Color.FromArgb(51, 00, 116, 255));
            btnReader3.Background = new SolidColorBrush(Color.FromArgb(51, 00, 116, 255));
            btnReader4.Background = new SolidColorBrush(Color.FromArgb(51, 00, 116, 255));
            btn.Background = new SolidColorBrush(Color.FromArgb(51, 00, 255, 139));
        }
        private void UpdateCurrentReader(int id)
        {
            _nfcEngine.ReaderId = (Sensors)id;
        }

        private void InitializeNfc()
        {
            txtNfcInput.LostFocus += txtNfcInput_LostFocus;
            _nfcEngine.il = il;
        }
        private async void txtNfcInput_LostFocus(object sender, RoutedEventArgs e)
        {
            //als de focus verloren is moet deze herstelt worden voor het inlezen van tags
            await Task.Delay(TimeSpan.FromMilliseconds(150));
            if (_overrideKeyboard) txtNfcInput.Focus(FocusState.Programmatic);
        }

        /// <summary>
        /// Roep deze methode aan als focus gained event tijdens userinput, dit pauzeert de nfc lezer zodat de gebruiker kan typen
        /// </summary>
        /// <param name="sender">De control die het event aanroept</param>
        /// <param name="e">Informatie over het event</param>
        private void UserInputFocusGained(object sender, RoutedEventArgs e)
        {
            _overrideKeyboard = false;
        }

        /// <summary>
        /// Roep deze methode aan als focus lost event tijdens userinput, dit zorgt ervoor dat nfc terug ingelezen kan worden nadat de user moest typen.
        /// </summary>
        /// <param name="sender">De control die het event aanroept</param>
        /// <param name="e">Informatie over het event</param>
        private void UserInputFocusLost(object sender, RoutedEventArgs e)
        {
            _overrideKeyboard = true;
            txtNfcInput.Focus(FocusState.Programmatic);
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (!_overrideKeyboard) return;
            //de gemiddelde leestijd tussen 2 tekens is 2 tot 7 ms
            //300 ms na eerste read queue clearen
            if (!_queueClearQueued) QueueQueueFlush();
            long currentCharMilliseconds = DateTime.Now.Millisecond;

            //Debug.WriteLine(currentCharMilliseconds - prevCharMilliseconds);
            prevCharMilliseconds = currentCharMilliseconds;
        }

        private async void QueueQueueFlush()
        {
            _queueClearQueued = true;
            await Task.Delay(TimeSpan.FromMilliseconds(300));
            _currentRead = txtNfcInput.Text;
            txtNfcInput.Text = string.Empty;
            _queueClearQueued = false;
            //Debug.WriteLine("read: " + _currentRead);

            PushReadToEngine(_currentRead);
        }

        private void PushReadToEngine(string currentRead)
        {
            _nfcEngine.SaveInput(currentRead);
        }

        #endregion

        #region MathPack

        private void ToggleMathPack_OnClick(object sender, RoutedEventArgs e)
        {
            if (_mathPackActive)
            {
                //Deactivate math pack
                btnToggleMathPack.Background = new SolidColorBrush(Color.FromArgb(51, 00, 116, 255));
                _mathPackActive = false;

                //Shutdown Math Engine
                MathEngine.CurrentInstance.ShutdownMathEngine();

                //Change Engine
                il = Engine.CurrentEngine;
                _nfcEngine.il = Engine.CurrentEngine;

                //Disable any overlay
                OverlayManager(null, false);
            }
            else
            {
                //Activate math pack
                btnToggleMathPack.Background = new SolidColorBrush(Color.FromArgb(51, 00, 255, 139));
                _mathPackActive = true;

                //Change Engine
                Engine.CurrentEngine.OnQueueClearRequest(true);
                il = MathEngine.CurrentInstance;
                _nfcEngine.il = MathEngine.CurrentInstance;

                //Request a new Math Equation
                MathEngine.CurrentInstance.StartupMathEngine();
            }
        }

        public async void OverlayManager(string overlayMessage, bool showOverlay = true, int xPosition = 400, int yPosition = -100)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
                //Position the overlay
                txtOverlay.Margin = new Thickness(xPosition, yPosition, 0, 0);

                //Set the visibility
                txtOverlay.Visibility = showOverlay ? Visibility.Visible : Visibility.Collapsed;

                //Display the message
                if (overlayMessage != null) txtOverlay.Text = overlayMessage;
            });
        }

        #endregion
    }
}
