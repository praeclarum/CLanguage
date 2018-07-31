using System;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
    public class CompiledVariable
    {
        public string Name { get; }
        public CType VariableType { get; }

        public CompiledVariable (string name, CType type)
        {
            Name = name;
            VariableType = type;
        }

        public override string ToString ()
        {
            return string.Format ("{0}: {1}", Name, VariableType);
        }
    }
}
