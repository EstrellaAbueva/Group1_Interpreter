using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.Methods;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Group1_InterpreterConsole.Contents.CodeParser;

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
            /*var varName = context.IDENTIFIER().GetText();
            var varType = VisitType(context.type());
            var value = VisitExpression(context.expression());

            // If the assignment only has one identifier, update its value directly
            if (identifiers.Count == 1)
            {
                var varName = identifiers[0];
                return Variables[varName] = value;
            }

            // If the assignment has multiple identifiers, update their values in a chain
            for (int j = 0; j < identifiers.Count(); j++)
            {
                var varName = identifiers[j];

                if (j == 0)
                {
                    // For the first identifier, assign the value directly
                    Variables[varName] = value;
                }
                else
                {
                    // For subsequent identifiers, assign the value of the previous identifier
                    Variables[varName] = Variables[identifiers[j - 1]];
                }
            }

            return Variables[varName] = valueWithType;*/
            var identifier = context.IDENTIFIER();
            foreach(var i in identifier)
            {
                var expression = context.expression().Accept(this);
                Variables[i.GetText()] = expression;
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
            var exp = Visit(context.expression());

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
            }
            else
            {
                throw new Exception("Unknown statement type");
            }
        }

        public override List<object?> VisitDeclaration([NotNull] CodeParser.DeclarationContext context)
        {
            var type = context.type().GetText();
            var varnames = context.IDENTIFIER();      

            // remove type
            var contextstring = context.GetText().Replace(type, "");

            var contextParts = contextstring.Split(',');
            var exp = context.expression();
            int expctr = 0;
            //Dictionary<string, object?> expList = new Dictionary<string, object?>();

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
                    if ( expctr < exp.Count())
                    {
                        Variables[varnames[x].GetText()] = Visit(exp[expctr]);
                        expctr++;
                    }
                }
                else
                {
                    Variables[varnames[x].GetText()] = null;
                }
                
            }

            /*for (int j = 0; j < values.Count(); j++)
            {
                var value = values[j].GetText();
                    
                if (j >= varnames.Count())
                {
                    Console.WriteLine($"Too many values specified for variable '{string.Join(",", varnames)}'");
                    break;
                }

                var identifier = varnames[j];

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
                    else if (type.Equals("BOOL"))
                    {
                        if (bool.TryParse(value.ToString().Trim('"'), out bool boolValue))
                            Variables[identifier] = boolValue;
                        else
                            Console.WriteLine($"Invalid value for boolean variable '{identifier}'");
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
            }*/

            return new List<object?>();
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
            if(left == null && right == null)
            {
                throw new NullReferenceException();
            }
            if (left != null) {
                if (Variables.ContainsKey(left.ToString()!))
                {
                    output += Variables[left.ToString()!];
                }
                else
                {
                    output += left.ToString();
                }
            }    
            if(right != null)
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
            if(context.constant().INT() is { } i)
                return int.Parse(i.GetText());
            
            if (context.constant().FLOAT() is { } f)
                return float.Parse(f.GetText());
            
            if (context.constant().CHAR() is { } g)
                return char.Parse(g.GetText());

            if (context.constant().STRING() is { } s)
                return s.GetText()[1..^1];

            if (context.constant().BOOL() is { } b)
                return b.GetText() == "True";

            throw new NotImplementedException();
        }

        public override object? VisitVariable_assignment([NotNull] CodeParser.Variable_assignmentContext context)
        {
            var type = context.type().GetText();
            var name = context.IDENTIFIER().GetText();

            return Variables[name] = null;
        }

    }
}
