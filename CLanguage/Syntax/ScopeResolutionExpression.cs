using CLanguage.Compiler;
using CLanguage.Types;

namespace CLanguage.Syntax
{
    public class ScopeResolutionExpression : Expression
    {
        public string TypeName { get; }
        public string MemberName { get; }

        public ScopeResolutionExpression (string typeName, string memberName)
        {
            TypeName = typeName;
            MemberName = memberName;
        }

        public override CType GetEvaluatedCType (EmitContext ec)
        {
            var r = ec.TryResolveQualifiedFunction (TypeName, MemberName, null);
            if (r != null)
                return r.VariableType;
            return CBasicType.SignedInt;
        }

        protected override void DoEmit (EmitContext ec)
        {
            var r = ec.TryResolveQualifiedFunction (TypeName, MemberName, null);
            if (r != null) {
                r.Emit (ec);
            }
            else {
                ec.Report.Error (103, $"'{TypeName}::{MemberName}' not found");
                ec.Emit (Interpreter.OpCode.LoadConstant, 0);
            }
        }

        public override string ToString () => $"{TypeName}::{MemberName}";
    }
}
