namespace CLanguage.Types
{
    public abstract class CStructMember
    {
        public string Name { get; set; } = "";
        public CType MemberType { get; set; } = CBasicType.SignedInt;

        public override string ToString () => $"{MemberType} {Name}";
    }

    public class CStructField : CStructMember
    {
    }

    public class CStructMethod : CStructMember
    {
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
        public bool IsPureVirtual { get; set; }
        public int? VTableSlotIndex { get; set; }
    }
}
