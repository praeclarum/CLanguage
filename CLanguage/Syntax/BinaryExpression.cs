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
			if (left == null) throw new ArgumentNullException ("left");
			if (right == null) throw new ArgumentNullException ("right");

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
    }
}
