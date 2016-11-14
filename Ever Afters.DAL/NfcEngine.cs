using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ever_Afters.DAL
{
    public class NfcEngine
    {
        private static NfcEngine instance = new NfcEngine();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static NfcEngine() { }

        private NfcEngine(){ }

        public static NfcEngine Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
