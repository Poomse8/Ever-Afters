using Ever_Afters.common.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ever_Afters.common.Core;
using Ever_Afters.common.Listeners;

namespace Ever_Afters.common.Models
{
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

    public enum QueuePosition
    {
        Q1, Q2, Q3, Q4, Q5, Nextfree, Priority
    }

    public enum Ending
    {
        Onscreen, Offscreen
    }

    //DUMMYCLASSES
    #region DummyClasses

    public class DummyVisualisation : IVisualisationHandler
    {
        public double GetRemainingDuration()
        {
            return 1.0;
        }

        public void PlayVideo(Uri uri)
        {
            Debug.WriteLine("Playing " + uri.AbsolutePath);
        }

        public void StopVideo()
        {
            Debug.WriteLine("Stopping Video");
        }

        public void ClearVideo()
        {
            Debug.WriteLine("Clearing Video");
        }

        public void DisplayError(string errormessage)
        {
            Debug.WriteLine("Error: " + errormessage);
        }
    }

    public class DummyDAL
    {
        private readonly InputChangedListener _listener = Engine.NewInstance();

        public DummyDAL()
        {
                _listener.OnTagAdded(Sensors.NFC_LEFT, "MATH-01");
        }
    }

    public class DummyDB : DataRequestHandler
    {
        public Tag SaveTag(string TagName)
        {
            throw new NotImplementedException();
        }

        public Video SaveVideo(bool BaseStartsOnScreen, string BasePath, string OnScreenEndingPath, string OffScreenEndingPath)
        {
            throw new NotImplementedException();
        }

        public bool BindVideoToTag(Video video, Tag tag)
        {
            throw new NotImplementedException();
        }

        public Tag LoadTagByName(string TagIdentifier)
        {
            return new Tag() {id=1,name="MATH-01"};
        }

        public Video LoadVideoFromTag(Tag tag)
        {
            String path = System.IO.Directory.GetCurrentDirectory() + "/Ever Afters.common/Resources/a.mp4";
            return new Video()
            {
                BasePath = path,
                BaseStartsOnScreen = true,
                id = 1,
                OffScreenEndingPath = path,
                OnScreenEndingPath = path,
                Request_TAG = "MATH-01",
                TAGS = new List<string>() { "MATH-01" }
            };
        }

        public IEnumerable<Tag> GetUnboundTags()
        {
            throw new NotImplementedException();
        }

        public Tag UpdateTag(Tag UpdatedTag)
        {
            throw new NotImplementedException();
        }

        public Video UpdateVideo(Video UpdatedVideo)
        {
            throw new NotImplementedException();
        }

        public bool DeleteTag(int TagId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteVideo(int VideoId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteBinding(Tag tag)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
