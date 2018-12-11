using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Syntax
{
    public class TranslationUnit : Block
    {
        public string Name { get; }

        public TranslationUnit (string name)
            : base (Compiler.VariableScope.Global)
        {
            if (string.IsNullOrWhiteSpace (name)) {
                throw new ArgumentException ("Translation unit name must be specified", nameof (name));
            }

            Name = name;
        }

        public override string ToString () => Name;
    }
}
