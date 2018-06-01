namespace CLanguage.Syntax
{
    public class TypeSpecifier
    {
        public TypeSpecifierKind Kind { get; private set; }
        public string Name { get; private set; }
        public Block Body { get; private set; }

        public TypeSpecifier (TypeSpecifierKind kind, string name, Block body = null)
        {
            Kind = kind;
            Name = name;
            Body = body;
        }

        public override string ToString ()
        {
            return Name;
        }
    }

    public enum TypeSpecifierKind
    {
        Builtin,
        Typename,
        Struct,
        Class,
        Union,
        Enum
    }
}
