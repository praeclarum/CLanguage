using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Ast
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
			ec.EmitCast (Right.GetEvaluatedCType (ec), Left.GetEvaluatedCType (ec));
			ec.Emit (OpCode.Dup);

			if (Left is VariableExpression) {

				var v = ec.ResolveVariable (((VariableExpression)Left).VariableName);

				if (v == null) {
					ec.Emit (OpCode.Pop);
				} else if (v.Scope == VariableScope.Global) {
					ec.Emit (OpCode.StoreMemory, v.Index);
				} else if (v.Scope == VariableScope.Local) {
					ec.Emit (OpCode.StoreLocal, v.Index);
				} else if (v.Scope == VariableScope.Arg) {
					ec.Emit (OpCode.StoreArg, v.Index);
				} else {
					throw new NotSupportedException ("Assigning to scope '" + v.Scope + "'");
				}
			} else {
				throw new NotImplementedException ("Assign " + Left.GetType ().Name);
			}
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Left, Right);
        }
    }
}
