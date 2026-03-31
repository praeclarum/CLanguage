using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Interpreter;
using CLanguage.Types;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class MemberFromReferenceExpression : Expression
    {
        public Expression Left { get; private set; }
        public string MemberName { get; private set; }

        public override bool CanEmitPointer => true;

        public MemberFromReferenceExpression(Expression left, string memberName)
        {
            Left = left;
            MemberName = memberName;
        }

        static CStructMember? FindMember (CStructType structType, string name)
        {
            return structType.FindMember (name);
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
            var targetType = Left.GetEvaluatedCType (ec);

            if (targetType is CStructType structType) {

                var member = FindMember (structType, MemberName);
                if (member == null) {
                    ec.Report.Error (1061, "'{1}' not found in '{0}'", structType.Name, MemberName);
                    return CBasicType.SignedInt;
                }

                return member.MemberType;
            }
            else {
                throw new NotImplementedException ("Member type on " + targetType?.GetType ().Name);
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            var targetType = Left.GetEvaluatedCType (ec);

            if (targetType is CStructType structType) {

                var member = FindMember (structType, MemberName);

                if (member == null) {
                    ec.Report.Error (1061, "'{1}' not found in '{0}'", structType.Name, MemberName);
                }
                else {
                    if (member is CStructMethod method && member.MemberType is CFunctionType functionType) {
                        if (method.VTableSlotIndex.HasValue && structType.VTableGlobalAddress.HasValue) {
                            // Virtual dispatch path
                            Left.EmitPointer (ec);
                            ec.Emit (OpCode.Dup);
                            ec.Emit (OpCode.LoadPointer);
                            ec.Emit (OpCode.LoadConstant, Value.Pointer (method.VTableSlotIndex.Value));
                            ec.Emit (OpCode.OffsetPointer);
                            ec.Emit (OpCode.LoadPointer);
                        }
                        else {
                            // Non-virtual direct path
                            var res = ec.ResolveMethodFunction (structType, method);
                            if (res != null) {
                                Left.EmitPointer (ec);
                                ec.Emit (OpCode.LoadConstant, Value.Pointer (res.Address));
                            }
                        }
                    }
                    else {
                        if (Left.CanEmitPointer) {
                            Left.EmitPointer (ec);
                        }
                        else {
                            // Left is an rvalue (e.g., function call returning struct).
                            // Allocate a temp, store the struct there, then use its address.
                            var tempOffset = ec.AllocateTemp (structType);
                            Left.Emit (ec);
                            var numValues = structType.NumValues;
                            for (int i = numValues - 1; i >= 0; i--) {
                                ec.Emit (OpCode.StoreLocal, tempOffset + i);
                            }
                            ec.Emit (OpCode.LoadConstant, Value.Pointer (tempOffset));
                            ec.Emit (OpCode.LoadFramePointer);
                            ec.Emit (OpCode.OffsetPointer);
                        }
                        ec.Emit (OpCode.LoadConstant, Value.Pointer (structType.GetFieldValueOffset(member, ec)));
                        ec.Emit (OpCode.OffsetPointer);
                        ec.Emit (OpCode.LoadPointer);
                    }
                }
            }
            else {
                throw new NotSupportedException ($"Cannot read '{MemberName}' on " + targetType?.GetType ().Name);
            }
        }

        protected override void DoEmitPointer (EmitContext ec)
        {
            var targetType = Left.GetEvaluatedCType (ec);

            if (targetType is CStructType structType) {

                var member = FindMember (structType, MemberName);

                if (member == null) {
                    ec.Report.Error (1061, "'{1}' not found in '{0}'", structType.Name, MemberName);
                }
                else {
                    if (member is CStructMethod method && member.MemberType is CFunctionType functionType) {
                        ec.Report.Error (1656, "Cannot assign to '{0}'", MemberName);
                    }
                    else {
                        Left.EmitPointer (ec);
                        ec.Emit (OpCode.LoadConstant, Value.Pointer (structType.GetFieldValueOffset (member, ec)));
                        ec.Emit (OpCode.OffsetPointer);
                    }
                }
            }
            else {
                throw new NotSupportedException ($"Cannot write '{MemberName}' on " + targetType?.GetType ().Name);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Left, MemberName);
        }
    }
}
