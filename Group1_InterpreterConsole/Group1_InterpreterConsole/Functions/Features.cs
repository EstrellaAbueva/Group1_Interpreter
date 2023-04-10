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
        public static object? Increment(Dictionary<string, object?> dictionary, Dictionary<string, object?> dictionarys, string id)
        {
            if (dictionary.ContainsKey(id))
            {
                if (dictionary[id] == typeof(int))
                {
                    return dictionarys[id] = Convert.ToInt32(dictionarys[id]) + 1;
                }
                else if (dictionary[id] == typeof(float))
                {
                    return dictionarys[id] = Convert.ToDouble(dictionarys[id]) + 1;
                }
                else
                {
                    throw new ArgumentException($"Cannot increment value of type {dictionary[id]?.GetType().Name}");
                }
            }
            else
            {
                throw new ArgumentException($"Variable {id} does not exist");
            }
        }

        public static object? Decrement(Dictionary<string, object?> dictionary, Dictionary<string, object?> dictionarys, string id)
        {
            if (dictionary.ContainsKey(id))
            {
                if (dictionary[id] == typeof(int))
                {
                    return dictionarys[id] = Convert.ToInt32(dictionarys[id]) - 1;
                }
                else if (dictionary[id] == typeof(float))
                {
                    return dictionarys[id] = Convert.ToDouble(dictionarys[id]) - 1;
                }
                else
                {
                    throw new ArgumentException($"Cannot increment value of type {dictionary[id]?.GetType().Name}");
                }
            }
            else
            {
                throw new ArgumentException($"Variable {id} does not exist");
            }
        }

        public static void Scan(Dictionary<string, object?> typeDictionary, Dictionary<string, object?> valueDictionary, string id, string input)
        {
            if (!typeDictionary.ContainsKey(id))
            {
                throw new ArgumentException($"Variable '{id}' is not declared.");
            }

            Type? valueType = (Type?)typeDictionary[id];
            try
            {
                object? convertedValue = Convert.ChangeType(input, valueType!);
                valueDictionary[id] = convertedValue;
            }
            catch (FormatException)
            {
                throw new ArgumentException($"Input '{input}' is not in the expected format for data type {valueType}.");
            }
        }
    }
}
