using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ever_Afters.common.Enums
{
    public enum Sensors
    {
        //We chose left -> right notation in order to keep clear of the reading order of the sensors, no matter their physical or numerical location.
        NFC_LEFT, NFC_LEFT_MIDDLE, NFC_MIDDLE, NFC_RIGHT_MIDDLE, NFC_RIGHT
    }
}
