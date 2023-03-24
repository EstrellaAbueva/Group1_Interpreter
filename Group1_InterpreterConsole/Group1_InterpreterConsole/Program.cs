using Antlr4.Runtime;
using Group1_InterpreterConsole.CodeVisitor;
using Group1_InterpreterConsole.Contents;
using System.CodeDom.Compiler;

var fileName = "Contents\\test.code";

var fileContents = File.ReadAllText(fileName);

var inputStream = new AntlrInputStream(fileContents);

var codeLexer = new CodeLexer(inputStream);
var commonTokenStream = new CommonTokenStream(codeLexer);
var codeParser = new CodeParser(commonTokenStream);
var codeContext = codeParser.program();
var visitor = new Visitor();
visitor.Visit(codeContext);
