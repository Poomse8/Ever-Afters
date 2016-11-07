using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ever_Afters.common.Models
{
    public class Error
    {
        public String NotFound
        {
            get { return "The tag you scanned was not found in the database. We're sorry."; }
        }

        public String Generic
        {
            get { return "An unknown error has occured. We're terribly sorry."; }
        }
    }
}
