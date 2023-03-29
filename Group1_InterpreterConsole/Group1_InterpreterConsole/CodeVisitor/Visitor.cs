﻿using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.Methods;
using System.Text.RegularExpressions;

namespace Group1_InterpreterConsole.CodeVisitor
{
    public class Visitor : CodeBaseVisitor<object?>
    {
        private Dictionary<string, object?> Variables { get; set; } = new Dictionary<string, object?>();

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
            var varName = context.IDENTIFIER().GetText();
            var value = VisitExpression(context.expression());

            return Variables[varName] = value;
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
            else
            {
                throw new InvalidOperationException("Unknown literal type");
            }
        }


        public override object? VisitDisplay([NotNull] CodeParser.DisplayContext context)
        {
            foreach (var variable in Variables)
            {
               Console.Write(variable.Value + " ");
            }

            Console.WriteLine();

            return null;
        }

        public override object? VisitType([NotNull] CodeParser.TypeContext context)
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
                    throw new NotImplementedException();
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
            //else if (context.declaration() != null)
            //{
            //    return VisitDeclaration(context.declaration());
            //}
            //else if (context.conditional() != null)
            //{
            //    return VisitConditional(context.conditional());
            //}
            //else if (context.loop() != null)
            //{
            //    return VisitLoop(context.loop());
            //}
            //else if (context.function_call() != null)
            //{
            //    return VisitFunction_call(context.function_call());
            //}
            //else if (context.function_declaration() != null)
            //{
            //    return VisitFunction_declaration(context.function_declaration());
            //}
            else
            {
                throw new Exception("Unknown statement type");
            }
        }


        public override object? VisitExpression([NotNull] CodeParser.ExpressionContext context)
        {
            if (context.constant() != null)
            {
                return VisitConstant(context.constant());
            }
            else if (context.IDENTIFIER() != null)
            {
                var varName = context.IDENTIFIER().GetText();
                if (Variables != null && Variables.ContainsKey(varName))
                {
                    return Variables[varName];
                }
                else
                {
                    throw new Exception($"Variable {varName} not found");
                }
            }

            return null;
        }

        public override object? VisitDeclaration([NotNull] CodeParser.DeclarationContext context)
        {
            var type = context.type().GetText();
            var varName = context.IDENTIFIER().Select(x => x.GetText()).ToArray();

            var varValue = context.expression();

            for(int i = 0; i < varName.Length; i++)
            {
                if (Variables.ContainsKey(varName[i]))
                {
                    Console.WriteLine($"Variable '{varName}' is already defined!");
                }
                else
                {
                    if (type.Equals("INT"))
                    {
                        if (int.TryParse(varValue.ToString(), out int intValue))
                        {
                            Variables[varName[i]] = intValue;
                        }
                        else
                        {
                            int value;
                            bool success = int.TryParse(varValue.ToString(), out value);
                            if (!success)
                            {
                                Console.WriteLine($"Invalid value for integer variable '{varName}'");
                            }
                        }
                    }
                    else if (type.Equals("FLOAT"))
                    {
                        if (float.TryParse(varValue.ToString(), out float floatValue))
                            return Variables[varName[i]] = floatValue;
                        else
                            Console.WriteLine($"Invalid value for float variable '{varName}'");
                    }
                    else if (type.Equals("BOOL"))
                    {
                        if (bool.TryParse(varValue.ToString(), out bool boolValue))
                            return Variables[varName[i]] = boolValue;
                        else
                            Console.WriteLine($"Invalid value for boolean variable '{varName}'");
                    }
                    else if (type.Equals("CHAR"))
                    {
                        var charValue = varValue.ToString();
                        if (charValue?.Length == 3 && charValue[i] == '\'' && charValue[2] == '\'')
                            return Variables[varName[i]] = charValue[1];
                        else
                            Console.WriteLine($"Invalid value for character variable '{varName}'");
                    }
                    else if (type.Equals("STRING"))
                    {
                        return Variables[varName[i]] = varValue.ToString();
                    }
                    else
                    {
                        Console.WriteLine($"Invalid variable type '{type}'");
                    }
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

    }
}
