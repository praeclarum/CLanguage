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
			var variable = ec.ResolveVariable (VariableName);

			if (variable != null) {
				if (variable.Scope == VariableScope.Arg) {
					//fexe.Instructions.Add (new LoadArgInstruction (variable.Index));
				}
				else if (variable.Scope == VariableScope.Function) {
					//fexe.Instructions.Add (new PushInstruction (variable.Function, variable.Function.FunctionType));
				}
				else if (variable.Scope == VariableScope.Global) {
					//fexe.Instructions.Add (new LoadGlobalInstruction (variable.Index));
				}
				else if (variable.Scope == VariableScope.Local) {
					//fexe.Instructions.Add (new LoadLocalInstruction (variable.Index));
				}
				else {

					throw new NotImplementedException (variable.Scope.ToString ());
				}
			}
			else {
				new ConstantExpression (0, CBasicType.SignedInt).Emit (ec);
			}
        }

        public override string ToString()
        {
            return VariableName.ToString();
        }
    }
}
