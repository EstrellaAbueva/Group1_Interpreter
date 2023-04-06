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
            if (value != null && value.Equals("TRUE")) return true;
            if (value != null && value.Equals("FALSE")) return false;

            throw new Exception($"Cannot convert {value} to bool");
        }

        public static bool IsValidType(object? obj)
        {
            return obj is int || obj is float || obj is bool || obj is char || obj is string;
        }
    }
}
