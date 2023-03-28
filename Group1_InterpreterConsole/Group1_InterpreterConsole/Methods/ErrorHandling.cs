using Group1_InterpreterConsole.Contents;
using System.Diagnostics.CodeAnalysis;

namespace Group1_InterpreterConsole.Methods
{
    public class ErrorHandling
    {
        public static object? ErrorProgram([NotNull] CodeParser.ProgramContext context)
        {
            string code = context.GetText().Trim();
            if (code.StartsWith("BEGIN CODE") && code.EndsWith("END CODE") || context.BEGIN() is null || context.END() is null)
            {
                Console.WriteLine("Code must start with 'BEGIN CODE' and end with 'END CODE'.");
            }

            Environment.Exit(0);
            return null;
        }
    }
}
