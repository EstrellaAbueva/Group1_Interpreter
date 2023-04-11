using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using System;
using System.Xml.Linq;

namespace Group1_InterpreterConsole.ErrorHandling
{
    public class ErrorHandler
    {
        /// <summary>
        /// Handles an error condition where a value cannot be converted to a boolean.
        /// </summary>
        /// <param name="context">The ParserRuleContext where the error occurred.</param>
        /// <param name="value">The value that could not be converted to a boolean.</param>
        /// <returns>Returns a nullable boolean value indicating the result of the conversion, or null if an error occurred.</returns>
        public static bool? HandleConditionError([NotNull] ParserRuleContext context, object? value)
        {
            if(value is bool b)
            {
                return b;
            }
            else
            {
                var line = context.Start.Line;
                Console.WriteLine($"Semantic Error: in line {line}.");
                Console.WriteLine($"Cannot convert {value} to boolean.");
                Environment.Exit(400);
                return false;
            }
        }

        /// <summary>
        /// Handles the type error in the given ParserRuleContext by comparing the type of the provided object with the given Type parameter.
        /// If the object's type is int, float, bool, char, or string and matches the given Type, returns true.
        /// If not, prints a semantic error message to the console indicating the mismatched types and exits the application with exit code 400.
        /// </summary>
        /// <param name="context">The ParserRuleContext where the type error occurred.</param>
        /// <param name="obj">The object whose type is being checked.</param>
        /// <param name="type">The Type to compare the object's type against.</param>
        /// <param name="location">The location in the code where the error occurred.</param>
        /// <returns>True if the object's type matches the given Type, false otherwise.</returns>
        public static bool HandleTypeError([NotNull] ParserRuleContext context, object? obj, Type? type, string location)
        {
            if (obj is int || obj is float || obj is bool || obj is char || obj is string)
            {
                if (obj.GetType() == type)
                {
                    return true;
                }
                else
                {
                    var line = context.Start.Line;
                    Console.WriteLine($"Semantic Error: in {location}, line {line}.");
                    Console.WriteLine($"Cannot convert {obj?.GetType().Name.ToUpper()} to {type?.Name.ToUpper()}.");
                    Environment.Exit(400);
                    return false;
                }
            }
            else
            {
                var line = context.Start.Line;
                Console.WriteLine($"Semantic Error: in {location}, line {line}.");
                Console.WriteLine($"Cannot convert {obj?.GetType().Name.ToUpper()} to {type?.Name.ToUpper()}.");
                Environment.Exit(400);
                return false;
            }
        }

        /// <summary>
        /// Handles the case where a variable is used but not defined, printing an error message to the console
        /// and exiting the application with a status code of 400.
        /// </summary>
        /// <param name="context">The parser context where the error occurred.</param>
        /// <param name="variableName">The name of the undefined variable.</param>
        public static void HandleUndefinedVariableError([NotNull] ParserRuleContext context, object? variableName)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in line {line}.\n" +
                              $"Variable '{variableName}' is not defined.");
            Environment.Exit(400);
        }

        /// <summary>
        /// Determines if the provided key identifier exists in the given dictionary,
        /// which represents a symbol table of declared variables. If the key identifier
        /// is not found, a semantic error message is printed to the console and the
        /// program terminates with exit code 400.
        /// </summary>
        /// <param name="context">The parser rule context that triggered this error check.</param>
        /// <param name="dictionary">The symbol table of declared variables.</param>
        /// <param name="keyId">The identifier of the variable to look for in the symbol table.</param>
        /// <returns>
        /// True if the key identifier is found in the symbol table, indicating that the
        /// variable has been declared; otherwise, false.
        /// </returns>
        public static bool HandleUndeclaredVariableError([NotNull] ParserRuleContext context, Dictionary<string, object?> dictionary, string keyId)
        {
            if (dictionary.ContainsKey(keyId))
            {
                return true;
            }
            else
            {
                var line = context.Start.Line;
                Console.WriteLine($"Semantic Error: in line {line}.\n" +
                                  $"Variable '{keyId}' has not been declared.");
                Environment.Exit(400);
                return false;
            }
        }

        /// <summary>
        /// Handles invalid scan type error by printing the error message with the location and input information, and exits the program with code 400.
        /// </summary>
        /// <param name="context">The context where the error occurred.</param>
        /// <param name="input">The input that caused the error.</param>
        /// <param name="type">The data type that the input was expected to be.</param>
        /// <param name="location">The location where the error occurred.</param>
        /// <returns>Returns null.</returns>
        public static object? HandleInvalidScanTypeError([NotNull] ParserRuleContext context, string input, Type? type, string location)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in {location}, in line {line}.\n" +
                              $"Input '{input}' is not in the expected format for data type {type?.Name.ToUpper()}.");
            Environment.Exit(400);
            return null;
        }

        /// <summary>
        /// Handles invalid scan inputs error by printing an error message containing location and line number,
        /// indicating the expected number of inputs and the actual number of inputs, and exits the environment.
        /// </summary>
        /// <param name="context">The ParserRuleContext where the error occurred.</param>
        /// <param name="length">The expected number of inputs.</param>
        /// <param name="inputs">The actual number of inputs.</param>
        /// <param name="location">The location where the error occurred.</param>
        /// <returns>Returns null after printing the error message and exiting the environment.</returns>
        public static object? HandlleInvalidScanInputsError([NotNull] ParserRuleContext context,int length, int inputs, string location)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in {location}, in line {line}.\n" +
                              $"Invalid number of inputs. Expected between 1 and {length}, but got {inputs}.");
            Environment.Exit(400);
            return null;
        }

        /// <summary>
        /// Handles the invalid operator error during semantic analysis.
        /// </summary>
        /// <param name="context">The parser rule context where the error occurred.</param>
        /// <param name="left">The left operand of the operator.</param>
        /// <param name="right">The right operand of the operator.</param>
        /// <param name="op">The invalid operator that caused the error.</param>
        /// <returns>Returns null after printing the error message and exiting the environment.</returns>
        public static object? HandleInvalidOperatorError([NotNull] ParserRuleContext context, object? left, object? right, string op)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in line {line}.\n" +
                              $"Cannot {op} values of types {left?.GetType().Name.ToUpper()} and {right?.GetType().Name.ToUpper()}");
            Environment.Exit(400);
            return null;
        }

        /// <summary>
        /// Handles invalid relation operator errors during semantic analysis.
        /// </summary>
        /// <param name="context">The ParserRuleContext object that contains the invalid operator.</param>
        /// <param name="left">The object representing the left-hand side of the invalid operator.</param>
        /// <param name="right">The object representing the right-hand side of the invalid operator.</param>
        /// <param name="op">The string representation of the invalid operator.</param>
        /// <returns>Returns null after printing the error message and exiting the environment.</returns>
        public static object? HandleInvalidRelationOperatorError([NotNull] ParserRuleContext context, object? left, object? right, string op)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in line {line}.\n" +
                              $"Cannot compare values of types {left?.GetType().Name.ToUpper()} and {right?.GetType().Name.ToUpper()} with '{op}' operator");
            Environment.Exit(400);
            return null;
        }

        /// <summary>
        /// Handles an invalid operator error by printing an error message to the console and exiting the program.
        /// </summary>
        /// <param name="context">The parser rule context that caused the error.</param>
        /// <param name="op">The invalid operator.</param>
        /// <param name="specifier">The type of operator (e.g., "arithmetic", "comparison").</param>
        /// <returns>Returns null after printing the error message and exiting the environment.</returns>
        public static object? HandleInvalidOperatorError([NotNull] ParserRuleContext context, string op, string specifier)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in line {line}.\n" +
                              $"Invalid {specifier}operator: {op}");
            Environment.Exit(400);
            return null;
        }

        /// <summary>
        /// Handles a semantic error that occurs when a negation operator is applied to a non-boolean value.
        /// </summary>
        /// <param name="context">The parser context where the error occurred.</param>
        /// <returns>Returns null as the error is handled by printing an error message and exiting the environment.</returns>

        public static object? HandleNegationError([NotNull] ParserRuleContext context)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in line {line}.\n" +
                              $"Argument must be of boolean value.");
            Environment.Exit(400);
            return null;
        }

        /// <summary>
        /// Handles semantic errors related to boolean operators in the input.
        /// </summary>
        /// <param name="context">The ParserRuleContext object representing the context where the error occurred.</param>
        /// <param name="boolop">The invalid boolean operator that caused the error.</param>
        /// <returns>Returns null as the error is handled by printing an error message and exiting the environment.</returns>
        public static object? HandleBoolOperationError([NotNull] ParserRuleContext context, string boolop)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in line {line}.\n" +
                              $"Invalid boolean operator: {boolop}");
            Environment.Exit(400);
            return null;
        }

        /// <summary>
        /// Handles a semantic error when trying to retrieve a unary value for a given symbol in a parsing context.
        /// </summary>
        /// <param name="context">The parsing context in which the error occurred.</param>
        /// <param name="symbol">The symbol for which a unary value could not be retrieved.</param>
        /// <returns>Returns null as the error is handled by printing an error message and exiting the environment.</returns>
        public static object? HandleUnaryError([NotNull] ParserRuleContext context, string symbol)
        {
            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in line {line}.\n" +
                              $"Cannot get unary value for symbol {symbol}");
            Environment.Exit(400);
            return null;
        }

        /// <summary>
        /// Handles an invalid increment type error and prints an error message to the console.
        /// </summary>
        /// <param name="context">The ParserRuleContext object associated with the error.</param>
        /// <param name="dictionarys">A dictionary containing objects to check for their types.</param>
        /// <param name="id">The key of the object to check for its type.</param>
        /// <returns>Returns null as the error is handled by printing an error message and exiting the environment.</returns>
        public static object? HandleInvalidIncrementTypeError([NotNull] ParserRuleContext context, Dictionary<string, object?> dictionarys, string id)
        {
            Type? type = null;

            if (dictionarys[id] is bool)
            {
                type = typeof(bool);
            }
            else if (dictionarys[id] is char)
            {
                type = typeof(char);
            }
            else if (dictionarys[id] is string)
            {
                type = typeof(string);
            }

            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in line {line}.\n" +
                              $"Cannot increment value of type {type?.Name.ToUpper()}");
            Environment.Exit(400);
            return null;
        }

        /// <summary>
        /// Handles an invalid decrement type error and prints the error message to the console.
        /// </summary>
        /// <param name="context">The context in which the error occurred.</param>
        /// <param name="dictionarys">The dictionary containing the values.</param>
        /// <param name="id">The identifier for the value.</param>
        /// <returns>Returns null as the error is handled by printing an error message and exiting the environment.</returns>
        public static object? HandleInvalidDecrementTypeError([NotNull] ParserRuleContext context, Dictionary<string, object?> dictionarys, string id)
        {
            Type? type = null;

            if ((Type?)dictionarys[id] == typeof(bool))
            {
                type = typeof(bool);
            }
            else if ((Type?)dictionarys[id] == typeof(char))
            {
                type = typeof(char);
            }
            else if ((Type?)dictionarys[id] == typeof(string))
            {
                type = typeof(string);
            }

            var line = context.Start.Line;
            Console.WriteLine($"Semantic Error: in line {line}.\n" +
                              $"Cannot decrement value of type {type?.Name.ToUpper()}");
            Environment.Exit(400);
            return null;
        }
    }
}
