using System;
using System.Collections.Generic;
using CLanguage.Interpreter;
using CLanguage.Types;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class MultiDeclaratorStatement : Statement
    {
        public DeclarationSpecifiers Specifiers;
        public List<InitDeclarator> InitDeclarators;

        public override bool AlwaysReturns => false;

        protected override void DoEmit (EmitContext ec)
        {
        }

        public override string ToString ()
        {
            return string.Join (" ", Specifiers.TypeSpecifiers) + " " + string.Join (", ", InitDeclarators);
        }
    }

    [Flags]
    public enum StorageClassSpecifier
    {
        None = 0,
        Typedef = 1,
        Extern = 2,
        Static = 4,
        Auto = 8,
        Register = 16
    }

    public class DeclarationSpecifiers
    {
        public StorageClassSpecifier StorageClassSpecifier { get; set; }
        public List<TypeSpecifier> TypeSpecifiers { get; private set; }
        public FunctionSpecifier FunctionSpecifier { get; set; }
        public TypeQualifiers TypeQualifiers { get; set; }
        public DeclarationSpecifiers ()
        {
            TypeSpecifiers = new List<TypeSpecifier> ();
        }
        public override string ToString ()
        {
            return StorageClassSpecifier == StorageClassSpecifier.Auto ? "auto" : string.Join (" ", TypeSpecifiers);
        }
    }

    public class InitDeclarator
    {
        public Declarator Declarator;
        public Initializer Initializer;

        public override string ToString ()
        {
            return Declarator.ToString ();
        }
    }
}
