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
                    if (variable.VariableType is CBasicType ||
                        variable.VariableType is CPointerType) {

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
                    else if (variable.VariableType is CArrayType arrayType) {
                        ec.Emit (OpCode.LoadValue, variable.Index);
                    }
					else {
						throw new NotSupportedException ("Cannot evaluate variable type '" + variable.VariableType + "'");
					}
				}
			}
			else {
                ec.Report.Error (103, $"The name `{VariableName}` does not exist in the current context.");
				ec.Emit (OpCode.LoadValue, 0);
			}
        }

        protected override void DoEmitPointer (EmitContext ec)
        {
            var variable = ec.ResolveVariable (VariableName);

            if (variable != null) {

                if (variable.Scope == VariableScope.Function) {
                    ec.Emit (OpCode.LoadValue, variable.Index);
                }
                else if (variable.Scope == VariableScope.Arg) {
                    ec.Emit (OpCode.LoadValue, variable.Index);
                }
                else if (variable.Scope == VariableScope.Global) {
                    ec.Emit (OpCode.LoadValue, variable.Index);
                }
                else if (variable.Scope == VariableScope.Local) {
                    ec.Emit (OpCode.LoadValue, variable.Index);
                }
                else {
                    throw new NotSupportedException ("Cannot get address of variable scope '" + variable.Scope + "'");
                }
            }
            else {
                ec.Report.Error (103, $"The name `{VariableName}` does not exist in the current context.");
                ec.Emit (OpCode.LoadValue, 0);
            }
        }

        public override string ToString()
        {
            return VariableName.ToString();
        }
    }
}
