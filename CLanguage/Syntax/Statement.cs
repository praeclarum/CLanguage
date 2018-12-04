using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public abstract class Statement
    {
        public Location Location { get; protected set; }

        public void Emit(EmitContext ec)
        {
            DoEmit(ec);
        }

        protected abstract void DoEmit(EmitContext ec);

		public abstract bool AlwaysReturns { get; }

        public Block ToBlock ()
        {
            var b = this as Block;
            if (b != null) return b;
            b = new Block ();
            b.AddStatement (this);
            return b;
        }
    }
}
