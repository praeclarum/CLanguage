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
            var label = new Label ();
			Left.Emit (ec);
			ec.EmitCastToBoolean (Left.GetEvaluatedCType (ec));

            switch (Op) {
                case LogicOp.And:
                    ec.Emit (OpCode.BranchIfFalseNoSPChange, label);  //or Dup instruction. 
                    break;
                case LogicOp.Or:
                    ec.Emit (OpCode.BranchIfTrueNoSPChange, label);
                    break;
            }

			Right.Emit (ec); // (true)||(1/0) <- second part not executed in C 
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
            ec.EmitLabel (label);
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
