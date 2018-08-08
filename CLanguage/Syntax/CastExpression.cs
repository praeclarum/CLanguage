using System;
using CLanguage.Interpreter;
using CLanguage.Types;

namespace CLanguage.Syntax
{
    public class CastExpression : Expression
    {
        public TypeName TypeName { get; }
        public Expression InnerExpression { get; }

        public CastExpression (TypeName typeName, Expression innerExpression)
        {
            TypeName = typeName;
            InnerExpression = innerExpression;
        }

        public override CType GetEvaluatedCType (EmitContext ec)
        {
            return ec.ResolveTypeName (TypeName) ?? CBasicType.SignedInt;
        }

        protected override void DoEmit (EmitContext ec)
        {
            var rtype = GetEvaluatedCType (ec);
            var itype = InnerExpression.GetEvaluatedCType (ec);
            ec.EmitCast (itype, rtype);
        }
    }
}
