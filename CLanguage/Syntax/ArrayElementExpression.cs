using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Types;
using CLanguage.Interpreter;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class ArrayElementExpression : Expression
    {
        public Expression Array { get; private set; }
        public Expression ElementIndex { get; private set; }

        public ArrayElementExpression (Expression array, Expression elementIndex)
        {
            Array = array;
            ElementIndex = elementIndex;
        }

        public override CType GetEvaluatedCType (EmitContext ec)
        {
            var t = Array.GetEvaluatedCType (ec);
            if (t is CArrayType a) {
                return a.ElementType;
            }
            else if (t is CPointerType p) {
                return p.InnerType;
            }
            else {
                ec.Report.Error (601, "Left hand side of [ must be an array or pointer");
                return CType.Void;
            }
        }
        public override CPointerType GetEvaluatedCPointerType (EmitContext ec)
        {
            var t = Array.GetEvaluatedCType (ec);
            if (t is CArrayType a) {
                return a.ElementType.Pointer;
            }
            else if (t is CPointerType p) {
                return p.InnerType.Pointer;
            }
            else {
                ec.Report.Error (601, "Left hand side of [ must be an array or pointer");
                return CPointerType.PointerToVoid;
            }
        }

        protected override void DoEmit (EmitContext ec)
        {
            DoEmitPointer (ec);
            ec.Emit (OpCode.LoadPointer);
        }

        public override string ToString ()
        {
            return string.Format ("{0}[{1}]", Array, ElementIndex);
        }

        public override bool CanEmitPointer => Array.CanEmitPointer;

        protected override void DoEmitPointer (EmitContext ec)
        {
            Array.Emit (ec);
            ElementIndex.Emit (ec);
            ec.Emit (OpCode.LoadConstant, GetEvaluatedCType (ec).NumValues);
            ec.Emit (OpCode.MultiplyInt32);
            ec.Emit (OpCode.OffsetPointer);
        }
    }
}
