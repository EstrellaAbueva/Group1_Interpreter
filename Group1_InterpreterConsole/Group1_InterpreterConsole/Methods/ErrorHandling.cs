using Group1_InterpreterConsole.Contents;
using System.Diagnostics.CodeAnalysis;

namespace Group1_InterpreterConsole.Methods
{
    public class ErrorHandling
    {
        public static void ErrorProgram([NotNull] CodeParser.ProgramContext context)
        {
            if (context.BEGIN() is null || context.END() is null)
            {
                Console.WriteLine("Code must start with 'BEGIN CODE' and end with 'END CODE'.");
            }

            Environment.Exit(0);
        }
    }
}
