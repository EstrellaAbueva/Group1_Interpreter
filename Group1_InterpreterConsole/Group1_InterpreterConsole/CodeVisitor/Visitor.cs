using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Group1_InterpreterConsole.Contents;

namespace Group1_InterpreterConsole.CodeVisitor
{
    public class Visitor : CodeBaseVisitor<object?>
    {
        private Dictionary<string, object?> _variables { get; } = new ();
       
        public override object? VisitAssignment(CodeParser.AssignmentContext context)
        {
            var varName = context.IDENTIFIER()[0].GetText();

            var value = Visit(context.expression());

            _variables[varName] = value;
            return null;
        }

        public override object? VisitPrint(CodeParser.PrintContext context)
        {
            var value = Visit(context.expression());
            Console.WriteLine(value);
            return null;
        }

        //for visitVariable
        public override object? VisitVariable(CodeParser.VariableContext context)
        {
            var varName = context.IDENTIFIER().GetText();
            if (_variables.ContainsKey(varName))
            {
                return _variables[varName];
            }
            else
            {
                throw new Exception($"Variable {varName} not found");
            }
        }


    }
}
