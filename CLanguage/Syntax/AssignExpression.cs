using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Types;
using CLanguage.Interpreter;

namespace CLanguage.Syntax
{
    public class AssignExpression : Expression
    {
        public Expression Left { get; private set; }
        public Expression Right { get; private set; }

        public AssignExpression(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return Left.GetEvaluatedCType (ec);
        }

        protected override void DoEmit(EmitContext ec)
        {
            Right.Emit(ec);

            if (Left is VariableExpression) {

                ec.EmitCast (Right.GetEvaluatedCType (ec), Left.GetEvaluatedCType (ec));
                ec.Emit (OpCode.Dup);

                string variableName = ((VariableExpression)Left).VariableName;
                var v = ec.ResolveVariable (variableName, null);

                if (v == null) {
                    ec.Emit (OpCode.Pop);
                    ec.Report.Error (103, $"The name `{variableName}` does not exist in the current context.");
                }
                else if (v.Scope == VariableScope.Global) {
                    ec.Emit (OpCode.StoreMemory, v.Index);
                }
                else if (v.Scope == VariableScope.Local) {
                    ec.Emit (OpCode.StoreLocal, v.Index);
                }
                else if (v.Scope == VariableScope.Arg) {
                    ec.Emit (OpCode.StoreArg, v.Index);
                }
                else if (v.Scope == VariableScope.Function) {
                    ec.Emit (OpCode.Pop);
                    ec.Report.Error (1656, $"Cannot assign to `{variableName}` because it is a function");
                }
                else {
                    throw new NotSupportedException ("Assigning to scope '" + v.Scope + "'");
                }
            }
            else {
                ec.Emit (OpCode.Pop);
                ec.Report.Error (131, "The left-hand side of an assignment must be a variable");
            }
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Left, Right);
        }
    }
}
