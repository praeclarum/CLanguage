using System;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
    public class CompiledType
    {
        public string Name { get; }
        public CType Type { get; }

        public CompiledType (string name, CType type)
        {
            Name = name;
        }

        public override string ToString ()
        {
            return string.Format ("{0}: {1}", Name, Type);
        }
    }
}
