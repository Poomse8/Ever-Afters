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
        Tag GiveTermTag(MathTerm term);
        Tag GiveNumberTag(int value);
    }
}
