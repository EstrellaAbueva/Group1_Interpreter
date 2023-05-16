using Antlr4.Runtime;
using Group1_InterpreterConsole.CodeVisitor;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.Methods;

bool isContinue = true;

while(isContinue) {
    var file = "..\\..\\..\\Contents\\test.code";
    var fileContents = File.ReadAllText(file);

    var inputStream = new AntlrInputStream(fileContents);

    // Create a lexer and parser for the code
    var codeLexer = new CodeLexer(inputStream);
    CommonTokenStream commonTokenStream = new CommonTokenStream(codeLexer);
    var codeParser = new CodeParser(commonTokenStream);

    // Error Listener
    var errorListener = new ErrorHandling();
    codeParser.AddErrorListener(errorListener);

    var codeContext = codeParser.program();
    var visitor = new Visitor();
    visitor.Visit(codeContext);

    Console.WriteLine("\n");
    Console.Write("Finish?(Y/N): ");
    var res = Console.ReadLine()![0];

    isContinue = (res == 'N' || res == 'n') ? true : false;

    Console.WriteLine("---------------------------------------------------------------");
    Console.WriteLine("\n\n");
}