using System;
using CLanguage.Interpreter;
using CLanguage.Types;

namespace CLanguage.Syntax
{
    public class DereferenceExpression : Expression
    {
        public Expression InnerExpression { get; }

        public DereferenceExpression (Expression innerExpression)
        {
            InnerExpression = innerExpression;
        }

        public override CType GetEvaluatedCType (EmitContext ec)
        {
            return InnerExpression.GetEvaluatedCType (ec);
        }

        protected override void DoEmit (EmitContext ec)
        {
            InnerExpression.Emit (ec);
            ec.Emit (OpCode.LoadMemory, 0);
            throw new NotImplementedException ();
        }
    }
}
