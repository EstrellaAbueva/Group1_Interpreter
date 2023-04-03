using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group1_InterpreterConsole.Functions
{
    public class Operators
    {
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
    }
}
