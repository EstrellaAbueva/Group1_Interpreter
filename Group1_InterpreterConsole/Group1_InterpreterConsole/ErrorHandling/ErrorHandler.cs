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
            if (value?.ToString()?.Equals("True", StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            else if (value?.ToString()?.Equals("False", StringComparison.OrdinalIgnoreCase) == true)
            {
                return false;
            }
            else
            {
                throw new ArgumentException($"Cannot convert {value} to boolean.");
            }
        }

        public static bool IsValidType(object? obj)
        {
            return obj is int || obj is float || obj is bool || obj is char || obj is string;
        }
    }
}
