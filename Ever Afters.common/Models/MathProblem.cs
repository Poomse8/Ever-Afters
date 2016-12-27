using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ever_Afters.common.Models
{
    public class MathProblem
    {
        //Properties
        public MathProblemType Type { get; set; }
        public Tag ExpectedAnswer { get; set; }
        public String Question { get; set; }

        public int input1 { get; set; }
        public int input2 { get; set; }
        public MathTerm Term { get; set; }
        public int output { get; set; }

        //Helper Methods
        public static String TermString(MathTerm term)
        {
            switch (term)
            {
                    case MathTerm.DIVIDE:
                    return " / ";
                    case MathTerm.EQUALS:
                    return " = ";
                    case MathTerm.MINUS:
                    return " - ";
                    case MathTerm.NOTEQUALS:
                    return " /= ";
                    case MathTerm.PLUS:
                    return " + ";
                    case MathTerm.TIMES:
                    return " x ";
            }
            return "?";
        }
    }
}
