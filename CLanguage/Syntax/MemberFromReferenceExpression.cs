using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Interpreter;
using CLanguage.Types;

namespace CLanguage.Syntax
{
    public class MemberFromReferenceExpression : Expression
    {
        public Expression Left { get; private set; }
        public string MemberName { get; private set; }

        public MemberFromReferenceExpression(Expression left, string memberName)
        {
            Left = left;
            MemberName = memberName;
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
            var targetType = Left.GetEvaluatedCType (ec);

            if (targetType is CStructType structType) {

                var member = structType.Members.FirstOrDefault (x => x.Name == MemberName);
                if (member == null) {
                    ec.Report.Error (1061, "Struct '{0}' does not contain a defintion for '{1}'", structType.Name, MemberName);
                    return CBasicType.SignedInt;
                }

                return member.MemberType;
            }
            else {
                throw new NotImplementedException ("Member type on " + targetType.GetType ().Name);
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            var targetType = Left.GetEvaluatedCType (ec);

            if (targetType is CStructType structType) {

                var member = structType.Members.FirstOrDefault (x => x.Name == MemberName);

                if (member == null) {
                    ec.Report.Error (1061, "Struct '{0}' does not contain a definition for '{1}'", structType.Name, MemberName);
                }
                else {
                    if (member is CStructMethod method && member.MemberType is CFunctionType functionType) {
                        var res = ec.ResolveMethodFunction (structType, method);
                        if (res != null) {
                            Left.EmitPointer (ec);
                            ec.Emit (OpCode.LoadConstant, Value.FunctionPointer (res.Index));
                        }
                    }
                    else {
                        throw new NotSupportedException ("Member field access on struct " + structType.Name);
                    }
                }
            }
            else {
                throw new NotSupportedException ("Member access on " + targetType.GetType ().Name);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Left, MemberName);
        }
    }
}
