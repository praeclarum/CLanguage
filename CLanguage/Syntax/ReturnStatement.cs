using System;

using CLanguage.Interpreter;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
	public class ReturnStatement : Statement
	{
		public Expression? ReturnExpression { get; set; }

		public ReturnStatement (Expression returnExpression)
		{
			ReturnExpression = returnExpression;
		}

		public ReturnStatement ()
		{
		}

		protected override void DoEmit (EmitContext ec)
		{
            var f = ec.FunctionDecl;
            if (f == null) {
                ec.Report.Error (1519, "Invalid return outside of function");
                return;
            }

			if (ReturnExpression != null) {
				if (f.FunctionType.IsVoid) {
					ec.Report.Error (127, "A return keyword must not be followed by any expression when the function returns void");
				}
				else {
					ReturnExpression.Emit (ec);
					ec.EmitCast (ReturnExpression.GetEvaluatedCType (ec), f.FunctionType.ReturnType);
					ec.Emit (OpCode.Return);
				}
			}
			else {
				if (f.FunctionType.IsVoid) {
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

