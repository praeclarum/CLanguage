using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
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

			if (Left is VariableExpression) {
				var v = ec.ResolveVariable (((VariableExpression)Left).VariableName);

				if (v == null) {
					//fexe.Instructions.Add (new PushInstruction (0, CBasicType.SignedInt));
				} else if (v.Scope == VariableScope.Global) {
					//fexe.Instructions.Add (new StoreGlobalInstruction (v.Index));
				} else if (v.Scope == VariableScope.Local) {
					//fexe.Instructions.Add (new StoreLocalInstruction (v.Index));
				} else {
					throw new NotSupportedException ("Assigning to " + v.Scope);
				}
			} else {
				throw new NotImplementedException ("Assign " + Left.GetType ());
			}
			throw new NotImplementedException ("Assign " + Left.GetType ());
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Left, Right);
        }
    }
}
