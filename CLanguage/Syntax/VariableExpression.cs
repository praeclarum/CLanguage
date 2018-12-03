using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Types;

using CLanguage.Interpreter;

namespace CLanguage.Syntax
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
			var v = ec.ResolveVariable (VariableName, null);
			if (v != null) {
				return v.VariableType;
			}
			else {
				return CBasicType.SignedInt;
			}
        }

        protected override void DoEmit(EmitContext ec)
        {
			var variable = ec.ResolveVariable (VariableName, null);

			if (variable != null) {

				if (variable.Scope == VariableScope.Function) {
                    ec.Emit (OpCode.LoadConstant, Value.Pointer (variable.Address));
				}
				else {
                    if (variable.VariableType is CBasicType ||
                        variable.VariableType is CPointerType) {

                        if (variable.Scope == VariableScope.Arg) {
                            ec.Emit (OpCode.LoadArg, variable.Address);
                        }
                        else if (variable.Scope == VariableScope.Global) {
                            ec.Emit (OpCode.LoadGlobal, variable.Address);
                        }
                        else if (variable.Scope == VariableScope.Local) {
                            ec.Emit (OpCode.LoadLocal, variable.Address);
                        }
                        else if (variable.Scope == VariableScope.MachineConstant) {
                            ec.Emit (OpCode.LoadConstant, variable.Constant);
                        }
                        else {
                            throw new NotSupportedException ("Cannot evaluate variable scope '" + variable.Scope + "'");
                        }
                    }
                    else if (variable.VariableType is CArrayType arrayType) {

                        if (variable.Scope == VariableScope.Arg) {
                            ec.Emit (OpCode.LoadConstant, Value.Pointer (variable.Address));
                            ec.Emit (OpCode.LoadFramePointer);
                            ec.Emit (OpCode.OffsetPointer);
                        }
                        else if (variable.Scope == VariableScope.Global) {
                            ec.Emit (OpCode.LoadConstant, Value.Pointer (variable.Address));
                        }
                        else if (variable.Scope == VariableScope.Local) {
                            ec.Emit (OpCode.LoadConstant, Value.Pointer (variable.Address));
                            ec.Emit (OpCode.LoadFramePointer);
                            ec.Emit (OpCode.OffsetPointer);
                        }
                        else {
                            throw new NotSupportedException ("Cannot evaluate array variable scope '" + variable.Scope + "'");
                        }
                    }
					else {
						throw new NotSupportedException ("Cannot evaluate variable type '" + variable.VariableType + "'");
					}
				}
			}
			else {
				ec.Emit (OpCode.LoadConstant, 0);
			}
        }

        protected override void DoEmitPointer (EmitContext ec)
        {
            var res = ec.ResolveVariable (VariableName, null);

            if (res != null) {
                res.EmitPointer (ec);
            }
            else {
                ec.Emit (OpCode.LoadConstant, 0);
            }
        }

        public override string ToString()
        {
            return VariableName.ToString();
        }
    }
}
