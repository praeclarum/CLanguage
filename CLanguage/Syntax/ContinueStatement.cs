using System;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class ContinueStatement : Statement
    {
        public ContinueStatement ()
        {
        }

        public override bool AlwaysReturns => false;

        public override void AddDeclarationToBlock (BlockContext context)
        {
        }

        protected override void DoEmit (EmitContext ec)
        {
            if (ec.Loop is object) {
                ec.Emit (Interpreter.OpCode.Jump, ec.Loop.ContinueLabel);
            }
            else {
                ec.Report.Error (139, "No enclosing loop out of which to continue");
            }
        }
    }
}
