using Ever_Afters.common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
