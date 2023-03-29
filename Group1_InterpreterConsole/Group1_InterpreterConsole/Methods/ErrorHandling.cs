using Antlr4.Runtime;

namespace Group1_InterpreterConsole.Methods
{
    public class ErrorHandling: BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            Console.Error.WriteLine($"line {line}:{charPositionInLine} {msg}");
        }
    }
}
