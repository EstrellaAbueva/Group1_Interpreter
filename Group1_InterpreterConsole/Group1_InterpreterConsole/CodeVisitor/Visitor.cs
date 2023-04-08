using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.ErrorHandling;
using Group1_InterpreterConsole.Functions;
using Group1_InterpreterConsole.Methods;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Group1_InterpreterConsole.Contents.CodeParser;

namespace Group1_InterpreterConsole.CodeVisitor
{
    public class Visitor : CodeBaseVisitor<object?>
    {
        private Dictionary<string, object?> Variables { get; set; } = new Dictionary<string, object?>();
        private Dictionary<string, object?> VarTypes { get; set; } = new Dictionary<string, object?>();

        public override object? VisitProgram([NotNull] CodeParser.ProgramContext context)
        {
            string code = context.GetText().Trim();
            if (ErrorHandler.HandleProgramCreationError(context,code,"Program Creation"))
            {
                // Visit each statement in the code
                foreach (var statementContext in context.statement())
                {
                    VisitStatement(statementContext);
                }
            }

            return null;
        }

        public override object? VisitAssignment([NotNull] CodeParser.AssignmentContext context)
        {
            foreach (var i in context.IDENTIFIER())
            {
                var expression = context.expression().Accept(this);

                // check type
                if (ErrorHandler.HandleTypeError(context, expression, (Type?)VarTypes[i.GetText()], "Variable Assignment"))
                { 
                    Variables[i.GetText()] = expression;
                }  
            }
            return null;
        }

        public override object? VisitVariable([NotNull] CodeParser.VariableContext context)
        {
            var varName = context.IDENTIFIER().GetText();

            if (Variables != null && Variables.TryGetValue(varName, out object? value))
            {
                return value;
            }
            else
            {
                ErrorHandler.HandleUndefinedVariableError(context, varName);
                return null;
            }
        }

        public override object? VisitConstant([NotNull] CodeParser.ConstantContext context)
        {
            var constant = context.GetText();

            if (constant.StartsWith("\"") && constant.EndsWith("\""))
            {
                return constant.Substring(1, constant.Length - 2);
            }
            else if (constant.StartsWith("'") && constant.EndsWith("'"))
            {
                return constant[1];
            }
            else if (context.BOOL() != null)
            {
                return bool.Parse(context.BOOL().GetText());
            }
            else if (context.INT() != null)
            {
                return int.Parse(context.INT().GetText());
            }
            else if (context.FLOAT() != null)
            {
                return float.Parse(context.FLOAT().GetText());
            }
            else if (context.STRING() != null)
            {
                string text = context.STRING().GetText();
                // Remove the enclosing quotes from the string
                text = text.Substring(1, text.Length - 2);
                // Replace escape sequences with their corresponding characters
                text = Regex.Replace(text, @"\\(.)", "$1");
                return text;
            }
            else if (context.CHAR() != null)
            {
                return context.CHAR().GetText()[1];
            }
            else
            {
                //no need to implement Error Handler
                throw new InvalidOperationException("Unknown literal type");
            }
        }

        public override object? VisitDisplay([NotNull] CodeParser.DisplayContext context)
        {
            var exp = Visit(context.expression());

            if (exp is bool b)
                exp = b.ToString().ToUpper();

            Console.Write(exp);

            return null;
        }

        public override object VisitType([NotNull] CodeParser.TypeContext context)
        {
            switch (context.GetText())
            {
                case "INT":
                    return typeof(int);
                case "FLOAT":
                    return typeof(float);
                case "BOOL":
                    return typeof(bool);
                case "CHAR":
                    return typeof(char);
                case "STRING":
                    return typeof(string);
                default:
                    //no need to implement Error Handler
                    throw new NotImplementedException("Invalid Data Type");
            }
        }

        public override object? VisitDeclaration([NotNull] CodeParser.DeclarationContext context)
        {
            var type = Visit(context.type());
            var typestr = context.type().GetText();
            var varnames = context.IDENTIFIER();

            // remove type
            var contextstring = context.GetText().Replace(typestr, "");

            var contextParts = contextstring.Split(',');
            var exp = context.expression();
            int expctr = 0;

            // traverse each part
            for (int x = 0; x < contextParts.Length; x++)
            {
                if (Variables.ContainsKey(varnames[x].GetText()))
                {
                    Console.WriteLine(varnames[x].GetText() + "is already declared");
                    continue;
                }
                if (contextParts[x].Contains('='))
                {
                    if (expctr < exp.Count())
                    {
                        // check type
                        if (ErrorHandler.HandleTypeError(context, Visit(exp[expctr]), (Type?)type, "Variable Declaration"))
                        {
                            Variables[varnames[x].GetText()] = Visit(exp[expctr]);
                            VarTypes[varnames[x].GetText()] = type;
                        }
                        expctr++;
                    }
                }
                else
                {
                    Variables[varnames[x].GetText()] = null;
                    VarTypes[varnames[x].GetText()] = type;
                }
            }
            return null;
        }

        public override object? VisitVariable_dec([NotNull] CodeParser.Variable_decContext context)
        {
            foreach (var declarationContext in context.declaration())
            {
                VisitDeclaration(declarationContext);
            }
            return null;
        }

        public override object VisitConcatOpExpression([NotNull] CodeParser.ConcatOpExpressionContext context)
        {
            // Get the left and right expressions;
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            var output = "";
            
            if(left is bool b)
                left = b.ToString().ToUpper();

            if(right is bool c)
                right = c.ToString().ToUpper();

            output = $"{left}{right}";
            return output;
        }

        public override object? VisitIdentifierExpression([NotNull] CodeParser.IdentifierExpressionContext context)
        {
            var identifier = context.IDENTIFIER().GetText();

            if (Variables.ContainsKey(identifier))
            {
                return Variables[identifier];
            }
            else
            {
                ErrorHandler.HandleUndefinedVariableError(context, identifier);
                return null;
            }
        }

        public override object? VisitConstantExpression([NotNull] CodeParser.ConstantExpressionContext context)
        {
            if (context.constant().INT() is { } i)
                return int.Parse(i.GetText());
            else if (context.constant().FLOAT() is { } f)
                return float.Parse(f.GetText());
            else if (context.constant().CHAR() is { } g)
                return g.GetText()[1];
            else if (context.constant().BOOL() is { } b)
                return b.GetText().Equals("\"TRUE\"");
            else if (context.constant().STRING() is { } s)
                return s.GetText()[1..^1];
            //no need to implement Error Handler
            throw new NotImplementedException();
        }

        public override object? VisitVariable_assignment([NotNull] CodeParser.Variable_assignmentContext context)
        {
            var name = context.IDENTIFIER().GetText();

            VarTypes[name] = Visit(context.type());
            return Variables[name] = null;
        }

        public override object? VisitUnaryExpression([NotNull] CodeParser.UnaryExpressionContext context)
        {
            return Operators.Unary(context.unary_operator().GetText(), Visit(context.expression()));
        }

        public override object? VisitAdditiveExpression([NotNull] AdditiveExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var ops = context.add_operator().GetText();

            return ops switch
            {
                "+" => Operators.Add(context, left, right),
                "-" => Operators.Subtract(context, left, right),
                _ => throw new NotImplementedException(),
            };
        }

        public override object? VisitMultiplicativeExpression([NotNull] MultiplicativeExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var ops = context.multiply_operator().GetText();

            return ops switch
            {
                "*" => Operators.Multiply(context, left, right),
                "/" => Operators.Divide(context, left, right),
                "%" => Operators.Modulo(context, left, right),
                _ => throw new NotImplementedException(),
            };
        }

        public override object? VisitRelationalExpression([NotNull] RelationalExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var ops = context.compare_operator().GetText();

            return Operators.Relational(left, right, ops);
        }

        public override object? VisitParenthesisExpression([NotNull] ParenthesisExpressionContext context)
        {
            return Visit(context.expression());
        }

        public override object? VisitNOTExpression([NotNull] NOTExpressionContext context)
        {
            var expressionValue = Visit(context.expression());

            return Operators.Negation(expressionValue);
        }

        public override object? VisitBoolOpExpression([NotNull] BoolOpExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            var boolop = context.bool_operator().GetText();

            return Operators.BoolOperation(left, right, boolop);
        }

        public override object? VisitIf_block([NotNull] If_blockContext context)
        {
            var condition = Visit(context.expression());

            var result = ErrorHandler.HandleConditionError(context, condition);
            result = Convert.ToBoolean(result);
            if (ErrorHandler.HandleConditionError(context, condition) == true)
            {
                var lines = context.line().ToList();
                foreach (var line in lines)
                {
                    Visit(line);
                }
            }
            else
            {
                var elseIfBlocks = context.else_if_block();
                foreach (var elseIfBlock in elseIfBlocks)
                {
                    var elseIfCondition = Visit(elseIfBlock.expression());
                    if (ErrorHandler.HandleConditionError(context, elseIfCondition) == true)
                    {
                        var elseIfLines = elseIfBlock.line().ToList();
                        foreach (var line in elseIfLines)
                        {
                            Visit(line);
                        }
                        // Exit the loop once the first true else-if block is found
                        return null;
                    }
                }

                var elseBlock = context.else_block();
                if (elseBlock != null)
                {
                    var elseLines = elseBlock.line().ToList();
                    foreach (var line in elseLines)
                    {
                        Visit(line);
                    }
                }
            }
            return null;
        }

        public override object? VisitWhile_loop([NotNull] While_loopContext context)
        {
            var condition = Visit(context.expression());

            while (ErrorHandler.HandleConditionError(context, condition) == true)
            {
                var lines = context.line().ToList();
                foreach (var line in lines)
                {
                    Visit(line);
                }
                condition = Visit(context.expression());
            }
            return null;
        }

        public override object? VisitEscapeSequenceExpression([NotNull] EscapeSequenceExpressionContext context)
        {
            var sequence = context.GetText()[1];
            return Operators.Escape(sequence) ?? throw new ArgumentException($"Invalid escape sequence: {context.GetText()}"); //no need to implement Error Handler
        }

        public override object? VisitNewlineOpExpression([NotNull] NewlineOpExpressionContext context)
        {
            var count = context.ChildCount - 1;
            var output = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                var childContext = context.GetChild(i);
                var childOutput = Visit(childContext);

                output.Append(childOutput);
            }

            output.Append(Environment.NewLine);

            return output;
        }

        public override object? VisitScan([NotNull] ScanContext context)
        {
            foreach (var id in context.IDENTIFIER())
            {
                string idName = id.GetText();
                if (ErrorHandler.HandleUndeclaredVariableError(context, VarTypes, idName))
                {
                    //testing purposes can be removed or kept after review
                    Console.Write($"Awaiting Input for {idName}: ");

                    string input = Console.ReadLine() ?? "";
                    if (idName != null)
                    {
                        switch (VarTypes[idName])
                        {
                            case int:
                                Variables[idName] = Convert.ToInt32(input);
                                break;
                            case float:
                                Variables[idName] = Convert.ToDouble(input);
                                break;
                            case bool:
                                Variables[idName] = Convert.ToBoolean(input);
                                break;
                            case char:
                                Variables[idName] = Convert.ToChar(input);
                                break;
                            case string:
                                Variables[idName] = Convert.ToString(input);
                                break;
                            default:
                                {
                                    ErrorHandler.HandleInvalidScanTypeError(context, idName, "Input Scan");
                                    break;
                                }
                        }

                    }
                }                
            }
            return null;
        }

    }
}
