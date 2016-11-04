using Ever_Afters.common.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ever_Afters.common.Enums;

namespace Ever_Afters.Core
{
    public class Engine : InputChangedListener
    {
        //Global Variables
        DataRequestHandler Database;


        //InputChangedListener Interface
        public bool OnQueueClearRequest(bool force)
        {
            throw new NotImplementedException();
        }

        public void OnTagAdded(Sensors sensor, string TagIdentifier)
        {
            throw new NotImplementedException();
        }

        public void OnTagRemoved(Sensors sensor)
        {
            throw new NotImplementedException();
        }
    }
}
