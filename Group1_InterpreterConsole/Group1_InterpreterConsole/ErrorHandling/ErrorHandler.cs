using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group1_InterpreterConsole.ErrorHandling
{
    public class ErrorHandler
    {
        public static bool ConditionChecker(object? value)
        {
            if(value is bool b)
                return b;
            
            throw new ArgumentException($"Cannot convert {value} to boolean.");
        }

     

    }
}
