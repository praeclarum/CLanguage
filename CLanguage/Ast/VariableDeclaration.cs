using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Types;

namespace CLanguage.Ast
{
    public class VariableDeclaration : Declaration
    {
        public string Name { get; private set; }
        public CType VariableType { get; private set; }

        public VariableDeclaration(string name, CType type)
        {
            Name = name;
            VariableType = type;
        }

        protected override void DoEmit(EmitContext ec)
        {
            ec.DeclareVariable(this);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Name, VariableType);
        }
    }
}
