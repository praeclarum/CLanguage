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

        protected override void DoEmit (EmitContext ec)
        {
            throw new NotImplementedException (" NO CONTINUE");
        }
    }
}
