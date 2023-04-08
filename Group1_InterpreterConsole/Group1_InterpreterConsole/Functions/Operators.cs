using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.ErrorHandling;
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
        public static object? Unary(string symbol, object? value)
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


        public static object? Add([NotNull] ParserRuleContext context, object? left, object? right)
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

            ErrorHandler.HandleInvalidOperatorError(context, left, right, "add");
            return null;
            /*throw new Exception($"Cannot add values of types {left?.GetType().Name.ToUpper()} and {right?.GetType().Name.ToUpper()}");*/
        }

        public static object? Subtract([NotNull] ParserRuleContext context, object? left, object? right)
        {
            if (left is int i && right is int j)
                return i - j;

            if (left is float f && right is float g)
                return f - g;

            if (left is int lInt && right is float rFloat)
                return lInt - rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat - rInt;

            ErrorHandler.HandleInvalidOperatorError(context, left, right, "subtract");
            return null;
            //throw new Exception($"Cannot subtract values of types {left?.GetType()} and {right?.GetType()}");
        }

        public static object? Multiply([NotNull] ParserRuleContext context, object? left, object? right)
        {
            if (left is int i && right is int j)
                return i * j;

            if (left is float f && right is float g)
                return f * g;

            if (left is int lInt && right is float rFloat)
                return lInt * rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat * rInt;

            ErrorHandler.HandleInvalidOperatorError(context, left, right, "multiply");
            return null;
            //throw new Exception($"Cannot multiply values of types {left?.GetType()} and {right?.GetType()}");
        }

        public static object? Divide([NotNull] ParserRuleContext context, object? left, object? right)
        {
            if (left is int i && right is int j)
                return i / j;

            if (left is float f && right is float g)
                return f / g;

            if (left is int lInt && right is float rFloat)
                return lInt / rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat / rInt;

            ErrorHandler.HandleInvalidOperatorError(context, left, right, "divide");
            return null;
            //throw new Exception($"Cannot divide values of types {left?.GetType()} and {right?.GetType()}");
        }

        public static object? Modulo([NotNull] ParserRuleContext context, object? left, object? right)
        {
            if (left is int i && right is int j)
                return i % j;

            if (left is float f && right is float g)
                return f % g;

            if (left is int lInt && right is float rFloat)
                return lInt % rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat % rInt;

            ErrorHandler.HandleInvalidOperatorError(context, left, right, "get modulo for the");
            return null;
            //throw new Exception($"Cannot get modulo for the values of types {left?.GetType()} and {right?.GetType()}");
        }

        public static object? Relational([NotNull] ParserRuleContext context, object? left, object? right, string op)
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
                    ErrorHandler.HandleInvalidRelationOperatorError(context, left, right, op);
                    return null;
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
                    ErrorHandler.HandleInvalidRelationOperatorError(context, left, right, op);
                    return null;
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
                    ErrorHandler.HandleInvalidRelationOperatorError(context, left, right, op);
                    return null;
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
                    ErrorHandler.HandleInvalidRelationOperatorError(context, left, right, op);
                    return null;
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

        public static object? Negation(object? op)
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

        public static object? BoolOperation(object? left, object? right, string boolop)
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

        public static object? Escape(object? sequence)
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
