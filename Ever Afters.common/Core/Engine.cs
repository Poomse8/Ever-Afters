﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Ever_Afters.common.Enums;
using Ever_Afters.common.Listeners;
using Ever_Afters.common.Models;

namespace Ever_Afters.common.Core
{
    public class Engine : InputChangedListener
    {
        private const int RenderEngineLeadTime = 20; //In Ms

        public static readonly String ResourcePath = Path.Combine(ApplicationData.Current.GetPublisherCacheFolder("EverAfters").Path, "Resources");

        #region SingleTon

        #region Fields & Properties

        public IVisualisationHandler Screen { get; set; }
        public DataRequestHandler Database { get; set; }

        public bool Ignited { get; private set; }
        private bool extra_pack_exception = false;

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

        private static Engine _engine;

        public static Engine CurrentEngine => _engine ?? (_engine = new Engine(new Skel_Db(), new Skel_Screen()));

        public Engine(DataRequestHandler db, IVisualisationHandler screen)
        {
            Database = db;
            Screen = screen;

            //Make sure that the resources directory exists
            if (!Directory.Exists(ResourcePath)) Directory.CreateDirectory(ResourcePath);
        }

        #endregion

        #region Core

        private async void Ignite()
        {
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
                    if (duration < 0) duration = 0;
                    TimeSpan waitTime = TimeSpan.FromSeconds(duration);

                    if (waitTime.TotalMilliseconds >= 250)
                    {
                        //If longer than 250 ms, wait 250 ms and then rerun the engine. This to be able to not freeze the engine lock.
                        await Task.Delay(TimeSpan.FromMilliseconds(250));
                        Ignited = false;
                        Ignite();
                    }
                    else
                    {
                        //4. Set thread to sleep until video is expected
                        //new Task(Update).Wait(waitTime);
                        await Task.Delay(waitTime);
                        Update();
                    }
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
                        } else
                        {
                            PushNextOffScreenEnding();
                        }
                    } else
                    {
                        //4. Push the appropriate ending.
                        if (Queue.NextVideo.BaseStartsOnScreen)
                        {
                            PushNextOnScreenEnding();
                        } else
                        {
                            PushNextOffScreenEnding();
                        }
                    }
                }
            }

            //Restart the loop
            Ignited = false;

            //For extra packs - only the base path is provided. If the on- or offscreen path isn't known, reboot the engine without errors.
            if (extra_pack_exception)
            {
                CurrentlyPlaying.IsBase = false;
                extra_pack_exception = false;
                Ignite();
            }
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

            //2. Set the path relative to the current running instance
            if(next.BasePath != null) next.BasePath = Path.Combine(ResourcePath, next.BasePath);
            if(next.OnScreenEndingPath != null) next.OnScreenEndingPath = Path.Combine(ResourcePath, next.OnScreenEndingPath);
            if(next.OffScreenEndingPath != null) next.OffScreenEndingPath = Path.Combine(ResourcePath, next.OffScreenEndingPath);

            //3. Check if the video is valid
            if (File.Exists(next.BasePath))
            {
                //4. Replace the field and order the screen to play
                CurrentlyPlaying = PlayingVideo.MakeFromVideo(next);
                CurrentlyPlaying.SetBase();
                Screen.PlayVideo(new Uri(next.BasePath));
            }
        }

        private void PushNextOnScreenEnding()
        {
            //Check if the video is valid
            if (File.Exists(CurrentlyPlaying.OnScreenEndingPath))
            {
                Uri onscreen = new Uri(CurrentlyPlaying.OnScreenEndingPath);
                CurrentlyPlaying.SetEnding(Ending.Onscreen);
                Screen.PlayVideo(onscreen);
            } else if (CurrentlyPlaying.OnScreenEndingPath == null) extra_pack_exception = true;
        }

        private void PushNextOffScreenEnding()
        {
            //Check if the video is valid
            if (File.Exists(CurrentlyPlaying.OffScreenEndingPath))
            {
                Uri offscreen = new Uri(CurrentlyPlaying.OffScreenEndingPath);
                CurrentlyPlaying.SetEnding(Ending.Offscreen);
                Screen.PlayVideo(offscreen);
            } else if (CurrentlyPlaying.OffScreenEndingPath == null) extra_pack_exception = true;
        }

        #endregion

        #region Helper Functions

        private Video ResolveTag(String TagIdentifier)
        {
            if (Tag.tagExists(TagIdentifier))
            {
                Tag loadedTag = Database.LoadTagByName(TagIdentifier);
                return Database.LoadVideoFromTag(loadedTag);

            }
            Screen.DisplayError("The tag you scanned was not found in the database. We're sorry.");
            return null;
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

            if (force)
            {
                //2. Delete the currentlyplayingobject
                CurrentlyPlaying = null;

                //3. Interrupt the visual
                Screen.StopVideo();
                Screen.ClearVideo();
            }

            return success;
        }

        public void OnTagAdded(Sensors sensor, string TagIdentifier)
        {
            //Log to console
            Debug.WriteLine("Tag added: " + TagIdentifier);

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
            //Log to console
            Debug.WriteLine("Tag removed");

            //1. Pass command through queue
            Queue.RemoveFromQueue(SensorQueueConverter.Convert(sensor));
        }

        #endregion
    }
}
