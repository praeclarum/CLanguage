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
            //
            // VariableExpressions must be resolved. If they are not then we just output a 0
            //
            ConstantExpression.Zero.Emit(ec);
        }

        public override string ToString()
        {
            return VariableName.ToString();
        }
    }
}
