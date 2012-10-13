using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
	public enum LogicOp
	{
		And,
		Or,
	}

	public class LogicExpression : Expression
	{
		public Expression Left { get; private set; }
		public LogicOp Op { get; private set; }
		public Expression Right { get; private set; }

		public LogicExpression (Expression left, LogicOp op, Expression right)
		{
			Left = left;
			Op = op;
			Right = right;
		}

		protected override void DoEmit (EmitContext ec)
		{
			Left.Emit (ec);
			ec.EmitCastToBoolean (Left.GetEvaluatedCType (ec));

			Right.Emit (ec);
			ec.EmitCastToBoolean (Right.GetEvaluatedCType (ec));

			switch (Op) {
			case LogicOp.And:
				ec.Emit (OpCode.LogicalAnd);
				break;
			case LogicOp.Or:
				ec.Emit (OpCode.LogicalOr);
				break;
			default:
				throw new NotSupportedException ("Unsupported logical operator '" + Op + "'");
			}
		}

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return CBasicType.SignedInt;
		}

		public override string ToString()
		{
			return string.Format("({0} {1} {2})", Left, Op, Right);
		}
	}
}
