using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.Methods;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml.Linq;

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
            var varType = VisitType(context.type());
            var value = VisitExpression(context.expression());

            var converter = TypeDescriptor.GetConverter((Type)varType);
            var valueWithType = converter.ConvertFrom(value?.ToString() ?? "");

            return Variables[varName] = valueWithType;
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
            if (context.BOOL() != null)
            {
                var boolValue = context.BOOL().GetText().ToLower() == "true";
                return boolValue;
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
            var displayValues = context.expression();

            foreach (var displayValue in displayValues)
            {
                var value = displayValue.GetText();

                if (value.StartsWith("\"") && value.EndsWith("\""))
                {
                    Console.Write(value.Trim('"'));
                }
                else if (Variables.ContainsKey(value))
                {
                    Console.Write(Variables[value]);
                }
                else
                {
                    Console.Write(value);
                }
            }

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
            else
            {
                throw new Exception("Unknown statement type");
            }
        }

        public override List<object?>? VisitDeclaration([NotNull] CodeParser.DeclarationContext context)
        {
            var type = context.type().GetText();
            var identifiers = context.IDENTIFIER().Select(x => x.GetText()).ToList();

            var values = context.expression();

            for (int j = 0; j < values.Count(); j++)
            {
                var value = values[j].GetText();
                    
                if (j >= identifiers.Count())
                {
                    Console.WriteLine($"Too many values specified for variable '{string.Join(",", identifiers)}'");
                    break;
                }

                var identifier = identifiers[j];

                if (Variables.ContainsKey(identifier))
                {
                    Console.WriteLine($"Variable '{identifier}' is already defined!");
                }
                else
                {
                    if (type.Equals("INT"))
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            Variables[identifier] = intValue;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid value for integer variable '{identifier}'");
                        }
                    }
                    else if (type.Equals("FLOAT"))
                    {
                        if (float.TryParse(value, out float floatValue))
                            Variables[identifier] = floatValue;
                        else
                            Console.WriteLine($"Invalid value for float variable '{identifier}'");
                    }
                    else if (bool.TryParse(value.ToString().Trim('"'), out bool boolValue))
                    {
                        var upperBoolValue = boolValue ? true : false;
                        Variables[identifier] = upperBoolValue;
                    }
                    else if (type.Equals("CHAR"))
                    {
                        var charValue = value;
                        if (charValue?.Length == 3 && charValue[0] == '\'' && charValue[2] == '\'')
                            Variables[identifier] = charValue[1];
                        else
                            Console.WriteLine($"Invalid value for character variable '{identifier}'");
                    }
                    else if (type.Equals("STRING"))
                    {
                        Variables[identifier] = value.Trim('"');
                    }
                    else
                    {
                        Console.WriteLine($"Invalid variable type '{type}'");
                    }
                }
            }

            return new List<object?>(); // add this to the end of the for loop
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
            // Get the left and right expressions
            var left = context.expression()[0].Accept(this);
            var right = context.expression()[1].Accept(this);

            // Check if both left and right contain strings and the '&' character
            if (left is string strLeft && right is string strRight && strLeft.Contains('&') && strRight.Contains('&'))
            {
                // Split the strings by the '&' character
                var partsLeft = strLeft.Split('&');
                var partsRight = strRight.Split('&');

                // Concatenate the corresponding parts
                var result = "";
                for (int i = 0; i < partsLeft.Length && i < partsRight.Length; i++)
                {
                    var partLeft = partsLeft[i].Trim();
                    var partRight = partsRight[i].Trim();

                    // Check if either part is a boolean, int, float, or char and concatenate them
                    if (bool.TryParse(partLeft, out _) || int.TryParse(partLeft, out _) || float.TryParse(partLeft, out _) || partLeft.StartsWith("'"))
                    {
                        result += partLeft;
                    }
                    else if (bool.TryParse(partRight, out _) || int.TryParse(partRight, out _) || float.TryParse(partRight, out _) || partRight.StartsWith("'"))
                    {
                        result += partRight;
                    }
                    else
                    {
                        result += partLeft + partRight;
                    }

                    // Add the '&' character if there are more parts
                    if (i < partsLeft.Length - 1 && i < partsRight.Length - 1)
                    {
                        result += "&";
                    }
                }

                return result;
            }
            else
            {
                // Throw an error if either left or right is not a string or the '&' character is missing
                throw new Exception("Cannot concatenate non-string values or missing '&' character.");
            }
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
            if(context.constant().INT() is { } i)
                return int.Parse(i.GetText());
            
            if (context.constant().FLOAT() is { } f)
                return float.Parse(f.GetText());
            
            if (context.constant().CHAR() is { } g)
                return char.Parse(g.GetText());

            if (context.constant().STRING() is { } s)
                return s.GetText()[1..^1];

            if (context.constant().BOOL() is { } b)
                return b.GetText() == "true";

            throw new NotImplementedException();
        }   
    }
}
