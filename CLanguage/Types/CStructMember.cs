namespace CLanguage.Types
{
    public abstract class CStructMember
    {
        public string Name { get; set; }
        public CType MemberType { get; set; }
    }

    public class CStructField : CStructMember
    {
    }

    public class CStructMethod : CStructMember
    {
    }
}