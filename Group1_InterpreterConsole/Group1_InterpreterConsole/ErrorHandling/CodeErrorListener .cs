using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Group1_InterpreterConsole.Methods
{
    public class ErrorHandling: BaseErrorListener
    {
        public override void SyntaxError
        ([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol,
        int line, int charPositionInLine, [NotNull] string msg,
        [Nullable] RecognitionException e)
        {
            Console.Error.WriteLine($"Syntax error: Unexpected symbol {offendingSymbol.Text.Replace("\r\n", "NEWLINE")} at line {line}, column {charPositionInLine + 1}");
            Console.Error.WriteLine($"Details: {msg[0].ToString().ToUpper() + msg[1..].Replace("\\r\\n", "")}");
            Environment.Exit(400);
            base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
        }
    }
}
