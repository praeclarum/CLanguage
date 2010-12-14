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

        public Expression Resolve(ResolveContext rc)
        {
            return DoResolve(rc);
        }

        public void Emit(EmitContext ec)
        {
            Resolve(ec).DoEmit(ec);
        }

        public abstract CType ExpressionType { get; }

        protected abstract void DoEmit(EmitContext ec);

        protected abstract Expression DoResolve(ResolveContext rc);
    }
}
