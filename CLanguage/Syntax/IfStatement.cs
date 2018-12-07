using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Interpreter;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class IfStatement : Statement
    {
        public Expression Condition { get; private set; }
        public Statement TrueStatement { get; private set; }
        public Statement? FalseStatement { get; private set; }

        public IfStatement(Expression condition, Statement trueStatement, Statement? falseStatement, Location loc)
        {
            if (condition == null) throw new ArgumentNullException (nameof (condition));
            if (trueStatement == null) throw new ArgumentNullException (nameof (trueStatement));
            Condition = condition;
            TrueStatement = trueStatement;
            FalseStatement = falseStatement;
            Location = loc;
        }

        public IfStatement(Expression condition, Statement trueStatement, Location loc)
        {
            if (condition == null) throw new ArgumentNullException (nameof (condition));
            if (trueStatement == null) throw new ArgumentNullException (nameof (trueStatement));
            Condition = condition;
            TrueStatement = trueStatement;
            FalseStatement = null;
            Location = loc;
        }

        protected override void DoEmit (EmitContext ec)
        {
            var endLabel = ec.DefineLabel();
            
            Condition.Emit(ec);
			ec.EmitCastToBoolean (Condition.GetEvaluatedCType (ec));

            if (FalseStatement == null)
            {
                ec.Emit (OpCode.BranchIfFalse, endLabel);
                TrueStatement.Emit(ec);
            }
            else
            {
                var falseLabel = ec.DefineLabel();
				ec.Emit (OpCode.BranchIfFalse, falseLabel);
                TrueStatement.Emit(ec);
				ec.Emit (OpCode.Jump, endLabel);
                ec.EmitLabel(falseLabel);
                FalseStatement.Emit(ec);
            }

            ec.EmitLabel(endLabel);
        }

        public override string ToString()
        {
            return string.Format("if ({0}) {1};", Condition, TrueStatement);
        }

		public override bool AlwaysReturns {
			get {
				var tr = TrueStatement.AlwaysReturns;
				var fr = FalseStatement != null ? FalseStatement.AlwaysReturns : false;
				return tr && fr;
			}
		}
    }
}
