using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class SizeOfExpression : Expression
    {
        public Expression Query { get; private set; }

        public SizeOfExpression(Expression query, Location loc)
        {
            Query = query;
            Location = loc;
        }

        public override CType ExpressionType
        {
            get { return CBasicType.SignedInt; }
        }

        protected override Expression DoResolve(ResolveContext rc)
        {
            return this;
        }

        protected override void DoEmit(EmitContext ec)
        {
            throw new NotImplementedException();
        }
    }
}
