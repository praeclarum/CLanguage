using System;
using CLanguage.Interpreter;
using CLanguage.Syntax;

namespace CLanguage.Types
{
    public class CArrayType : CType
    {
        public CType ElementType { get; }
        public int? Length { get; }

        public CArrayType (CType elementType, int? length)
        {
            ElementType = elementType;
            Length = length;
        }

        public override int GetByteSize (EmitContext c)
        {
            if (Length == null) return c.MachineInfo.PointerSize;
            var innerSize = ElementType.GetByteSize (c);
            return Length.Value * innerSize;
        }

        public override string ToString ()
        {
            return string.Format ("{0}[{1}]", ElementType, Length);
        }
    }
}
