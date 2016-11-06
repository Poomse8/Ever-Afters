using System;
using System.Collections.Generic;
using System.Linq;
using Ever_Afters.common.Models;

namespace Ever_Afters.common.Core
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

        private static QueuePosition pointer = QueuePosition.Q1;
        private static QueuePosition queuePointer = QueuePosition.Q1;
        private static Video[] VidQueue = new Video[5];
        private static List<Video> PriorityQueue = new List<Video>();

        private static void MovePointer(QueuePosition position)
        {
            pointer = position;
        }

        private static void MovePointerForward(ref QueuePosition pointer)
        {
            switch (pointer)
            {
                case QueuePosition.Q1:
                    pointer = QueuePosition.Q2;
                    break;
                case QueuePosition.Q2:
                    pointer = QueuePosition.Q3;
                    break;
                case QueuePosition.Q3:
                    pointer = QueuePosition.Q4;
                    break;
                case QueuePosition.Q4:
                    pointer = QueuePosition.Q5;
                    break;
                case QueuePosition.Q5:
                    pointer = QueuePosition.Q1;
                    break;
            }
        }

        private static Video GetVideoFromPointer(QueuePosition pointer, bool includePriority = false)
        {
            if (includePriority)
            {
                if(PriorityQueue.Count > 0) return PriorityQueue.First();
            }

            switch (pointer)
            {
                case QueuePosition.Q1:
                    return VidQueue[0];
                case QueuePosition.Q2:
                    return VidQueue[1];
                case QueuePosition.Q3:
                    return VidQueue[2];
                case QueuePosition.Q4:
                    return VidQueue[3];
                case QueuePosition.Q5:
                    return VidQueue[4];
            }

            return null;
        }

        private static void InsertOnPointer(Video vid)
        {
            switch (pointer)
            {
                case QueuePosition.Q1:
                    VidQueue[0] = vid;
                    break;
                case QueuePosition.Q2:
                    VidQueue[1] = vid;
                    break;
                case QueuePosition.Q3:
                    VidQueue[2] = vid;
                    break;
                case QueuePosition.Q4:
                    VidQueue[3] = vid;
                    break;
                case QueuePosition.Q5:
                    VidQueue[4] = vid;
                    break;
            }
        }

        private static void RemoveOnPointer()
        {
            switch (pointer)
            {
                case QueuePosition.Q1:
                    VidQueue[0] = null;
                    break;
                case QueuePosition.Q2:
                    VidQueue[1] = null;
                    break;
                case QueuePosition.Q3:
                    VidQueue[2] = null;
                    break;
                case QueuePosition.Q4:
                    VidQueue[3] = null;
                    break;
                case QueuePosition.Q5:
                    VidQueue[4] = null;
                    break;
            }
        }

        #endregion

        #region Output

        public static Video NextVideo => GetVideoFromPointer(queuePointer, true);

        public static Video GiveNextVideo()
        {
            if (PriorityQueue.Count > 0)
            {
                //1. Process the priority Queue
                Video now = PriorityQueue.First();
                PriorityQueue.Remove(now);
                return now;
            } else
            {
                //2. Process the normal Queue
                Video now = GetVideoFromPointer(queuePointer);
                MovePointerForward(ref queuePointer);
                
                //Move the pointer to the next video
                int count = 10;
                Video vidAtPos = GetVideoFromPointer(queuePointer);
                while (vidAtPos == null && count > 0)
                {
                    MovePointerForward(ref queuePointer);
                    vidAtPos = GetVideoFromPointer(queuePointer);
                    count--;
                }
                
                return now;
            }
        }

        public static bool IsEmpty()
        {
            bool isEmpty = true;
            foreach (Video vid in VidQueue)
            {
                if (vid != null)
                {
                    isEmpty = false;
                    break;
                }
            }
            return isEmpty;
        }

        #endregion

        #region Input

        public static void AddToQueue(Video video, QueuePosition position = QueuePosition.Nextfree)
        {
            switch (position)
            {
                case QueuePosition.Nextfree:
                    int count = 5;
                    pointer = queuePointer;
                    Video vidAtCurrentPosition = GetVideoFromPointer(pointer);

                    while (vidAtCurrentPosition != null && count >= 0)
                    {
                        MovePointerForward(ref pointer);
                        vidAtCurrentPosition = GetVideoFromPointer(pointer);
                        count--;
                    }

                    if (vidAtCurrentPosition == null)
                    {
                        InsertOnPointer(video);
                    }
                    break;
                case QueuePosition.Priority:
                    PriorityQueue.Add(video);
                    break;
                default:
                    MovePointer(position);
                    InsertOnPointer(video);
                    break;
            }
        }

        public static void RemoveFromQueue(QueuePosition position)
        {
            MovePointer(position);
            RemoveOnPointer();
        }

        public static Boolean ClearQueue()
        {
            //Reset the pointers
            pointer = QueuePosition.Q1;
            queuePointer = QueuePosition.Q1;

            //Check if the queue is already empty
            if (IsEmpty()) return false;

            //Clear the queue
            VidQueue = new Video[5];

            return true;
        }

        #endregion
    }


}
