using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System.Xml.Linq;

namespace Group1_InterpreterConsole.ErrorHandling
{
    public class ErrorHandler
    {
        public static bool? HandleConditionError([NotNull] ParserRuleContext context, object? value)
        {
            if(value is bool b)
            {
                return b;
            }
            else
            {
                var line = context.Start.Line;
                Console.WriteLine($"Semantic Error: in line {line}.");
                Console.WriteLine($"Cannot convert {value} to boolean.");
                Environment.Exit(400);
                return false;
            }
        }

        public static bool HandleTypeError([NotNull] ParserRuleContext context, object? obj, Type? type, string location)
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
                    Environment.Exit(400);
                    return false;
                }
            }
            else
            {
                var line = context.Start.Line;
                Console.WriteLine($"Semantic Error: in {location}, line {line}.");
                Console.WriteLine($"Cannot convert {obj?.GetType().Name.ToUpper()} to {type?.Name.ToUpper()}.");
                Environment.Exit(400);
                return false;
            }
        }

        public static void HandleUndefinedVariableError([NotNull] ParserRuleContext context, object? variableName)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in line {line}.\n" +
                              $"Variable '{variableName}' is not defined.");
            Environment.Exit(400);
        }

        public static bool HandleProgramCreationError([NotNull] ParserRuleContext context, string message, string location)
        {

            if (message.StartsWith("BEGIN CODE") && message.EndsWith("END CODE"))
            {
                return true;
            }
            else
            {
                /*var line = context.Start.Line;
                Console.WriteLine($"Semantic Error: in {location}, line {line}.");
                Console.WriteLine("Code must start with 'BEGIN CODE' and end with 'END CODE'.");*/
                return false;
            }
        }

        public static bool HandleUndeclaredVariableError([NotNull] ParserRuleContext context, Dictionary<string, object?> dictionary, string keyId)
        {
            if (dictionary.ContainsKey(keyId))
            {
                return true;
            }
            else
            {
                var line = context.Start.Line;
                Console.WriteLine($"Semantic Error: in line {line}.\n" +
                                  $"Variable '{keyId}' has not been declared.");
                Environment.Exit(400);
                return false;
            }
        }

        public static void HandleInvalidScanTypeError([NotNull] ParserRuleContext context, string input, string location)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in {location}, in line {line}.\n" +
                              $"Input data assigned to '{input}' is Invalid.");
            Environment.Exit(400);
        }

        public static void HandleInvalidOperatorError([NotNull] ParserRuleContext context, object? left, object? right, string op)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in line {line}.\n" +
                              $"Cannot {op} values of types {left?.GetType().Name.ToUpper()} and {right?.GetType().Name.ToUpper()}");
            Environment.Exit(400);
        }
    }
}
