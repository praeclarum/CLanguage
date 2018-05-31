using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Types;

namespace CLanguage.Ast
{
    public class SizeOfExpression : Expression
    {
        public Expression Query { get; private set; }

        public SizeOfExpression(Expression query, Location loc)
        {
            Query = query;
            Location = loc;
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return CBasicType.SignedInt;
        }

        protected override void DoEmit(EmitContext ec)
        {
            throw new NotImplementedException();
        }
    }
}
