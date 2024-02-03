using System;
using CLanguage.Compiler;
using CLanguage.Interpreter;
using CLanguage.Types;

namespace CLanguage.Syntax
{
    public class SizeOfTypeExpression : Expression
    {
        public TypeName TypeName { get; }

        public SizeOfTypeExpression (TypeName typeName)
        {
            TypeName = typeName;
        }

        public override CType GetEvaluatedCType (EmitContext ec)
        {
            return CBasicType.UnsignedLongInt;
        }

        protected override void DoEmit (EmitContext ec)
        {
            var type = ec.ResolveTypeName (TypeName);
            Value cval = type.NumValues;
            ec.Emit (OpCode.LoadConstant, cval);
        }
    }
}
