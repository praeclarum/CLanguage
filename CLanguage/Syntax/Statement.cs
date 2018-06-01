using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Interpreter;

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
    }
}
