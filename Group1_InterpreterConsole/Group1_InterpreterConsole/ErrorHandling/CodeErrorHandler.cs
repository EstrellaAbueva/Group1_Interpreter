using Group1_InterpreterConsole.Contents;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group1_InterpreterConsole.Methods
{
    public class CodeErrorHandler
    {
        public static object? ThrowError(int line, string message)
        {
            Console.WriteLine($"Error: Line {line}.");
            Console.WriteLine("Details: " + message);
            Environment.Exit(400);

            return null;
        }
    }
}
