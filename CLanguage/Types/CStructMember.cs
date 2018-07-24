namespace CLanguage.Types
{
    public abstract class CStructMember
    {
    }

    public class CStructField : CStructMember
    {
        public CType FieldType { get; set; }
        public string Name { get; set; }
    }
}