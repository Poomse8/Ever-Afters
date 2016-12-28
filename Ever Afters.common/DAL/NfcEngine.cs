using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ever_Afters.common.Enums;
using Ever_Afters.common.Listeners;

namespace Ever_Afters.common.DAL
{
    public class NfcEngine
    {
        private static NfcEngine instance = new NfcEngine();
        public static NfcEngine Instance { get { return instance; } }
        public Sensors ReaderId { get; set; }

        private List<string> _currentReads;

        public List<string> CurrentReads
        {
            get
            {
                if (_currentReads == null)
                {
                    _currentReads = new List<string>();
                    _currentReads.Insert(0, "");
                    _currentReads.Insert(1, "");
                    _currentReads.Insert(2, "");
                    _currentReads.Insert(3, "");
                    _currentReads.Insert(4, "");
                }
                return _currentReads;
            }
            set { _currentReads = value; }
        }

        public InputChangedListener il { get; set; }

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static NfcEngine() { }

        private NfcEngine() { }

        public void SaveInput(string input)
        {
            //Debug.WriteLine(CurrentReads.Count + "+" + CurrentReads.Capacity);
            if (CurrentReads[(int)ReaderId] == input)
            {
                CurrentReads[(int)ReaderId] = string.Empty;
                il.OnTagRemoved(ReaderId);
                Debug.WriteLine("Reader " + ReaderId + " cleared");
            }
            else
            {
                CurrentReads[(int)ReaderId] = input;
                il.OnTagAdded(ReaderId, input);
                Debug.WriteLine("Reader " + ReaderId + ": " + input);
            }
        }


    }
}
