using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.Functions;
using Group1_InterpreterConsole.Methods;
using System;
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
        private Operators op = new();

        public override object? VisitProgram([NotNull] CodeParser.ProgramContext context)
        {
            string code = context.GetText().Trim();
            if (code.StartsWith("BEGIN CODE") && code.EndsWith("END CODE"))
            {
                // Visit each statement in the code
                foreach (var statementContext in context.statement())
                {
                    VisitStatement(statementContext);
                }
            }
            else
            {
                Console.WriteLine("Code must start with 'BEGIN CODE' and end with 'END CODE'.");
            }

            return null;
        }

        public override object? VisitAssignment([NotNull] CodeParser.AssignmentContext context)
        {
            var identifier = context.IDENTIFIER();
            foreach (var i in identifier)
            {
                var expression = context.expression().Accept(this);

                // check type
                if (VarTypes[i.GetText()] == expression?.GetType())
                {
                    Variables[i.GetText()] = expression;
                }
                else
                {
                    throw new InvalidCastException($"Cannot convert type {expression?.GetType()} to {VarTypes[i.GetText()]}");
                }   
            }

            return null;
        }

        public override object? VisitVariable([NotNull] CodeParser.VariableContext context)
        {
            var varName = context.IDENTIFIER().GetText();
            return Variables?.GetValueOrDefault(varName) ?? throw new Exception($"Variable {varName} not found");
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
                return bool.Parse(context.BOOL().GetText().ToUpper());
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
                    throw new NotImplementedException("Invalid Data Type");
            }
        }

        public override object? VisitComparison([NotNull] CodeParser.ComparisonContext context)
        {
            var left = Visit(context.expression()[0]);
            var right = Visit(context.expression()[1]);

            if (left is null || right is null)
            {
                throw new Exception("Cannot compare null values");
            }

            var op = context.comparison_operator().GetText();

            switch (op)
            {
                case "==":
                    return left.Equals(right);
                case "!=":
                    return !left.Equals(right);
                case ">":
                    return (dynamic)left > (dynamic)right;
                case ">=":
                    return (dynamic)left >= (dynamic)right;
                case "<":
                    return (dynamic)left < (dynamic)right;
                case "<=":
                    return (dynamic)left <= (dynamic)right;
                default:
                    throw new NotImplementedException();
            }

        }

        public override object? VisitStatement([NotNull] CodeParser.StatementContext context)
        {
            if (context.assignment() != null)
            {
                return VisitAssignment(context.assignment());
            }
            else if (context.display() != null)
            {
                return VisitDisplay(context.display());
            }
            else if (context.declaration() != null)
            {
                return VisitDeclaration(context.declaration());
            }
            else if (context.variable_assignment() != null)
            {
                return VisitVariable_assignment(context.variable_assignment());
            }
            else if (context.variable() != null)
            {
                return VisitVariable(context.variable());
            }else if(context.scan() != null)
            {
                return VisitScan(context.scan());
            }
            else
            {
                throw new Exception($"Unknown statement type: {context.GetText()}");
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
                        if (type == Visit(exp[expctr])?.GetType()){
                            Variables[varnames[x].GetText()] = Visit(exp[expctr]);
                            VarTypes[varnames[x].GetText()] = type;
                            expctr++;
                        }
                        else
                        {
                            throw new InvalidCastException($"Cannot convert type {Visit(exp[expctr])?.GetType()} to {type}");
                        }
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
            var left = context.expression()[0].Accept(this);
            var right = context.expression()[1].Accept(this);
            var output = "";
            // Check if both left and right are variable names

            if(left is bool b)
                left = b.ToString().ToUpper();

            if(right is bool c)
                right = c.ToString().ToUpper();


            if (left == null && right == null)
            {
                throw new NullReferenceException();
            }
            if (left != null)
            {
                if (Variables.ContainsKey(left.ToString()!))
                {
                    output += Variables[left.ToString()!];
                }
                else
                {
                    output += left.ToString();
                }
            }
            if (right != null)
            {
                if (Variables.ContainsKey(right.ToString()!))
                {
                    output += Variables[right.ToString()!];
                }
                else
                {
                    output += right.ToString();
                }
            }
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
                Console.WriteLine($"Variable '{identifier}' is not defined!");
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
            return op.Unary(context.unary_operator().GetText(), Visit(context.expression()));
        }

        public override object? VisitAdditiveExpression([NotNull] AdditiveExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var ops = context.add_operator().GetText();

            return ops switch
            {
                "+" => op.Add(left, right),
                "-" => op.Subtract(left, right),
                _ => throw new NotImplementedException(),
            }; ;
        }

        public override object? VisitMultiplicativeExpression([NotNull] MultiplicativeExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var ops = context.multiply_operator().GetText();

            return ops switch
            {
                "*" => op.Multiply(left, right),
                "/" => op.Divide(left, right),
                "%" => op.Modulo(left, right),
                _ => throw new NotImplementedException(),
            };
        }

        public override object? VisitRelationalExpression([NotNull] RelationalExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var ops = context.compare_operator().GetText();

            var result = op.Relational(left, right, ops);

            if (result?.GetType() == typeof(bool))
            {
                return result.ToString()?.ToUpper();
            }
           
            return null;
        }

        public override object? VisitParenthesisExpression([NotNull] ParenthesisExpressionContext context)
        {
            return Visit(context.expression());
        }

        public override object? VisitNOTExpression([NotNull] NOTExpressionContext context)
        {
            var expressionValue = Visit(context.expression());

            var negatedValue = op.Negation(expressionValue);

            if (negatedValue != null && negatedValue is bool boolValue)
            {
                return boolValue.ToString()?.ToUpper();
            }
            
             return null;
        }

        public override object? VisitBoolOpExpression([NotNull] BoolOpExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            var boolop = context.bool_operator().GetText();

            return op.BoolOperation(left, right, boolop);
        }

        public override object? VisitEscapeSequenceExpression([NotNull] EscapeSequenceExpressionContext context)
        {
            var sequence = context.GetText()[1];
            var result = op.Escape(sequence) ?? throw new ArgumentException($"Invalid escape sequence: {context.GetText()}");

            return result;
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

            return output.ToString();
        }

        public override object? VisitScan([NotNull] ScanContext context)
        {
            foreach (var id in context.IDENTIFIER())
            {
                //testing purposes can be removed or kept after review
                Console.WriteLine($"Awaiting input for {id.GetText()}");

                string input = Console.ReadLine() ?? "";
                if (id.GetText() != null)
                {
                   if(VarTypes[id.GetText()] == typeof(int))
                    {
                        Variables[id.GetText()] = Convert.ToInt32(input);
                    }
                   else if (VarTypes[id.GetText()] == typeof(float))
                    {
                        Variables[id.GetText()] = Convert.ToDouble(input);
                    }
                   else if (VarTypes[id.GetText()] == typeof(bool))
                    {
                        Variables[id.GetText()] = Convert.ToBoolean(input);
                    }
                   else if (VarTypes[id.GetText()] == typeof(char))
                    {
                        Variables[id.GetText()] = Convert.ToChar(input);
                    }
                   else if (VarTypes[id.GetText()] == typeof(string))
                    {
                        Variables[id.GetText()] = Convert.ToString(input);
                    }
                    else
                    {
                        throw new Exception("Data type does not exist!");
                    }
                }
            }
            return null;
        }
    }
}