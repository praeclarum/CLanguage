using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Interpreter;
using CLanguage.Types;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class SizeOfExpression : Expression
    {
        public Expression Query { get; private set; }

        public SizeOfExpression(Expression query)
        {
            Query = query;
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return CBasicType.UnsignedLongInt;
        }

        protected override void DoEmit(EmitContext ec)
        {
            throw new NotImplementedException(GetType ().Name + ": Emit");
        }
    }
}
