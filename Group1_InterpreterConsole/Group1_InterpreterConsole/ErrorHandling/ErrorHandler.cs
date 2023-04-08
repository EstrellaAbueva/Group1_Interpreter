using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Group1_InterpreterConsole.ErrorHandling
{
    public class ErrorHandler
    {
        public bool? ConditionChecker(object? value)
        {
            if(value is bool b)
                return b;
            
            throw new ArgumentException($"Cannot convert {value} to boolean.");
        }

        public bool IsValidType([NotNull] ParserRuleContext context, object? obj, Type? type, string location)
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

        public void HandleUndefinedVariableError([NotNull] ParserRuleContext context, object? variableName)
        {
                var line = context.Start.Line;
                Console.WriteLine($"Semantic Error: in line {line}.\n" +
                                  $"Variable '{variableName}' is not defined.");
        }

        public bool HandleProgramCreationError([NotNull] ParserRuleContext context, string message, string location)
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
    }
}
