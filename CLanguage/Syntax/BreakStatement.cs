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
            throw new NotImplementedException ("NO BREAK");
        }
    }
}
