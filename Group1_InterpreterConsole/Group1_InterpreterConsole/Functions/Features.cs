using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.ErrorHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Group1_InterpreterConsole.Functions
{
    public class Features
    {
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

        public static object? Display(object? expression)
        {
            if (expression is bool b)
                expression = b.ToString().ToUpper();

            Console.Write(expression);

            return null;

        }

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