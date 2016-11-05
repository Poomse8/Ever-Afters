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
        public static Queue CurrentInstance { get; private set; }

        //SINGLETON PATTERN
        public static Queue NewInstance()
        {
            if(CurrentInstance == null)
            {
                CurrentInstance = new Queue();
            }
            return CurrentInstance;
        }

        //Methods
        public static Video NextVideo()
        {
            
        }
    }
}
