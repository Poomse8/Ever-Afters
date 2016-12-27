using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ever_Afters.common.Listeners;
using Ever_Afters.common.Models;

namespace Ever_Afters.common.DatabaseLayer
{
    public class MathPackDatapool : IMathRequestHandler
    {
        #region Singleton

        private static MathPackDatapool _pool;

        public static MathPackDatapool CurrentInstance => _pool ?? (_pool = new MathPackDatapool());

        #endregion

        //RETURN NULL IF THE TAG ISNT FOUND
        public Tag GiveTermTag(MathTerm term)
        {
            return new Tag();
        }

        public Tag GiveNumberTag(int value)
        {
            return new Tag();
        }
    }
}
