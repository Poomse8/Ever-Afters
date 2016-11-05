using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ever_Afters.common.Models;

namespace Ever_Afters.Core
{
    public class Queue
    {
        #region SingleTon

        public static Queue CurrentInstance { get; private set; }

        //SINGLETON PATTERN
        public static Queue NewInstance()
        {
            if (CurrentInstance == null)
            {
                CurrentInstance = new Queue();
            }
            return CurrentInstance;
        }

        #endregion

        #region Core

        public static Video NextVideo { get; set; }

        #endregion

        #region Output Methods

        public static Video GiveNextVideo()
        {
            throw new NotImplementedException();
        }

        public static bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Input Methods

        public static void AddToQueue(Video video, QueuePosition position = QueuePosition.Nextfree)
        {
            
        }

        public static void RemoveFromQueue(QueuePosition position)
        {
            
        }

        #endregion
    }


}
