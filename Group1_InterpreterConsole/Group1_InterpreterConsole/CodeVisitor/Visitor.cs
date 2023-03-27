using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;

namespace Group1_InterpreterConsole.CodeVisitor
{
    public class Visitor : CodeBaseVisitor<object?>
    {
        private Dictionary<string, object?> _variables { get; set; } = new();

        public override object VisitProgram([NotNull] CodeParser.ProgramContext context)
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
                throw new ArgumentException("Code must start with 'BEGIN CODE' and end with 'END CODE'.");
            }
            return new object();
        }

        public override object? VisitAssignment([NotNull] CodeParser.AssignmentContext context)
        {
            var varName = context.IDENTIFIER().GetText();

            var value = Visit(context.expression());

            return _variables[varName] = value;
        }

        public override object? VisitVariable([NotNull] CodeParser.VariableContext context)
        {
            var varName = context.IDENTIFIER().GetText();
            return _variables?.GetValueOrDefault(varName) ?? throw new Exception($"Variable {varName} not found");
        }

        public override object VisitConstant([NotNull] CodeParser.ConstantContext context)
        {
            if (context.INT() is { } i)
                return int.Parse(i.GetText());

            if (context.FLOAT() is { } f)
                return float.Parse(f.GetText());

            if (context.STRING() is { } s)
                return context.STRING().GetText()[1..^1];

            if (context.BOOL() is { } b)
                return b.GetText() == "true";

            if (context.CHAR() is { } c)
                return char.Parse(c.GetText());

            throw new NotImplementedException();
        }

        public override object? VisitDeclaration([NotNull] CodeParser.DeclarationContext context)
        {
            var type = context.type().GetText();
            var vars = context.expression();

            var varName = vars.IDENTIFIER().GetText();
            var varValue = vars.expression();

            if (_variables.ContainsKey(varName))
            {
                Console.WriteLine($"Variable '{varName}' is already defined!");
            }
            else
            {
                if (type.Equals("INT"))
                {
                    if (int.TryParse(varValue.ToString(), out int intValue))
                    {
                        _variables[varName] = intValue;
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

                //else if (type.Equals("FLOAT"))
                //{
                //    if (float.TryParse(varValue.ToString(), out float floatValue))
                //        return _variables[varName] = floatValue;
                //    else
                //        Console.WriteLine($"Invalid value for float variable '{varName}'");
                //}
                //else if (type.Equals("BOOL"))
                //{
                //    if (bool.TryParse(varValue.ToString(), out bool boolValue))
                //        return _variables[varName] = boolValue;
                //    else
                //        Console.WriteLine($"Invalid value for boolean variable '{varName}'");
                //}
                //else if (type.Equals("CHAR"))
                //{
                //    var charValue = varValue.ToString();
                //    if (charValue?.Length == 3 && charValue[0] == '\'' && charValue[2] == '\'')
                //        return _variables[varName] = charValue[1];
                //    else
                //        Console.WriteLine($"Invalid value for character variable '{varName}'");
                //}
                //else if (type.Equals("STRING"))
                //{
                //    return _variables[varName] = varValue.ToString();
                //}
                else
                {
                    Console.WriteLine($"Invalid variable type '{type}'");
                }
            }

            return new object();
        }


        public override object VisitDisplay([NotNull] CodeParser.DisplayContext context)
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

            return new object();
        }

        public override object VisitComparison([NotNull] CodeParser.ComparisonContext context)
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
                Console.WriteLine("Sud");
                return VisitAssignment(context.assignment());
            }
            //else if (context.function_call() != null)
            //{
            //    return VisitFunction_call(context.function_call());
            //}
            //else if (context.if_statement() != null)
            //{
            //    return VisitIf_statement(context.if_statement());
            //}
            //else if (context.while_loop() != null)
            //{
            //    return VisitWhile_loop(context.while_loop());
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
                if (_variables != null && _variables.ContainsKey(varName))
                {
                    return _variables[varName];
                }
                else
                {
                    throw new Exception($"Variable {varName} not found");
                }
            }

            return new object();
        }


    }
}
