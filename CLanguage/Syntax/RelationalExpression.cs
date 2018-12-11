using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Types;

using CLanguage.Interpreter;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
	public enum RelationalOp
	{
		Equals,
		NotEquals,
		LessThan,
		LessThanOrEqual,
		GreaterThan,
		GreaterThanOrEqual,
	}

	public class RelationalExpression : Expression
	{
		public Expression Left { get; private set; }
		public RelationalOp Op { get; private set; }
		public Expression Right { get; private set; }

		public RelationalExpression (Expression left, RelationalOp op, Expression right)
		{
			Left = left;
			Op = op;
			Right = right;
		}

		protected override void DoEmit (EmitContext ec)
		{
			var aType = GetArithmeticType (Left, Right, Op.ToString (), ec);

			Left.Emit (ec);
			ec.EmitCast (Left.GetEvaluatedCType (ec), aType);
			Right.Emit(ec);
			ec.EmitCast (Right.GetEvaluatedCType (ec), aType);

			var ioff = ec.GetInstructionOffset (aType);

			switch (Op) {
			case RelationalOp.Equals:
				ec.Emit ((OpCode)(OpCode.EqualToInt8 + ioff));
				break;
			case RelationalOp.NotEquals:
				ec.Emit ((OpCode)(OpCode.EqualToInt8 + ioff));
                ec.Emit ((OpCode)(OpCode.NotInt8 + ioff));
                break;
			case RelationalOp.LessThan:
				ec.Emit ((OpCode)(OpCode.LessThanInt8 + ioff));
				break;
			case RelationalOp.LessThanOrEqual:
				ec.Emit ((OpCode)(OpCode.GreaterThanInt8 + ioff));
                ec.Emit ((OpCode)(OpCode.NotInt8 + ioff));
                break;
			case RelationalOp.GreaterThan:
				ec.Emit ((OpCode)(OpCode.GreaterThanInt8 + ioff));
				break;
			case RelationalOp.GreaterThanOrEqual:
				ec.Emit ((OpCode)(OpCode.LessThanInt8 + ioff));
                ec.Emit ((OpCode)(OpCode.NotInt8 + ioff));
                break;
			default:
				throw new NotSupportedException ("Unsupported relational operator '" + Op + "'");
			}
		}

        public override Value EvalConstant (EmitContext ec)
        {
            var leftType = Left.GetEvaluatedCType (ec);
            var rightType = Right.GetEvaluatedCType (ec);

            if (leftType.IsIntegral && rightType.IsIntegral) {
                var left = (int)Left.EvalConstant (ec);
                var right = (int)Right.EvalConstant (ec);

                switch (Op) {
                    case RelationalOp.Equals:
                        return left == right;
                    case RelationalOp.NotEquals:
                        return left != right;
                    case RelationalOp.LessThan:
                        return left < right;
                    case RelationalOp.LessThanOrEqual:
                        return left <= right;
                    case RelationalOp.GreaterThan:
                        return left > right;
                    case RelationalOp.GreaterThanOrEqual:
                        return left >= right;
                    default:
                        throw new NotSupportedException ("Unsupported relational operator '" + Op + "'");
                }
            }

            return base.EvalConstant (ec);
        }

        public override CType GetEvaluatedCType (EmitContext ec)
		{
            return CBasicType.Bool;
		}

		public override string ToString()
		{
			return string.Format("({0} {1} {2})", Left, Op, Right);
		}
    }
}
