using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.Methods;

namespace Group1_InterpreterConsole.CodeVisitor
{
    public class Visitor : CodeBaseVisitor<object?>
    {
        private Dictionary<string, object?> _variables { get; set; } = new Dictionary<string, object?>();

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
            var value = Visit(context.expression());

            return _variables[varName] = value;
        }

        public override object? VisitVariable([NotNull] CodeParser.VariableContext context)
        {
            var varName = context.IDENTIFIER().GetText();
            return _variables?.GetValueOrDefault(varName) ?? throw new Exception($"Variable {varName} not found");
        }

        public override object? VisitConstant([NotNull] CodeParser.ConstantContext context)
        {
            var constant = context.GetText();
            //check constant 
            if (constant.StartsWith("\"") && constant.EndsWith("\""))
            {
                return constant.Substring(1, constant.Length - 2);
            }
            else if (constant.StartsWith("'") && constant.EndsWith("'"))
            {
                return constant[1];
            }
            else if (constant == "TRUE" || constant == "FALSE")
            {
                return bool.Parse(constant);
            }
            else if (int.TryParse(constant, out var intResult))
            {
                return intResult;
            }
            else if (float.TryParse(constant, out var floatResult))
            {
                return floatResult;
            }
            else if(char.TryParse(constant, out var charResult))
            {
                return charResult;
            }
            else
            {
                throw new Exception($"Unknown constant {constant}");
            }
        }

        public override object? VisitDisplay([NotNull] CodeParser.DisplayContext context)
        {
            foreach (var variable in _variables)
            {
                if (context.expression().IDENTIFIER().GetText().Equals(variable.Key))
                {
                    Console.WriteLine("{0}", variable.Value);
                    break;
                }
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
                if (_variables != null && _variables.ContainsKey(varName))
                {
                    return _variables[varName];
                }
                else
                {
                    throw new Exception($"Variable {varName} not found");
                }
            }

            return null;
        }


    }
}
