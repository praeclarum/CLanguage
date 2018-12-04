using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Types;

using CLanguage.Interpreter;

namespace CLanguage.Syntax
{
    public enum Binop
    {
        Add,
        Subtract,
        Multiply,
        Divide,
        Mod,
        ShiftLeft,
        ShiftRight,
		BinaryAnd,
		BinaryOr,
		BinaryXor,
	}

    public class BinaryExpression : Expression
    {
        public Expression Left { get; private set; }
        public Binop Op { get; private set; }
        public Expression Right { get; private set; }

        public BinaryExpression(Expression left, Binop op, Expression right)
        {
			if (left == null) throw new ArgumentNullException (nameof (left));
			if (right == null) throw new ArgumentNullException (nameof (right));

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
                case Binop.Add:
                    ec.Emit ((OpCode)(OpCode.AddInt8 + ioff));
                    break;
                case Binop.Subtract:
                    ec.Emit ((OpCode)(OpCode.SubtractInt8 + ioff));
                    break;
                case Binop.Multiply:
                    ec.Emit ((OpCode)(OpCode.MultiplyInt8 + ioff));
                    break;
                case Binop.Divide:
                    ec.Emit ((OpCode)(OpCode.DivideInt8 + ioff));
                    break;
                case Binop.Mod:
                    ec.Emit ((OpCode)(OpCode.ModuloInt8 + ioff));
                    break;
                case Binop.BinaryAnd:
                    ec.Emit ((OpCode)(OpCode.BinaryAndInt8 + ioff));
                    break;
                case Binop.BinaryOr:
                    ec.Emit ((OpCode)(OpCode.BinaryOrInt8 + ioff));
                    break;
                case Binop.BinaryXor:
                    ec.Emit ((OpCode)(OpCode.BinaryXorInt8 + ioff));
                    break;
                case Binop.ShiftLeft:
                    ec.Emit ((OpCode)(OpCode.ShiftLeftInt8 + ioff));
                    break;
                case Binop.ShiftRight:
                    ec.Emit ((OpCode)(OpCode.ShiftRightInt8 + ioff));
                    break;
                default:
                    throw new NotSupportedException ("Unsupported binary operator '" + Op + "'");
            }
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return GetArithmeticType (Left, Right, Op.ToString (), ec);
		}

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", Left, Op, Right);
        }

        public override Value EvalConstant (EmitContext ec)
        {
            var leftType = Left.GetEvaluatedCType (ec);
            var rightType = Right.GetEvaluatedCType (ec);

            if (leftType.IsIntegral && rightType.IsIntegral) {
                var left = (int)Left.EvalConstant (ec);
                var right = (int)Right.EvalConstant (ec);

                switch (Op) {
                    case Binop.Add:
                        return left + right;
                    case Binop.Subtract:
                        return left - right;
                    case Binop.Multiply:
                        return left * right;
                    case Binop.Divide:
                        return left / right;
                    case Binop.Mod:
                        return left % right;
                    case Binop.BinaryAnd:
                        return left & right;
                    case Binop.BinaryOr:
                        return left | right;
                    case Binop.BinaryXor:
                        return left ^ right;
                    case Binop.ShiftLeft:
                        return left << right;
                    case Binop.ShiftRight:
                        return left >> right;
                    default:
                        throw new NotSupportedException ("Unsupported binary operator '" + Op + "'");
                }
            }

            return base.EvalConstant (ec);
        }
    }
}
