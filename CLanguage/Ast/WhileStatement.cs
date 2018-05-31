using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Interpreter;

namespace CLanguage.Ast
{
    public class WhileStatement : Statement
    {
        public bool IsDo { get; private set; }
        public Expression Condition { get; private set; }
        public Statement Loop { get; private set; }

        public WhileStatement(bool isDo, Expression condition, Statement loop)
        {
            IsDo = isDo;
            Condition = condition;
            Loop = loop;
        }

        protected override void DoEmit(EmitContext ec)
        {
            if (IsDo)
            {
                throw new NotImplementedException();
            }
            else
            {
                var condLabel = ec.DefineLabel();
                var loopLabel = ec.DefineLabel();
                var endLabel = ec.DefineLabel();

                ec.EmitLabel(condLabel);
                Condition.Emit(ec);
				ec.EmitCastToBoolean (Condition.GetEvaluatedCType (ec));
				ec.Emit (OpCode.BranchIfFalse, endLabel);
                ec.EmitLabel(loopLabel);
                Loop.Emit(ec);
                ec.Emit (OpCode.Jump, condLabel);
                ec.EmitLabel(endLabel);
            }
        }

		public override bool AlwaysReturns {
			get {
				return false;
			}
		}

        public override string ToString()
        {
            if (IsDo)
            {
                return string.Format("do {1} while({0});", Condition, Loop);
            }
            else
            {
                return string.Format("while ({0}) {1};", Condition, Loop);
            }
        }
    }
}
