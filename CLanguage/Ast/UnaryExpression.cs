using System;

using CLanguage.Types;
using CLanguage.Interpreter;

namespace CLanguage.Syntax
{
    public enum Unop
    {
		None,
		Not,
		Negate,
        BinaryComplement,
        PreIncrement,
        PreDecrement,
        PostIncrement,
        PostDecrement,
    }

    public class UnaryExpression : Expression
    {
        public Unop Op { get; private set; }
        public Expression Right { get; private set; }

        public UnaryExpression(Unop op, Expression right)
        {
            Op = op;
            Right = right;
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return (Op == Unop.Not) ? CBasicType.SignedInt : GetPromotedType (Right, Op.ToString (), ec);
        }

        protected override void DoEmit(EmitContext ec)
        {
			var aType = (CBasicType)GetEvaluatedCType (ec);

			Right.Emit (ec);
			ec.EmitCast (Right.GetEvaluatedCType (ec), aType);

			var ioff = GetInstructionOffset (aType, ec);

			switch (Op) {
			case Unop.None:
				break;
			case Unop.Negate:
				ec.Emit ((OpCode)(OpCode.NegateInt16 + ioff));
				break;
			case Unop.Not:
				ec.Emit ((OpCode)(OpCode.NotInt16 + ioff));
				break;
			default:
				throw new NotSupportedException ("Unsupported unary operator '" + Op + "'");
			}
		}

        public override string ToString()
        {
            return string.Format("({0} {1})", Op, Right);
        }
    }
}
