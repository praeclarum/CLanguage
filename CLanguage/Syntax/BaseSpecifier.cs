namespace CLanguage.Syntax
{
    public class BaseSpecifier
    {
        public string Name { get; }
        public DeclarationsVisibility? Visibility { get; }

        public BaseSpecifier (string name, DeclarationsVisibility? visibility = null)
        {
            Name = name;
            Visibility = visibility;
        }

        public override string ToString ()
        {
            return Visibility.HasValue ? $"{Visibility.Value.ToString ().ToLowerInvariant ()} {Name}" : Name;
        }
    }
}
