using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ever_Afters.common.Listeners
{
    public interface IVisualisationHandler
    {
        Double GetRemainingDuration();
        void PlayVideo(Uri uri);
        void StopVideo();
        void ClearVideo();
        void DisplayError(String errormessage);

        void OverlayManager(String overlayMessage, bool showOverlay = true, int xPosition = 400, int yPosition = -100);
    }
}
