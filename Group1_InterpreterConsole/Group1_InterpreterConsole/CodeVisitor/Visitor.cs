using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;

namespace Group1_InterpreterConsole.CodeVisitor
{
    public class Visitor : CodeBaseVisitor<object?>
    {
        private Dictionary<string, object?> _variables { get; } = new ();
        
        public override object? VisitProgram(CodeParser.ProgramContext context)
        {
            //check if the program has BEGIN CODE
            if (context.GetText() is null)
            {
                throw new Exception("Program must start with BEGIN");
            }

            //check if the program has END CODE
            if (context.GetText() is null)
            {
                throw new Exception("Program must end with END");
            }

            return null;
        }

        public override object? VisitAssignment(CodeParser.AssignmentContext context)
        {
            var varName = context.IDENTIFIER()[0].GetText();

            var value = Visit(context.expression());

            _variables[varName] = value;
            return null;
        }

        public override object? VisitVariable(CodeParser.VariableContext context)
        {
            var varName = context.IDENTIFIER().GetText();
            if (_variables.ContainsKey(varName))
            {
                return _variables[varName];
            }
            else
            {
                throw new Exception($"Variable {varName} not found");
            }
        }

        public override object? VisitConstant(CodeParser.ConstantContext context)
        {
            if (context.INT() is { } i)
                return int.Parse(i.GetText());

            if (context.FLOAT() is { } f)
                return float.Parse(f.GetText());

            if (context.STRING() is { } s)
                return s.GetText();

            if (context.BOOL() is { } b)
                return b.GetText() == "true";

            if (context.CHAR() is { } c)
                return char.Parse(c.GetText());

            throw new NotImplementedException();
        }

        public override object? VisitDeclaration(CodeParser.DeclarationContext context)
        {
            var type = context.type().GetText();
            var vars = context.variable();

            foreach (var v in vars)
            {
                var varName = v.IDENTIFIER().GetText();
                var varValue = v.expression() == null ? null : Visit(v.expression());

                if (_variables.ContainsKey(varName))
                {
                    Console.WriteLine($"Variable '{varName}' is already defined!");
                }
                else
                {
                    switch (type)
                    {
                        case "INT":
                            if (int.TryParse(varValue?.ToString(), out int intValue))
                                _variables[varName] = intValue;
                            else
                                Console.WriteLine($"Invalid value for integer variable '{varName}'");
                            break;
                        case "FLOAT":
                            if (float.TryParse(varValue?.ToString(), out float floatValue))
                                _variables[varName] = floatValue;
                            else
                                Console.WriteLine($"Invalid value for float variable '{varName}'");
                            break;
                        case "BOOL":
                            if (bool.TryParse(varValue?.ToString(), out bool boolValue))
                                _variables[varName] = boolValue;
                            else
                                Console.WriteLine($"Invalid value for boolean variable '{varName}'");
                            break;
                        case "CHAR":
                            var charValue = varValue?.ToString();
                            if (charValue?.Length == 3 && charValue[0] == '\'' && charValue[2] == '\'')
                                _variables[varName] = charValue[1];
                            else
                                Console.WriteLine($"Invalid value for character variable '{varName}'");
                            break;
                        case "STRING":
                            _variables[varName] = varValue?.ToString();
                            break;
                        default:
                            Console.WriteLine($"Invalid variable type '{type}'");
                            break;
                    }
                }
            }

            return null;
        }

        public override object? VisitDisplay(CodeParser.DisplayContext context)
        {
            var value = Visit(context.expression());

            if (value != null)
            {
                Console.WriteLine(value.ToString());
            }
            else
            {
                Console.WriteLine("null");
            }

            return null;
        }

        public override object? VisitComparison(CodeParser.ComparisonContext context)
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

        public override object? VisitIf_statement(CodeParser.If_statementContext context)
        {
            //bool condition = (bool)Visit(context.comparison());
            //if (condition)
            //{
            //    Visit(context.if_block().executable_code());
            //}
            //else
            //{
            //    foreach (var elseIfBlock in context.else_if_block())
            //    {
            //        condition = (bool)Visit(elseIfBlock.comparison());
            //        if (condition)
            //        {
            //            Visit(elseIfBlock.executable_code());
            //            return null;
            //        }
            //    }
            //    if (context.else_block() != null)
            //    {
            //        Visit(context.else_block().executable_code());
            //    }
            //}
            return null;
        }

    }
}
