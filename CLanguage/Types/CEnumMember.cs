using System;
using CLanguage.Syntax;

namespace CLanguage.Types
{
    public class CEnumMember
    {
        public string Name { get; }
        public int Value { get; }

        public CEnumMember (string name, int value)
        {
            Name = name ?? throw new ArgumentNullException (nameof (name));
            Value = value;
        }

        public override string ToString ()
        {
            return $"{Name} = {Value}";
        }
    }
}
