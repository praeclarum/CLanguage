using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public abstract class Expression
    {
        public Location Location { get; protected set; }

        public bool HasError { get; set; }

        public void Emit(EmitContext ec)
        {
            DoEmit(ec);
        }

		public abstract CType GetEvaluatedCType (EmitContext ec);

        protected abstract void DoEmit(EmitContext ec);
    }
}
