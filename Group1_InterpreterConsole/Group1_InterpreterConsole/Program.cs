using Antlr4.Runtime;
using Group1_InterpreterConsole.Content;
using System.CodeDom.Compiler;

var fileName = "Content\\test.code";

var fileContents = File.ReadAllText(fileName);

var inputStream = new AntlrInputStream(fileContents);

var codeLexer = new CodeLexer(inputStream);
var commonTokenStream = new CommonTokenStream(codeLexer);
var codeParser = new CodeParser(commonTokenStream);
var codeContext = codeParser.program();
var visitor = new CodeVisitor();
visitor.Visit(codeContext);
