using System;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
    public class CompiledVariable
    {
        public string Name { get; private set; }
        public CType VariableType { get; private set; }

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
