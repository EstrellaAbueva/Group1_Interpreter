using Antlr4.Runtime.Misc;
using Group1_InterpreterConsole.Contents;
using Group1_InterpreterConsole.ErrorHandling;
using Group1_InterpreterConsole.Functions;
using Group1_InterpreterConsole.Methods;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Group1_InterpreterConsole.Contents.CodeParser;

namespace Group1_InterpreterConsole.CodeVisitor
{
    public class Visitor : CodeBaseVisitor<object?>
    {
        private Dictionary<string, object?> Variables { get; set; } = new Dictionary<string, object?>();
        private Dictionary<string, object?> VarTypes { get; set; } = new Dictionary<string, object?>();

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

        public override object? VisitVariable([NotNull] CodeParser.VariableContext context)
        {
            var varName = context.IDENTIFIER().GetText();
            return Helper.Variables(Variables, varName, context);
        }

        public override object? VisitConstant([NotNull] CodeParser.ConstantContext context)
        {
            var constant = context.GetText();
            return Helper.ConstantParser(constant, context);
        }

        public override object? VisitDisplay([NotNull] CodeParser.DisplayContext context)
        {
            var exp = Visit(context.expression());
            return Helper.Display(exp);
        }

        public override object? VisitType([NotNull] CodeParser.TypeContext context)
        {
            return Helper.TypeParser(context);
        }

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

        public override object? VisitVariable_dec([NotNull] CodeParser.Variable_decContext context)
        {
            foreach (var declarationContext in context.declaration())
            {
                VisitDeclaration(declarationContext);
            }
            return null;
        }

        public override object? VisitConcatOpExpression([NotNull] CodeParser.ConcatOpExpressionContext context)
        {
            // Get the left and right expressions;
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            return Helper.Concat(left, right);
        }

        public override object? VisitIdentifierExpression([NotNull] CodeParser.IdentifierExpressionContext context)
        {
            var identifier = context.IDENTIFIER().GetText();
            return Helper.Identifier(Variables, identifier, context);
        }

        public override object? VisitConstantExpression([NotNull] CodeParser.ConstantExpressionContext context)
        {
            return Helper.ConstantExpressionParser(context);
        }

        public override object? VisitVariable_assignment([NotNull] CodeParser.Variable_assignmentContext context)
        {
            var name = context.IDENTIFIER().GetText();

            VarTypes[name] = Visit(context.type());
            return Variables[name] = null;
        }

        public override object? VisitUnaryExpression([NotNull] CodeParser.UnaryExpressionContext context)
        {
            return Operators.Unary(context, context.unary_operator().GetText(), Visit(context.expression()));
        }

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

        public override object? VisitRelationalExpression([NotNull] RelationalExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var ops = context.compare_operator().GetText();

            return Operators.Relational(context, left, right, ops);
        }

        public override object? VisitParenthesisExpression([NotNull] ParenthesisExpressionContext context)
        {
            return Visit(context.expression());
        }

        public override object? VisitNOTExpression([NotNull] NOTExpressionContext context)
        {
            var expressionValue = Visit(context.expression());

            return Operators.Negation(context, expressionValue);
        }

        public override object? VisitBoolOpExpression([NotNull] BoolOpExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));
            var boolop = context.bool_operator().GetText();

            return Operators.BoolOperation(context, left, right, boolop);
        }

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

        public override object? VisitWhile_loop([NotNull] While_loopContext context)
        {
            var condition = Visit(context.expression());

            while (ErrorHandler.HandleConditionError(context, condition) == true)
            {
                var lines = context.line().ToList();
                foreach (var line in lines)
                {
                    Visit(line);
                }
                condition = Visit(context.expression());
            }
            return null;
        }

        public override object? VisitDo_while_loop([NotNull] Do_while_loopContext context)
        {
            var condition = Visit(context.expression());

            do
            {
                var lines = context.line().ToList();
                foreach (var line in lines)
                {
                    Visit(line);
                }
                condition = Visit(context.expression());
            } while (ErrorHandler.HandleConditionError(context, condition) == true);

            return null;
        }

        public override object? VisitFor_loop([NotNull] For_loopContext context)
        {
            // Extract the loop variables from the context
            var statement = context.statement();
            var expression = context.expression();
            var additional = context.additional();
            var lines = context.line();

            // Visit the loop initialization statement
            Visit(statement);

            // Evaluate the loop condition expression
            bool loopCondition = Convert.ToBoolean(Visit(expression));

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
            }

            return null;
        }

        public override object? VisitEscapeSequenceExpression([NotNull] EscapeSequenceExpressionContext context)
        {
            var sequence = context.GetText()[1];
            return Operators.Escape(context, sequence) ?? ErrorHandler.HandleInvalidEscapeSequenceError(context, sequence);
        }

        public override object? VisitNewlineOpExpression([NotNull] NewlineOpExpressionContext context)
        {
            var count = context.ChildCount - 1;
            var output = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                var childContext = context.GetChild(i);
                var childOutput = Visit(childContext);

                output.Append(childOutput);
            }

            output.Append(Environment.NewLine);

            return output;
        }

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

        public override object? VisitIncrement_statement([NotNull] Increment_statementContext context)
        {
            var id = context.IDENTIFIER().GetText();

            if (ErrorHandler.HandleUndeclaredVariableError(context, VarTypes, id))
            {
                Convert.ToInt32(Helper.Increment(context, VarTypes, Variables, id));
            }
            return null;
        }

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