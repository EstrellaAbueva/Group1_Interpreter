using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Group1_InterpreterConsole.ErrorHandling
{
    public class ErrorHandler
    {
        public static bool? ConditionChecker(object? value)
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
                Console.WriteLine($"Cannot convert {value} to boolean.");
                return null;
            }
        }

        public static bool IsValidType([NotNull] ParserRuleContext context, object? obj, Type? type, string location)
        {
            if (obj is int || obj is float || obj is bool || obj is char || obj is string)
            {
                if (obj.GetType() == type)
                {
                    return true;
                }
                else
                {
                    var line = context.Start.Line;
                    Console.WriteLine($"Semantic Error: in {location}, line {line}.");
                    Console.WriteLine($"Cannot convert {obj?.GetType().Name.ToUpper()} to {type?.Name.ToUpper()}.");
                    return false;
                }
            }
            else
            {
                var line = context.Start.Line;
                Console.WriteLine($"Semantic Error: in {location}, line {line}.");
                Console.WriteLine($"Cannot convert {obj?.GetType().Name.ToUpper()} to {type?.Name.ToUpper()}.");
                return false;
            }
        }

        public static void HandleUndefinedVariableError(object? variableName)
        {
            Console.WriteLine($"Semantic Error: Variable '{variableName}' is not defined!");
        }
    }
}
