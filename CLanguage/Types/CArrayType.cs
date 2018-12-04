using System;
using CLanguage.Compiler;

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

        public override int NumValues {
            get {
                if (Length == null) return 1;
                var innerSize = ElementType.NumValues;
                return Length.Value * innerSize;
            }
        }

        public override int GetByteSize (EmitContext c)
        {
            if (Length == null) return c.MachineInfo.PointerSize;
            var innerSize = ElementType.GetByteSize (c);
            return Length.Value * innerSize;
        }

        public override int ScoreCastTo (CType otherType)
        {
            if (Equals (otherType)) return 1000;

            if (otherType is CPointerType pt) {
                if (ElementType.Equals (pt.InnerType)) {
                    return 900;
                }
                else {
                    return ElementType.ScoreCastTo (pt.InnerType) / 2;
                }
            }

            return 0;
        }

        public override bool Equals (object obj)
        {
            return obj is CArrayType a && Length == a.Length && ElementType.Equals (a.ElementType);
        }

        public override int GetHashCode ()
        {
            var hash = 17;
            hash = hash * 37 + ElementType.GetHashCode ();
            hash = hash * 37 + Length.GetHashCode ();
            return hash;
        }

        public override string ToString ()
        {
            return string.Format ("{0}[{1}]", ElementType, Length);
        }
    }
}
