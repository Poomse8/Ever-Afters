﻿using Ever_Afters.common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Ever_Afters.common.Core;
using Ever_Afters.common.Listeners;

namespace Ever_Afters.common.Models
{

    #region Models
    public class PlayingVideo : Video
    {
        public bool IsBase { get; set; }
        public bool IsOnScreenEnding { get; set; }
        public bool IsOffScreenEnding { get; set; }

        public void SetBase()
        {
            this.IsBase = true;
            this.IsOnScreenEnding = false;
            this.IsOffScreenEnding = false;
        }

        public void SetEnding(Ending ending)
        {
            this.IsBase = false;
            if (ending == Ending.Onscreen)
            {
                this.IsOnScreenEnding = true;
                this.IsOffScreenEnding = false;
            }
            else
            {
                this.IsOnScreenEnding = false;
                this.IsOffScreenEnding = true;
            }
        }

        public static PlayingVideo MakeFromVideo(Video vid)
        {
            return new PlayingVideo()
            {
                id = vid.id,
                BaseStartsOnScreen = vid.BaseStartsOnScreen,
                BasePath = vid.BasePath,
                OnScreenEndingPath = vid.OnScreenEndingPath,
                OffScreenEndingPath = vid.OffScreenEndingPath,
                Request_TAG = vid.Request_TAG,
                TAGS = vid.TAGS
            };
        }
    }

    public class SensorQueueConverter
    {
        public static QueuePosition Convert(Sensors sensor)
        {
            switch (sensor)
            {
                case Sensors.NFC_LEFT:
                    return QueuePosition.Q1;
                case Sensors.NFC_LEFT_MIDDLE:
                    return QueuePosition.Q2;
                case Sensors.NFC_MIDDLE:
                    return QueuePosition.Q3;
                case Sensors.NFC_RIGHT_MIDDLE:
                    return QueuePosition.Q4;
                case Sensors.NFC_RIGHT:
                    return QueuePosition.Q5;
                default:
                    return QueuePosition.Nextfree;
            }
        }
    }

    #endregion

    #region enums

    public enum QueuePosition
    {
        Q1, Q2, Q3, Q4, Q5, Nextfree, Priority
    }

    public enum Ending
    {
        Onscreen, Offscreen
    }

    public enum MathProblemType
    {
        QuestionExpected, AnswerExpected, TermExpected
    }

    public enum MathTerm
    {
        EQUALS = 0, NOTEQUALS = 1, PLUS = 2, MINUS = 3, TIMES = 4, DIVIDE = 5
    }

    public enum MathVideos
    {
        BEGIN, MID, END_GOOD, END_BAD
    }

    #endregion

    #region Skeletons

    public class Skel_Db : DataRequestHandler, IMathRequestHandler
    {
        public Tag SaveTag(string TagName)
        {
            return null;
        }

        public Video SaveVideo(bool BaseStartsOnScreen, string BasePath, string OnScreenEndingPath, string OffScreenEndingPath)
        {
            return null;
        }

        public bool BindVideoToTag(Video video, Tag tag)
        {
            return false;
        }

        public Tag LoadTagByName(string TagIdentifier)
        {
            return new Tag() { id = 0, name = "SKEL" };
        }

        public Video LoadVideoFromTag(Tag tag)
        {
            //String path = "";
            //String path = ApplicationData.Current.LocalFolder.Path + "/Resources/skel.mp4";
            String path = ApplicationData.Current.GetPublisherCacheFolder("EverAfters").Path + "/Ever Afters.common/Resources/skel.mp4";
            return new Video()
            {
                BasePath = path,
                BaseStartsOnScreen = true,
                id = 1,
                OffScreenEndingPath = path,
                OnScreenEndingPath = path,
                Request_TAG = tag.name,
                TAGS = new List<string>() { "SKEL" }
            };
        }

        public IEnumerable<Tag> GetUnboundTags()
        {
            return new List<Tag>();
        }

        public IEnumerable<Tag> GetAllTags()
        {
            return new List<Tag>();
        }

        public IEnumerable<Video> GetAllVideos()
        {
            return new List<Video>();
        }

        public Tag UpdateTag(Tag UpdatedTag)
        {
            return UpdatedTag;
        }

        public Video UpdateVideo(Video UpdatedVideo)
        {
            return UpdatedVideo;
        }

        public bool DeleteTag(int TagId)
        {
            return false;
        }

        public bool DeleteVideo(int VideoId)
        {
            return false;
        }

        public bool DeleteBinding(Tag tag)
        {
            return false;
        }

        public List<Tag> GiveTermTag(MathTerm term)
        {
            return new List<Tag>();
        }

        public List<Tag> GiveNumberTag(int value)
        {
            return new List<Tag>();
        }

        public Video GiveVideo(MathVideos video)
        {
            return LoadVideoFromTag(null);
        }
    }

    public class Skel_Screen : IVisualisationHandler
    {
        public double GetRemainingDuration()
        {
            return 1.0;
        }

        public void PlayVideo(Uri uri)
        {

        }

        public void StopVideo()
        {

        }

        public void ClearVideo()
        {

        }

        public void DisplayError(string errormessage)
        {

        }

        public void OverlayManager(string overlayMessage, bool showOverlay = true, int xPosition = 500, int yPosition = 500)
        {
            
        }
    }

    #endregion
}
