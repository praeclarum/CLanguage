using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Types;

namespace CLanguage.Syntax
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

    public class ParameterDecl
    {
        public string Name { get; private set; }
        public DeclarationSpecifiers DeclarationSpecifiers { get; private set; }
        public Declarator Declarator { get; private set; }

        public ParameterDecl (string name)
        {
            Name = name;
        }

        public ParameterDecl (DeclarationSpecifiers specs)
        {
            DeclarationSpecifiers = specs;
            Name = "";
        }

        public ParameterDecl (DeclarationSpecifiers specs, Declarator dec)
        {
            DeclarationSpecifiers = specs;
            Name = dec.DeclaredIdentifier;
            Declarator = dec;
        }
    }

    public class VarParameter : ParameterDecl
    {
        public VarParameter ()
            : base ("...")
        {
        }
    }



}
