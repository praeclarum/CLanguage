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
    }
}
