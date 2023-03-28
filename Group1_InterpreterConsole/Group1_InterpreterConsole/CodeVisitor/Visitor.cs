using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.Methods;
using System.Runtime.CompilerServices;

namespace Group1_InterpreterConsole.CodeVisitor
{
    public class Visitor : CodeBaseVisitor<object?>
    {
        private Dictionary<string, object?> _variables { get; set; } = new Dictionary<string, object?>();
        private readonly ErrorHandling errorHandling = new ErrorHandling();

        public override object? VisitProgram([NotNull] CodeParser.ProgramContext context)
        {
            ErrorHandling.ErrorProgram(context);
            string code = context.GetText().Trim();
            if (code.StartsWith("BEGIN CODE") && code.EndsWith("END CODE"))
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
                return context.STRING().GetText();

            if (context.BOOL() is { } b)
                return b.GetText() == "true";

            if (context.CHAR() is { } c)
                return char.Parse(c.GetText());

            throw new NotImplementedException();
        }

        public override object? VisitDisplay([NotNull] CodeParser.DisplayContext context)
        {
            foreach (var variable in _variables)
            {
               Console.WriteLine("{0}",variable.Value);
               break;
            }

            Console.WriteLine();

            return null;
        }

        public override object? VisitType([NotNull] CodeParser.TypeContext context)
        {
            return null;
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
                return VisitAssignment(context.assignment());
            }
            else if (context.display() != null)
            {
                return VisitDisplay(context.display());
            }
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
