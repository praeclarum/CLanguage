using System;

using CLanguage.Interpreter;

namespace CLanguage.Ast
{
	public class ReturnStatement : Statement
	{
		public Expression ReturnExpression { get; set; }

		public ReturnStatement (Expression returnExpression)
		{
			ReturnExpression = returnExpression;
		}

		public ReturnStatement ()
		{
		}

		protected override void DoEmit (EmitContext ec)
		{
			if (ReturnExpression != null) {
				if (ec.FunctionDecl.FunctionType.IsVoid) {
					ec.Report.Error (127, "A return keyword must not be followed by any expression when the function returns void");
				}
				else {
					ReturnExpression.Emit (ec);
					ec.EmitCast (ReturnExpression.GetEvaluatedCType (ec), ec.FunctionDecl.FunctionType.ReturnType);
					ec.Emit (OpCode.Return);
				}
			}
			else {
				if (ec.FunctionDecl.FunctionType.IsVoid) {
					ec.Emit (OpCode.Return);
				}
				else {
					ec.Report.Error (126, "A value is required for the return statement");
				}
			}
		}

		public override bool AlwaysReturns {
			get {
				return true;
			}
		}
	}
}

