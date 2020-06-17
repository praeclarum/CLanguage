using System;
using CLanguage.Interpreter;
using CLanguage.Types;
using CLanguage.Compiler;

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
            var it = InnerExpression.GetEvaluatedCType (ec);
            if (it is CPointerType pointerType) {
                return pointerType.InnerType;
            }
            else {
                ec.Report.Error (0, $"Cannot dereference values of type `{it}`.");
                return CBasicType.SignedInt;
            }
        }

        protected override void DoEmit (EmitContext ec)
        {
            InnerExpression.Emit (ec);
            ec.Emit (OpCode.LoadPointer);
        }

        public override bool CanEmitPointer => true;

        protected override void DoEmitPointer (EmitContext ec)
        {
            InnerExpression.Emit (ec);
        }
    }
}
