using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Ast
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
			var variable = ec.ResolveVariable (VariableName);

			if (variable != null) {

				if (variable.Scope == VariableScope.Function) {
					ec.Emit (OpCode.LoadFunction, variable.Index);
				}
				else {
					var basicType = variable.VariableType as CBasicType;
					if (basicType != null) {

						var size = basicType.GetSize (ec);
						if (size > 4 || basicType.Equals (CBasicType.Double)) {
							throw new NotSupportedException ("Cannot evaluate variable type '" + variable.VariableType + "'");
						}

						if (variable.Scope == VariableScope.Arg) {
							ec.Emit (OpCode.LoadArg, variable.Index);
						}
						else if (variable.Scope == VariableScope.Global) {
							ec.Emit (OpCode.LoadMemory, variable.Index);
						}
						else if (variable.Scope == VariableScope.Local) {
							ec.Emit (OpCode.LoadLocal, variable.Index);
						}
						else {
							throw new NotSupportedException ("Cannot evaluate variable scope '" + variable.Scope + "'");
						}
					}
					else {
						throw new NotSupportedException ("Cannot evaluate variable type '" + variable.VariableType + "'");
					}
				}
			}
			else {
				ec.Emit (OpCode.LoadValue, 0);
			}
        }

        public override string ToString()
        {
            return VariableName.ToString();
        }
    }
}
