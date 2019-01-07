using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Types;

namespace CLanguage.Syntax
{
    public class ParameterDeclaration
    {
        public string Name { get; private set; }
        public DeclarationSpecifiers? DeclarationSpecifiers { get; private set; }
        public Declarator? Declarator { get; private set; }
        public Expression? DefaultValue { get; }

        public ParameterDeclaration (string name)
        {
            Name = name;
        }

        public ParameterDeclaration (DeclarationSpecifiers specs)
        {
            DeclarationSpecifiers = specs;
            Name = "";
        }

        public ParameterDeclaration (DeclarationSpecifiers specs, Declarator dec)
        {
            DeclarationSpecifiers = specs;
            Name = dec.DeclaredIdentifier;
            Declarator = dec;
        }

        public ParameterDeclaration (DeclarationSpecifiers specs, Declarator dec, Expression defaultValue)
        {
            DeclarationSpecifiers = specs;
            Name = dec.DeclaredIdentifier;
            Declarator = dec;
            DefaultValue = defaultValue;
        }

        public override string ToString ()
        {
            return DeclarationSpecifiers + " " + Declarator;
        }
    }

    public class VarParameter : ParameterDeclaration
    {
        public VarParameter ()
            : base ("...")
        {
        }
    }



}
