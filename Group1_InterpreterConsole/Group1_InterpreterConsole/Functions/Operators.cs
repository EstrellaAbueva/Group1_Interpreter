using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Group1_InterpreterConsole.Functions
{
    public class Operators
    {
        public object? Unary(string symbol, object? value)
        {
            if (symbol == "+")
                return value;

            if (symbol == "-")
            {
                if (value is int i)
                    return -i;
                if (value is float f)
                    return -f;
            }

            throw new Exception($"Cannot get unary value for symbol {symbol}");
        }


        public object? Add(object? left, object? right)
        {
            if (left is int i && right is int j)
                return i + j;

            if (left is float f && right is float g)
                return f + g;

            if (left is int lInt && right is float rFloat)
                return lInt + rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat + rInt;

            //concatenate
            if (left is string || right is string)
                return $"{left}{right}";

            throw new Exception($"Cannot add values of types {left?.GetType()} and {right?.GetType()}");
        }

        public object? Subtract(object? left, object? right)
        {
            if (left is int i && right is int j)
                return i - j;

            if (left is float f && right is float g)
                return f - g;

            if (left is int lInt && right is float rFloat)
                return lInt - rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat - rInt;

            throw new Exception($"Cannot subtract values of types {left?.GetType()} and {right?.GetType()}");
        }

        public object? Multiply(object? left, object? right)
        {
            if (left is int i && right is int j)
                return i * j;

            if (left is float f && right is float g)
                return f * g;

            if (left is int lInt && right is float rFloat)
                return lInt * rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat * rInt;

            throw new Exception($"Cannot multiply values of types {left?.GetType()} and {right?.GetType()}");
        }

        public object? Divide(object? left, object? right)
        {
            if (left is int i && right is int j)
                return i / j;

            if (left is float f && right is float g)
                return f / g;

            if (left is int lInt && right is float rFloat)
                return lInt / rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat / rInt;

            throw new Exception($"Cannot divide values of types {left?.GetType()} and {right?.GetType()}");
        }

        public object? Modulo(object? left, object? right)
        {
            if (left is int i && right is int j)
                return i % j;

            if (left is float f && right is float g)
                return f % g;

            if (left is int lInt && right is float rFloat)
                return lInt % rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat % rInt;

            throw new Exception($"Cannot get modulo for the values of types {left?.GetType()} and {right?.GetType()}");
        }

        public object? Relational(object? left, object? right, string op)
        {
            if (op == ">")
            {
                if (left is int leftInt && right is int rightInt)
                {
                    return leftInt > rightInt;
                }
                else if (left is float leftFloat && right is float rightFloat)
                {
                    return leftFloat > rightFloat;
                }
                else
                {
                    throw new Exception($"Cannot compare values of types {left?.GetType()} and {right?.GetType()} with '>' operator");
                }
            }
            else if (op == "<")
            {
                if (left is int leftInt && right is int rightInt)
                {
                    return leftInt < rightInt;
                }
                else if (left is float leftFloat && right is float rightFloat)
                {
                    return leftFloat < rightFloat;
                }
                else
                {
                    throw new Exception($"Cannot compare values of types {left?.GetType()} and {right?.GetType()} with '<' operator");
                }
            }
            else if (op == ">=")
            {
                if (left is int leftInt && right is int rightInt)
                {
                    return leftInt >= rightInt;
                }
                else if (left is float leftFloat && right is float rightFloat)
                {
                    return leftFloat >= rightFloat;
                }
                else
                {
                    throw new Exception($"Cannot compare values of types {left?.GetType()} and {right?.GetType()} with '>=' operator");
                }
            }
            else if (op == "<=")
            {
                if (left is int leftInt && right is int rightInt)
                {
                    return leftInt <= rightInt;
                }
                else if (left is float leftFloat && right is float rightFloat)
                {
                    return leftFloat <= rightFloat;
                }
                else
                {
                    throw new Exception($"Cannot compare values of types {left?.GetType()} and {right?.GetType()} with '<=' operator");
                }
            }
            else if (op == "==")
            {
                return left?.Equals(right);
            }
            else if (op == "<>")
            {
                return !left?.Equals(right);
            }
            else
            {
                throw new Exception($"Invalid operator: {op}");
            }
        }

        public object? Negation(object? op)
        {
            var not = Convert.ToBoolean(op);

            if (not is bool boolValue)
            {
                return !boolValue;
            }
            else
            {
                throw new ArgumentException("Argument must be a boolean value.");
            }
        }

        public object? BoolOperation(object? left, object? right, string boolop)
        {
            switch (boolop)
            {
                case "AND":
                    return (Convert.ToBoolean(left) && Convert.ToBoolean(right));
                case "OR":
                    return (Convert.ToBoolean(left) || Convert.ToBoolean(right));
                default:
                    throw new Exception("Invalid boolean operator: " + boolop);
            }
        }

        public object? Escape(object? sequence)
        {
            if(sequence != null)
            {
                sequence = Convert.ToChar(sequence);
                return $"{sequence}";
            }
            else
            {
                throw new ArgumentException($"Invalid escape sequence: {sequence}");
            }
        }
    }
}
