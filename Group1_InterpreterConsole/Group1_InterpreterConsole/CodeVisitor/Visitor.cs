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
        public override object? VisitProgram(CodeParser.ProgramContext context)
        {
            return base.VisitProgram(context);
        }

        public override object? VisitStatement(CodeParser.StatementContext context)
        {
            return base.VisitStatement(context);
        }

        public override object? VisitAssignment(CodeParser.AssignmentContext context)
        {
            return base.VisitAssignment(context);
        }

        public override object? VisitDeclaration(CodeParser.DeclarationContext context)
        {
            return base.VisitDeclaration(context);
        }

        public override object? VisitVariable(CodeParser.VariableContext context)
        {
            return base.VisitVariable(context);
        }
    }
}
