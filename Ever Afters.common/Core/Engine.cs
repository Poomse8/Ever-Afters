using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Ever_Afters.common.Enums;
using Ever_Afters.common.Listeners;
using Ever_Afters.common.Models;

namespace Ever_Afters.common.Core
{
    public class Engine : InputChangedListener
    {
        private const int RenderEngineLeadTime = 20; //In Ms

        #region SingleTon

        #region Fields & Properties

        public IVisualisationHandler Screen { get; set; }
        public DataRequestHandler Database { get; set; }

        public bool Ignited { get; private set; }

        private Random _random;

        public bool RandomBool
        {
            get
            {
                if (_random == null) _random = new Random();
                return (_random.NextDouble() >= 0.5);
            }
        }

        public PlayingVideo CurrentlyPlaying { get; private set; }

        #endregion

        public static Engine CurrentEngine { get; private set; }

        public static Engine NewInstance(DataRequestHandler db = null, IVisualisationHandler scn = null)
        {
            if(db == null) db = new Skel_Db();
            if(scn == null) scn = new Skel_Screen();

            return CurrentEngine ?? (CurrentEngine = new Engine(db, scn));
        }

        public Engine(DataRequestHandler db, IVisualisationHandler screen)
        {
            Database = db;
            Screen = screen;
        }

        #endregion

        #region Core

        private async void Ignite()
        {
            Debug.WriteLine("Ignited");
            //1. Check if the engine isn't already working
            if (!Ignited)
            {
                //2. Check if the queue isn't empty
                if (!Queue.IsEmpty() || CurrentlyPlaying != null)
                {
                    Ignited = true;

                    //3. Check with Screen -> How long until the next video is expected?
                    Double duration = Screen.GetRemainingDuration();
                    if (duration - RenderEngineLeadTime > 0) duration -= RenderEngineLeadTime;
                    TimeSpan waitTime = TimeSpan.FromSeconds(duration);

                    //4. Set thread to sleep until video is expected
                    //new Task(Update).Wait(waitTime);
                    await Task.Delay(waitTime);
                    Update();
                }
            }
        }

        private void Update()
        {
            //1. Is there a video playing?
            if (CurrentlyPlaying == null) PushNextVideo();
            else
            {
                //2. Look at the currently playing video. Is it the base or an ending?
                if (CurrentlyPlaying.IsBase == false) PushNextVideo();
                else
                {
                    //3. Check if there is a video in queue.
                    if (Queue.IsEmpty())
                    {
                        //4. Select a random ending.
                        if (RandomBool)
                        {
                            PushNextOnScreenEnding();
                        }
                        else
                        {
                            PushNextOffScreenEnding();
                        }
                    }
                    else
                    {
                        //4. Push the appropriate ending.
                        if (Queue.NextVideo.BaseStartsOnScreen)
                        {
                            PushNextOnScreenEnding();
                        }
                        else
                        {
                            PushNextOffScreenEnding();
                        }
                    }
                }
            }

            //Restart the loop
            Ignited = false;
        }

        private void PushNextVideo()
        {
            if (Queue.IsEmpty())
            {
                CurrentlyPlaying = null;
                return;
            }

            //1. Get the next video from the queue
            Video next = Queue.GiveNextVideo();

            //2. Replace the field and order the screen to play
            CurrentlyPlaying = PlayingVideo.MakeFromVideo(next);
            CurrentlyPlaying.SetBase();
            Screen.PlayVideo(new Uri(next.BasePath));
        }

        private void PushNextOnScreenEnding()
        {
            Uri onscreen = new Uri(CurrentlyPlaying.OnScreenEndingPath);
            CurrentlyPlaying.SetEnding(Ending.Onscreen);
            Screen.PlayVideo(onscreen);
        }

        private void PushNextOffScreenEnding()
        {
            Uri offscreen = new Uri(CurrentlyPlaying.OffScreenEndingPath);
            CurrentlyPlaying.SetEnding(Ending.Offscreen);
            Screen.PlayVideo(offscreen);
        }

        #endregion

        #region Helper Functions

        private Video ResolveTag(String TagIdentifier)
        {
            if (Tag.tagExists(TagIdentifier))
            {
                Tag loadedTag = Database.LoadTagByName(TagIdentifier);
                return Database.LoadVideoFromTag(loadedTag);

            } else
            {
                Screen.DisplayError("The tag you scanned was not found in the database. We're sorry.");
                return null;
            }
        }

        public void VideoStarted()
        {
            Ignite();
        }

        #endregion

        #region InputChangedListener Interface

        public bool OnQueueClearRequest(bool force)
        {
            //1. Clear the queue
            bool success = Queue.ClearQueue();

            //2. Delete the currentlyplayingobject
            CurrentlyPlaying = null;

            //3. Interrupt the visual
            Screen.StopVideo();
            Screen.ClearVideo();

            return success;
        }

        public void OnTagAdded(Sensors sensor, string TagIdentifier)
        {
            //1. Resolve tag from database
            Video vid = ResolveTag(TagIdentifier);
            if (vid == null) return;

            //2. Add the new video to the queue
            Queue.AddToQueue(vid, SensorQueueConverter.Convert(sensor));

            //3. Start the Engine.
            Ignite();
        }

        public void OnTagRemoved(Sensors sensor)
        {
            //1. Pass command through queue
            Queue.RemoveFromQueue(SensorQueueConverter.Convert(sensor));
        }

        #endregion
    }
}
