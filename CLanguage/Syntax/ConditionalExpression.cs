using System;
using CLanguage.Types;
using CLanguage.Interpreter;

namespace CLanguage.Syntax
{
	public class ConditionalExpression : Expression
	{
		public Expression Condition { get; set; }
		public Expression TrueValue { get; set; }
		public Expression FalseValue { get; set; }

		public ConditionalExpression (Expression condition, Expression trueValue, Expression falseValue)
		{
			Condition = condition;
			TrueValue = trueValue;
			FalseValue = falseValue;
		}

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return TrueValue.GetEvaluatedCType (ec);
		}

		protected override void DoEmit (EmitContext ec)
		{
			var falseLabel = ec.DefineLabel ();
			var endLabel = ec.DefineLabel ();

			Condition.Emit (ec);
			ec.EmitCastToBoolean (Condition.GetEvaluatedCType (ec));
			ec.Emit (OpCode.BranchIfFalse, falseLabel);

			TrueValue.Emit (ec);
			ec.Emit (OpCode.Jump, endLabel);

			ec.EmitLabel (falseLabel);
			FalseValue.Emit (ec);

			ec.EmitLabel (endLabel);
		}
	}
}

