using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ever_Afters.common.Models;

namespace Ever_Afters.common.Listeners
{
    public interface IMathRequestHandler
    {
        List<Tag> GiveTermTag(MathTerm term);
        List<Tag> GiveNumberTag(int value);
        Video GiveVideo(MathVideos video);
    }
}
