using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Interpreter;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
	public class ExpressionStatement : Statement
	{
		public Expression Expression { get; set; }

		public ExpressionStatement (Expression expr)
		{
			Expression = expr;
		}

		protected override void DoEmit (EmitContext ec)
		{
			if (Expression != null) {
				Expression.Emit (ec);

				ec.Emit (OpCode.Pop);
			}
		}

		public override string ToString ()
		{
			return string.Format ("{0};", Expression);
		}

		public override bool AlwaysReturns {
			get {
				return false;
			}
		}
	}
}
