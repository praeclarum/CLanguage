using CLanguage.Compiler;

namespace CLanguage.Types
{
    public class CReferenceType : CType
    {
        public CType InnerType { get; }

        public CReferenceType (CType innerType)
        {
            InnerType = innerType;
        }

        public override int NumValues => 1; // Stored as a pointer (single Value slot)

        public override int GetByteSize (EmitContext ec) => ec.MachineInfo.PointerSize;

        public override int ScoreCastTo (CType otherType)
        {
            // Reference to same type: perfect match
            if (otherType is CReferenceType otherRef && InnerType.Equals (otherRef.InnerType))
                return 1000;
            // Reference can implicitly convert to the inner type (deref)
            if (InnerType.Equals (otherType))
                return 900;
            // Reference can convert to pointer to same type
            if (otherType is CPointerType pt && InnerType.Equals (pt.InnerType))
                return 800;
            return InnerType.ScoreCastTo (otherType);
        }

        public override bool Equals (object? obj) =>
            obj is CReferenceType other && InnerType.Equals (other.InnerType);

        public override int GetHashCode () => InnerType.GetHashCode () ^ 0x5A5A;

        public override string ToString () => $"{InnerType}&";
    }
}
