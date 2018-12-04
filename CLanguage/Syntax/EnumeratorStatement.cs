using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Types;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class EnumeratorStatement : Statement
    {
        public string Name { get; }
        public Expression LiteralValue { get; }

        public override bool AlwaysReturns => false;

        public EnumeratorStatement (string left, Expression right = null)
        {
            Name = left ?? throw new ArgumentNullException (nameof (left));
            LiteralValue = right;
        }

        public override string ToString ()
        {
            return string.Format ("{0} = {1}", Name, LiteralValue);
        }

        protected override void DoEmit (EmitContext ec)
        {
        }
    }
}
