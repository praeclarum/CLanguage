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

        public override CType ExpressionType
        {
            get { return CType.Void; }
        }

        protected override void DoEmit(EmitContext ec)
        {
			var v = ec.ResolveVariable (VariableName);
			if (v != null) {
				ec.EmitVariable (v);
			}
			else {
				ec.Report.Error (103, "The name `" + VariableName + "' does not exist in the current context");
			}
        }

        public override string ToString()
        {
            return VariableName.ToString();
        }
    }
}
