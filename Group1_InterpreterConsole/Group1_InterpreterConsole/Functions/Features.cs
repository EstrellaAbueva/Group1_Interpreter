using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.ErrorHandling;
using System;
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

        public static object? Scan(Dictionary<string, object?> dictionary, Dictionary<string, object?> dictionarys, string id, string input)
        {
            if (dictionary[id] == typeof(int))
            {
                return dictionarys[id] = Convert.ToInt32(input);
            }
            else if (dictionary[id] == typeof(float))
            {
                return dictionarys[id] = Convert.ToDouble(input);
            }
            else if (dictionary[id] == typeof(bool))
            {
                return dictionarys[id] = Convert.ToBoolean(input);
            }
            else if (dictionary[id] == typeof(char))
            {
                return dictionarys[id] = Convert.ToChar(input);
            }
            else if (dictionary[id] == typeof(string))
            {
                return dictionarys[id] = Convert.ToString(input);
            }
            else
            {
                throw new Exception("Data type does not exist!");
            }
        }
    }
}
