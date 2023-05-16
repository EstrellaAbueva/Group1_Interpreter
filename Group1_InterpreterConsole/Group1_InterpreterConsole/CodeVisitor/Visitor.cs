using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.ErrorHandling;
using Group1_InterpreterConsole.Functions;
using System.Text;
using static Group1_InterpreterConsole.Contents.CodeParser;

namespace Group1_InterpreterConsole.CodeVisitor
{
    public class Visitor : CodeBaseVisitor<object?>
    {
        private Dictionary<string, object?> Variables { get; set; } = new Dictionary<string, object?>();
        private Dictionary<string, object?> VarTypes { get; set; } = new Dictionary<string, object?>();

        private bool isInfiniteLoop = false;

        /// <summary>
        /// Visits Assignment rule.
        /// </summary>
        /// <param name="context">Context of the rule.</param>
        /// <returns>Null</returns>
        public override object? VisitAssignment([NotNull] CodeParser.AssignmentContext context)
        {
            foreach (var i in context.IDENTIFIER())
            {
                var expression = context.expression().Accept(this);

                // check type
                if (ErrorHandler.HandleTypeError(context, expression, (Type?)VarTypes[i.GetText()], "Variable Assignment"))
                { 
                    Variables[i.GetText()] = expression;
                }  
            }
            return null;
        }

        public override object? VisitFor_assignment([NotNull] For_assignmentContext context)
        {
            foreach (var i in context.IDENTIFIER())
            {
                var expression = context.expression().Accept(this);

                // check type
                if (ErrorHandler.HandleTypeError(context, expression, (Type?)VarTypes[i.GetText()], "Variable Assignment"))
                {
                    Variables[i.GetText()] = expression;
                }
            }
            return null;
        }

        /// <summary>
        /// Visits the Variable rule.
        /// </summary>
        /// <param name="context">Context of the rule.</param>
        /// <returns>Result of the Helper class' Variable function.</returns>
        public override object? VisitVariable([NotNull] CodeParser.VariableContext context)
        {
            var varName = context.IDENTIFIER().GetText();
            return Helper.Variables(Variables, varName, context);
        }

        /// <summary>
        /// Visit the Constant rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Result of the Helper class' ConstantParser function.</returns>
        public override object? VisitConstant([NotNull] CodeParser.ConstantContext context)
        {
            var constant = context.GetText();
            return Helper.ConstantParser(constant, context);
        }

        /// <summary>
        /// Visits the Display rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Result of the Helper class' Display function.</returns>
        public override object? VisitDisplay([NotNull] CodeParser.DisplayContext context)
        {
            var exp = Visit(context.expression());
            return Helper.Display(exp);
        }

        /// <summary>
        /// Visits the Type rule.
        /// </summary>
        /// <param name="context">Contxt of the current rule.</param>
        /// <returns>Result of the Helper class' TypeParser function.</returns>
        public override object? VisitType([NotNull] CodeParser.TypeContext context)
        {
            return Helper.TypeParser(context);
        }

        /// <summary>
        /// Visits the Declration rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Null</returns>
        public override object? VisitDeclaration([NotNull] CodeParser.DeclarationContext context)
        {
            var type = Visit(context.type());
            var typestr = context.type().GetText();
            var varnames = context.IDENTIFIER();

            // remove type
            var contextstring = context.GetText().Replace(typestr, "");

            var contextParts = contextstring.Split(',');
            var exp = context.expression();
            int expctr = 0;

            // traverse each part
            for (int x = 0; x < contextParts.Length; x++)
            {
                if (Variables.ContainsKey(varnames[x].GetText()))
                {
                    Console.WriteLine(varnames[x].GetText() + "is already declared");
                    continue;
                }
                if (contextParts[x].Contains('='))
                {
                    if (expctr < exp.Count())
                    {
                        // check type
                        if (ErrorHandler.HandleTypeError(context, Visit(exp[expctr]), (Type?)type, "Variable Declaration"))
                        {
                            Variables[varnames[x].GetText()] = Visit(exp[expctr]);
                            VarTypes[varnames[x].GetText()] = type;
                        }
                        expctr++;
                    }
                }
                else
                {
                    Variables[varnames[x].GetText()] = null;
                    VarTypes[varnames[x].GetText()] = type;
                }
            }
            return null;
        }

        ///// <summary>
        ///// Visits the Variable declaration rule.
        ///// </summary>
        ///// <param name="context">Context of the current rule.</param>
        ///// <returns>Null</returns>
        //public override object? VisitVariable_dec([NotNull] CodeParser.Variable_decContext context)
        //{
        //    foreach (var declarationContext in context.declaration())
        //    {
        //        VisitDeclaration(declarationContext);
        //    }
        //    return null;
        //}

        /// <summary>
        /// Visits the Concat Operation rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Result of the Helper class' Concat function.</returns>
        public override object? VisitConcatOpExpression([NotNull] CodeParser.ConcatOpExpressionContext context)
        {
            // Get the left and right expressions;
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            return Helper.Concat(left, right);
        }

        /// <summary>
        /// Visits the Identifier Expression rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Result of the Helper class' Identifier function.</returns>
        public override object? VisitIdentifierExpression([NotNull] CodeParser.IdentifierExpressionContext context)
        {
            var identifier = context.IDENTIFIER().GetText();
            return Helper.Identifier(Variables, identifier, context);
        }

        /// <summary>
        /// Visits the Constant Expression rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Result of the Helper class' ConstantExpressionParser function.</returns>
        public override object? VisitConstantExpression([NotNull] CodeParser.ConstantExpressionContext context)
        {
            return Helper.ConstantExpressionParser(context);
        }

        /// <summary>
        /// Visits the Variable Assignment rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Variable given.</returns>
        public override object? VisitVariable_assignment([NotNull] CodeParser.Variable_assignmentContext context)
        {
            var name = context.IDENTIFIER().GetText();

            VarTypes[name] = Visit(context.type());
            return Variables[name] = null;
        }

        /// <summary>
        /// Visits the UnaryExpression rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns> Result of the Operator class' Unary function.
        /// </returns>
        public override object? VisitUnaryExpression([NotNull] CodeParser.UnaryExpressionContext context)
        {
            return Operators.Unary(context, context.unary_operator().GetText(), Visit(context.expression()));
        }

        /// <summary>
        /// Visits AdditiveExpression rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>1. Result of the Operator class' Add function.
        ///          2. Result of the Operator class' Subtract function.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public override object? VisitAdditiveExpression([NotNull] AdditiveExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var ops = context.add_operator().GetText();

            return ops switch
            {
                "+" => Operators.Add(context, left, right),
                "-" => Operators.Subtract(context, left, right),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Visits MulticativeExpression rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>1. Result of the Operator class' Multiply function.
        ///          2. Result of the Operator class' Divide function.
        ///          3. Result of the Operator class' Modulo function</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override object? VisitMultiplicativeExpression([NotNull] MultiplicativeExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var ops = context.multiply_operator().GetText();

            return ops switch
            {
                "*" => Operators.Multiply(context, left, right),
                "/" => Operators.Divide(context, left, right),
                "%" => Operators.Modulo(context, left, right),
                _ => throw new NotImplementedException(),
            };
        }

        /// <summary>
        /// Visits RelationalExpression rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Result of the Operator class' Relational function.</returns>
        public override object? VisitRelationalExpression([NotNull] RelationalExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var ops = context.compare_operator().GetText();

            return Operators.Relational(context, left, right, ops);
        }

        /// <summary>
        /// Visits ParenthesisExpression rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Result of the expression inside the variable</returns>
        public override object? VisitParenthesisExpression([NotNull] ParenthesisExpressionContext context)
        {
            return Visit(context.expression());
        }

        /// <summary>
        /// Visits NOTExpression rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Result of the Operator class' Negation function.</returns>
        public override object? VisitNOTExpression([NotNull] NOTExpressionContext context)
        {
            var expressionValue = Visit(context.expression());

            return Operators.Negation(context, expressionValue);
        }

        /// <summary>
        /// Visits BoolOpExpression rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Result of the Operator class' BoolOperation function.</returns>
        public override object? VisitBoolOpExpression([NotNull] BoolOpExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            var boolop = context.bool_operator().GetText();

            return Operators.BoolOperation(context, left, right, boolop);
        }

        /// <summary>
        /// Visits If_block rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>null</returns>
        public override object? VisitIf_block([NotNull] If_blockContext context)
        {
            var condition = Visit(context.expression());

            var result = ErrorHandler.HandleConditionError(context, condition);
            result = Convert.ToBoolean(result);
            if (ErrorHandler.HandleConditionError(context, condition) == true)
            {
                var lines = context.line().ToList();
                foreach (var line in lines)
                {
                    Visit(line);
                }
            }
            else
            {
                var elseIfBlocks = context.else_if_block();
                foreach (var elseIfBlock in elseIfBlocks)
                {
                    var elseIfCondition = Visit(elseIfBlock.expression());
                    if (ErrorHandler.HandleConditionError(context, elseIfCondition) == true)
                    {
                        var elseIfLines = elseIfBlock.line().ToList();
                        foreach (var line in elseIfLines)
                        {
                            Visit(line);
                        }
                        // Exit the loop once the first true else-if block is found
                        return null;
                    }
                }

                var elseBlock = context.else_block();
                if (elseBlock != null)
                {
                    var elseLines = elseBlock.line().ToList();
                    foreach (var line in elseLines)
                    {
                        Visit(line);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Visits While_loop rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>null</returns>
        public override object? VisitWhile_loop([NotNull] While_loopContext context)
        {
            var condition = Visit(context.expression());
            var maxIterations = 1000; // Set a maximum number of iterations
            var iterations = 0;

            while (ErrorHandler.HandleConditionError(context, condition) == true)
            {
                if (iterations >= maxIterations) // Check if the maximum number of iterations is reached
                {
                    return ErrorHandler.HandleInfiniteLoopError(context);
                } else
                {
                    var lines = context.line().ToList();
                    foreach (var line in lines)
                    {
                        Visit(line);
                    }
                    condition = Visit(context.expression());
                    iterations++;
                }
            }
            return null;
        }

        /// <summary>
        /// Visits Do_while_loop rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>null</returns>
        public override object? VisitDo_while_loop([NotNull] Do_while_loopContext context)
        {
            var condition = Visit(context.expression());
            var maxIterations = 1000; // Set a maximum number of iterations
            var iterations = 0;

            do
            {
                var lines = context.line().ToList();
                foreach (var line in lines)
                {
                    Visit(line);
                }
                condition = Visit(context.expression());
                iterations++;

                if (iterations >= maxIterations) // Check if the maximum number of iterations is reached
                {
                    return ErrorHandler.HandleInfiniteLoopError(context);
                }

            } while (ErrorHandler.HandleConditionError(context, condition) == true);

            return null;
        }

        /// <summary>
        /// Visits For_loop rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>null</returns>
        public override object? VisitFor_loop([NotNull] For_loopContext context)
        {
            // Extract the loop variables from the context
            var assignment = context.for_assignment();
            var expression = context.expression();
            var additional = context.additional();
            var lines = context.line();

            // Visit the loop initialization statement
            Visit(assignment);

            // Evaluate the loop condition expression
            bool loopCondition = Convert.ToBoolean(Visit(expression));

            var maxIterations = 1000; // Set a maximum number of iterations
            var iterations = 0;

            // Loop while the condition is true
            while (loopCondition)
            {
                // Execute the loop body lines
                foreach (var line in lines)
                {
                    Visit(line);
                }

                // Evaluate the loop additional expression
                Visit(additional);

                // Evaluate the loop condition expression again
                loopCondition = Convert.ToBoolean(Visit(expression));
                iterations++;

                if (iterations >= maxIterations)
                {
                    ErrorHandler.HandleInfiniteLoopError(context);
                    break;
                }
            }

            return null;
        }

        /// <summary>
        /// Visits EscapeSequenceExpression rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>Result of the Operator class' Escape function.</returns>
        public override object? VisitEscapeSequenceExpression([NotNull] EscapeSequenceExpressionContext context)
        {
            var sequence = context.GetText()[1];
            return Operators.Escape(context, sequence) ?? ErrorHandler.HandleInvalidEscapeSequenceError(context, sequence);
        }

        /// <summary>
        /// Visits NewlineOpExpression rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>String on a new line.</returns>
        public override object? VisitNewlineOpExpression([NotNull] NewlineOpExpressionContext context)
        {
            return "\n";
        }

        /// <summary>
        /// Visits Scan rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>null</returns>
        public override object? VisitScan([NotNull] ScanContext context)
        {
            var input = Console.ReadLine();
            var inputs = input!.Split(',').Select(s => s.Trim()).ToArray();

            if (inputs.Length < 1 || inputs.Length > context.IDENTIFIER().Length)
            {
                return ErrorHandler.HandlleInvalidScanInputsError(context, context.IDENTIFIER().Length, inputs.Length, "Input Scan");
                //throw new ArgumentException($"Invalid number of inputs. Expected between 1 and {context.IDENTIFIER().Length}, but got {inputs.Length}.");
            }

            for (int i = 0; i < inputs.Length; i++)
            {
                var idName = context.IDENTIFIER(i).GetText();
                if (!VarTypes.ContainsKey(idName))
                {
                    return ErrorHandler.HandleUndeclaredVariableError(context, VarTypes, idName);
                    //throw new ArgumentException($"Variable '{idName}' is not declared.");
                }
                Helper.Scan(context, VarTypes, Variables, idName, inputs[i]);
            }

            return null;
        }

        /// <summary>
        /// Visits Increment_statement rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>null</returns>
        public override object? VisitIncrement_statement([NotNull] Increment_statementContext context)
        {
            var id = context.IDENTIFIER().GetText();

            if (ErrorHandler.HandleUndeclaredVariableError(context, VarTypes, id))
            {
                Convert.ToInt32(Helper.Increment(context, VarTypes, Variables, id));
            }
            return null;
        }

        /// <summary>
        /// Visits Decrement_statement rule.
        /// </summary>
        /// <param name="context">Context of the current rule.</param>
        /// <returns>null</returns>
        public override object? VisitDecrement_statement([NotNull] Decrement_statementContext context)
        {
            var id = context.IDENTIFIER().GetText();

            if (ErrorHandler.HandleUndeclaredVariableError(context, VarTypes, id))
            {
                Convert.ToInt32(Helper.Decrement(context, VarTypes, Variables, id));
            }
            return null;
        }
    }
}