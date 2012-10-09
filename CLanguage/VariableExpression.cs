using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class VariableExpression : Expression
    {
        public string VariableName { get; private set; }

        public VariableExpression(string val)
        {
            VariableName = val;
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			var v = ec.ResolveVariable (VariableName);
			if (v != null) {
				return v.VariableType;
			}
			else {
				return CBasicType.SignedInt;
			}
        }

        protected override void DoEmit(EmitContext ec)
        {
			var v = ec.ResolveVariable (VariableName);
			if (v != null) {
				ec.EmitVariable (v);
			}
			else {
				ec.EmitConstant (0, CBasicType.SignedInt);
			}
        }

        public override string ToString()
        {
            return VariableName.ToString();
        }
    }
}
