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

        public static object? Scan(Dictionary<string, object?> dictionarys, string id, string input)
        {
            if (int.TryParse(input, out int intValue))
            {
                dictionarys[id] = intValue;
            }
            else if (float.TryParse(input, out float floatValue))
            {
                dictionarys[id] = floatValue;
            }
            else if (char.TryParse(input, out char charValue))
            {
                dictionarys[id] = charValue;
            }
            else if (input != null)
            {
                dictionarys[id] = input;
            }
            else
            {
                throw new ArgumentException($"Invalid input for variable {id}");
            }
            return null;
        }
    }
}
