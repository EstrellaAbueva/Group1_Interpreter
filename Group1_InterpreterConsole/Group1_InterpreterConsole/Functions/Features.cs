using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.ErrorHandling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Group1_InterpreterConsole.Functions
{
    public class Features
    {
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
            else
            {
                return ErrorHandler.HandleUndeclaredVariableError(context, dictionary, id);
            }
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
            else
            {
                return ErrorHandler.HandleUndeclaredVariableError(context, dictionary, id); 
            }
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
