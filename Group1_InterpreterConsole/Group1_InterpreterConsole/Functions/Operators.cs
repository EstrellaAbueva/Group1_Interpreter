using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.ErrorHandling;

namespace Group1_InterpreterConsole.Functions
{
    public class Operators
    {
        /// <summary>
        /// Performs a unary operation on the given value based on the given symbol.
        /// </summary>
        /// <param name="context">The parser rule context in which the unary operation is being performed.</param>
        /// <param name="symbol">The symbol representing the unary operation to be performed.</param>
        /// <param name="value">The value on which the unary operation is to be performed.</param>
        /// <returns>
        /// The result of the unary operation.
        /// If the objects are not of type int or float, an error message is returned.
        /// </returns>
        public static object? Unary([NotNull] ParserRuleContext context, string symbol, object? value)
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

            return ErrorHandler.HandleUnaryError(context, symbol);
        }

        /// <summary>
        /// Adds two objects of compatible types or concatenates them if either or both are strings.
        /// </summary>
        /// <param name="context">The ParserRuleContext of the operation.</param>
        /// <param name="left">The first object to add or concatenate.</param>
        /// <param name="right">The second object to add or concatenate.</param>
        /// <returns>
        /// The sum of the two objects if both are numbers (int or float),
        /// the concatenation of the two objects if either or both are strings,
        /// or an error message if the objects are not compatible for addition.
        /// </returns>
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

            return ErrorHandler.HandleInvalidOperatorError(context, left, right, "add");
        }

        /// <summary>
        /// Computes the subtraction of two values, either integer or float, and returns the result.
        /// </summary>
        /// <param name="context">The context of the parser rule that calls this method.</param>
        /// <param name="left">The left operand of the subtraction operation.</param>
        /// <param name="right">The right operand of the subtraction operation.</param>
        /// <returns>
        /// The result of the subtraction operation.
        /// If the objects are not of type int or float, an error message is returned.
        /// </returns>
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

            return ErrorHandler.HandleInvalidOperatorError(context, left, right, "subtract");
        }

        /// <summary>
        /// Multiplies two operands of type int or float.
        /// </summary>
        /// <param name="context">The parser rule context.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        /// The product of the two operands.
        /// If the objects are not of type int or float, an error message is returned.
        /// </returns>
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

            return ErrorHandler.HandleInvalidOperatorError(context, left, right, "multiply");
        }

        /// <summary>
        /// Divides two objects of type int or float.
        /// </summary>
        /// <param name="context">The context in which the division operation occurs.</param>
        /// <param name="left">The object to be divided.</param>
        /// <param name="right">The object by which to divide.</param>
        /// <returns>
        /// The quotient of the division operation if the objects are of type int or float.
        /// If the objects are not of type int or float, an error message is returned.
        /// </returns>
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

            return ErrorHandler.HandleInvalidOperatorError(context, left, right, "divide");
        }

        /// <summary>
        /// Calculates the remainder of the division of two numbers.
        /// </summary>
        /// <param name="context">The parser rule context.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        /// The remainder of the division of the two operands.
        /// If the objects are not of type int or float, an error message is returned.
        /// </returns>
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

            return ErrorHandler.HandleInvalidOperatorError(context, left, right, "get modulo for the");
        }

        /// <summary>
        /// Determines the relationship between two given values based on the specified relational operator.
        /// </summary>
        /// <param name="context">The ParserRuleContext object containing the relational operator.</param>
        /// <param name="left">The left-hand side operand of the relational expression.</param>
        /// <param name="right">The right-hand side operand of the relational expression.</param>
        /// <param name="op">The relational operator to be applied to the operands.</param>
        /// <returns>
        /// Returns the result of the relational expression evaluated against the given operands.
        /// If the objects are not of type int or float, an error message is returned.
        /// </returns>
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
                    return ErrorHandler.HandleInvalidRelationOperatorError(context, left, right, op);
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
                    return ErrorHandler.HandleInvalidRelationOperatorError(context, left, right, op);
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
                    return ErrorHandler.HandleInvalidRelationOperatorError(context, left, right, op);
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
                    return ErrorHandler.HandleInvalidRelationOperatorError(context, left, right, op);
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
                return ErrorHandler.HandleInvalidOperatorError(context, op, "");
            }
        }

        /// <summary>
        /// Computes the negation of a given boolean value. If the input is not a boolean, an error is returned.
        /// </summary>
        /// <param name="context">The context in which the negation operation is performed.</param>
        /// <param name="op">The boolean value to negate.</param>
        /// <returns>The negation of the given boolean value, or an error if the input is not a boolean.</returns>
        public static object? Negation([NotNull] ParserRuleContext context, object? op)
        {
            var not = Convert.ToBoolean(op);

            if (op is bool boolValue)
            {
                return !boolValue;
            }
            else
            {
                return ErrorHandler.HandleNegationError(context);
            }
        }

        /// <summary>
        /// Performs a boolean operation on two object values based on the given operator.
        /// </summary>
        /// <param name="context">The parser rule context.</param>
        /// <param name="left">The left operand object value.</param>
        /// <param name="right">The right operand object value.</param>
        /// <param name="boolop">The boolean operator to apply on the operands. Supported values are "AND" and "OR".</param>
        /// <returns>
        /// Returns a nullable object that represents the result of the boolean operation. The result is null
        /// if any of the operands is null or if the boolean operator is not "AND" or "OR".
        /// </returns>
        public static object? BoolOperation([NotNull] ParserRuleContext context, object? left, object? right, string boolop)
        {
            switch (boolop)
            {
                case "AND":
                    return (Convert.ToBoolean(left) && Convert.ToBoolean(right));
                case "OR":
                    return (Convert.ToBoolean(left) || Convert.ToBoolean(right));
                default:
                    return ErrorHandler.HandleBoolOperationError(context, boolop);
            }
        }

        /// <summary>
        /// Escapes the given object <paramref name="sequence"/> if it is not null.
        /// </summary>
        /// <param name="context">The parser rule context.</param>
        /// <param name="sequence">The object to escape.</param>
        /// <returns>The escaped object as a string, or an error message if the <paramref name="sequence"/> is null.</returns>
        public static object? Escape([NotNull] ParserRuleContext context, object? sequence)
        {
            if(sequence != null)
            {
                sequence = Convert.ToChar(sequence);
                return $"{sequence}";
            }
            else
            {
                return ErrorHandler.HandleInvalidEscapeSequenceError(context, sequence);
            }
        }
    }
}