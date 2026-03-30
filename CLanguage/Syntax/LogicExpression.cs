using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Types;
using CLanguage.Interpreter;
using CLanguage.Compiler;

namespace CLanguage.Syntax
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
            var shortCircuitLabel = ec.DefineLabel ();
            var endLabel = ec.DefineLabel ();

			Left.Emit (ec);
			ec.EmitCastToBoolean (Left.GetEvaluatedCType (ec));

            switch (Op) {
                case LogicOp.And:
                    // For &&: if left is false, result is 0
                    ec.Emit (OpCode.BranchIfFalse, shortCircuitLabel);
                    break;
                case LogicOp.Or:
                    // For ||: if left is true, result is 1
                    ec.Emit (OpCode.BranchIfTrue, shortCircuitLabel);
                    break;
            }

            // Left didn't short-circuit, so evaluate right side
			Right.Emit (ec);
            ec.EmitCastToBoolean (Right.GetEvaluatedCType (ec));

            switch (Op) {
                case LogicOp.And:
                    // For &&: if right is also false, result is 0
                    ec.Emit (OpCode.BranchIfFalse, shortCircuitLabel);
                    break;
                case LogicOp.Or:
                    // For ||: if right is true, result is 1
                    ec.Emit (OpCode.BranchIfTrue, shortCircuitLabel);
                    break;
            }

            // Neither side triggered the short-circuit branch
            switch (Op) {
                case LogicOp.And:
                    // Both sides were true
                    ec.Emit (OpCode.LoadConstant, 1);
                    break;
                case LogicOp.Or:
                    // Both sides were false
                    ec.Emit (OpCode.LoadConstant, 0);
                    break;
                default:
                    throw new NotSupportedException ("Unsupported logical operator '" + Op + "'");
            }
            ec.Emit (OpCode.Jump, endLabel);

            // Short-circuit path
            ec.EmitLabel (shortCircuitLabel);
            switch (Op) {
                case LogicOp.And:
                    ec.Emit (OpCode.LoadConstant, 0);
                    break;
                case LogicOp.Or:
                    ec.Emit (OpCode.LoadConstant, 1);
                    break;
            }

            ec.EmitLabel (endLabel);
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
