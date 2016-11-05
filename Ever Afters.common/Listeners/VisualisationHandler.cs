using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ever_Afters.common.Listeners
{
    public interface VisualisationHandler
    {
        Double GetRemainingDuration();
        void PlayVideo(Uri uri);
    }
}
