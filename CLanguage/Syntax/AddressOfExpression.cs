using System;
using CLanguage.Interpreter;
using CLanguage.Types;

namespace CLanguage.Syntax
{
    public class AddressOfExpression : Expression
    {
        public Expression InnerExpression { get; }

        public AddressOfExpression (Expression innerExpression)
        {
            InnerExpression = innerExpression;
        }

        public override CType GetEvaluatedCType (EmitContext ec)
        {
            return InnerExpression.GetEvaluatedCType (ec).Pointer;
        }

        protected override void DoEmit (EmitContext ec)
        {
            InnerExpression.EmitPointer (ec);
        }
    }
}
