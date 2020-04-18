using System;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class BreakStatement : Statement
    {
        public BreakStatement ()
        {
        }

        public override bool AlwaysReturns => false;

        protected override void DoEmit (EmitContext ec)
        {
            if (ec.Loop is object) {
                ec.Emit (Interpreter.OpCode.Jump, ec.Loop.BreakLabel);
            }
            else {
                ec.Report.Error (139, "No enclosing loop out of which to break");
            }
        }
    }
}
