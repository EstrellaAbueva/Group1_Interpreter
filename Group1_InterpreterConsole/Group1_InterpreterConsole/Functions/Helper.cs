using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.ErrorHandling;
using System.Text.RegularExpressions;

namespace Group1_InterpreterConsole.Functions
{
    public class Helper
    {
        /// <summary>
        /// Used for the code Visitor VisitVariable, where it checks if the given variable is present in the Dictionary.
        /// If the given varName is present then it would return the value.
        /// </summary>
        /// <param name="variables">Dictionary containing all the elements.</param>
        /// <param name="varName">The name that would be checked in the dictionary.</param>
        /// <param name="context">VariableContext that would be of use for the ErrorHandler.</param>
        /// <returns>Object Data Type</returns>
        public static object? Variables(Dictionary<string, object?> variables, string varName, CodeParser.VariableContext context)
        {
            if (variables != null && variables.TryGetValue(varName, out object? value))
            {
                return value;
            }
            else
            {
                ErrorHandler.HandleUndefinedVariableError(context, varName);
                return null;
            }
        }

        /// <summary>
        /// Function that would help in Parsing the provided string.
        /// </summary>
        /// <param name="constant">The string to be converted in to a certain data type.</param>
        /// <param name="context">ConstantContext data type that would be needed to access base on the Code.g4 to get the data type.</param>
        /// <returns>A parsed data type of the parameter constant.</returns>
        /// <exception cref="InvalidOperationException">Throws this exception if the data type is Invalid.</exception>
        public static object? ConstantParser(string constant, CodeParser.ConstantContext context)
        {
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

        /// <summary>
        /// Displays the expression in the Console.
        /// 1. If the data type is BOOL it should be in uppercase
        /// 2. If other data types it would display automatically.
        /// </summary>
        /// <param name="expression">The string that needs to be displayed.</param>
        /// <returns></returns>
        public static object? Display(object? expression)
        {
            if (expression is bool b)
                expression = b.ToString().ToUpper();

            Console.Write(expression);

            return null;

        }

        /// <summary>
        /// Parse the context to a specific data type.
        /// </summary>
        /// <param name="context">TypeContext that would be of use in converting it to a specific data type.</param>
        /// <returns>The typeof depending on the context.</returns>
        /// <exception cref="NotImplementedException">Throws this exception if the data type is Invalid.</exception>
        public static object? TypeParser(CodeParser.TypeContext context)
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

        /// <summary>
        /// Displays a concatenated version of the left and right.
        /// </summary>
        /// <param name="left">The left object you want to concatenate.</param>
        /// <param name="right">The right object you want to concatenate.</param>
        /// <returns></returns>
        public static object? Concat(object? left, object? right)
        {
            if (left is bool b)
                left = b.ToString().ToUpper();

            if (right is bool c)
                right = c.ToString().ToUpper();

            return $"{left}{right}";
        }

        public static object? Identifier(Dictionary<string, object?> dictionary, string identifier, CodeParser.IdentifierExpressionContext context)
        {
            if (dictionary.ContainsKey(identifier))
            {
                return dictionary[identifier];
            }
            else
            {
                ErrorHandler.HandleUndefinedVariableError(context, identifier);
                return null;
            }
        }

        public static object? ConstantExpressionParser(CodeParser.ConstantExpressionContext context)
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

        public static object? Increment([NotNull] ParserRuleContext context, Dictionary<string, object?> dictionary, Dictionary<string, object?> dictionarys, string id)
        {
            if (dictionary.ContainsKey(id))
            {
                if ((Type?)dictionary[id] == typeof(int))
                {
                    return dictionarys[id] = Convert.ToInt32(dictionarys[id]) + 1;
                }
                else if ((Type?)dictionary[id] == typeof(float))
                {
                    return dictionarys[id] = Convert.ToDouble(dictionarys[id]) + 1;
                }
                else
                {
                    return ErrorHandler.HandleInvalidIncrementTypeError(context, dictionary, id);
                }
            }
            
            return ErrorHandler.HandleUndeclaredVariableError(context, dictionary, id);
        }

        public static object? Decrement([NotNull] ParserRuleContext context, Dictionary<string, object?> dictionary, Dictionary<string, object?> dictionarys, string id)
        {
            if (dictionary.ContainsKey(id))
            {
                if ((Type?)dictionary[id] == typeof(int))
                {
                    return dictionarys[id] = Convert.ToInt32(dictionarys[id]) - 1;
                }
                else if ((Type?)dictionary[id] == typeof(float))
                {
                    return dictionarys[id] = Convert.ToDouble(dictionarys[id]) - 1;
                }
                else
                {
                    return ErrorHandler.HandleInvalidDecrementTypeError(context, dictionary, id);
                }
            }
            return ErrorHandler.HandleUndeclaredVariableError(context, dictionary, id); 
        }

        public static object? Scan([NotNull] ParserRuleContext context, Dictionary<string, object?> typeDictionary, Dictionary<string, object?> valueDictionary, string id, string input)
        {
            if (!typeDictionary.ContainsKey(id))
            {
                return ErrorHandler.HandleUndeclaredVariableError(context, typeDictionary, id);
            }

            Type? valueType = (Type?)typeDictionary[id];
            try
            {
                object? convertedValue = Convert.ChangeType(input, valueType!);
                return valueDictionary[id] = convertedValue;
            }
            catch (FormatException)
            {
                return ErrorHandler.HandleInvalidScanTypeError(context,input,valueType,"Input Scan");
                //throw new ArgumentException($"Input '{input}' is not in the expected format for data type {valueType}.");
            }
        }
    }
}