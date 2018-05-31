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

    public enum TypeSpecifierKind
    {
        Builtin,
        Typename,
        Struct,
        Class,
        Union,
        Enum
    }

    public class TypeSpecifier
    {
        public TypeSpecifierKind Kind { get; private set; }
        public string Name { get; private set; }

        public TypeSpecifier (TypeSpecifierKind kind, string name)
        {
            Kind = kind;
            Name = name;
        }

        public override string ToString ()
        {
            return Name;
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

    public class FunctionDefinition
    {
        public DeclarationSpecifiers Specifiers { get; set; }
        public Declarator Declarator { get; set; }
        public List<Declaration> ParameterDeclarations { get; set; }
        public Block Body { get; set; }
    }

    public class ArrayDeclarator : Declarator
    {
        public Expression LengthExpression { get; set; }
        public TypeQualifiers TypeQualifiers { get; set; }
        public bool LengthIsStatic { get; set; }

        public override string DeclaredIdentifier {
            get {
                return (InnerDeclarator != null) ? InnerDeclarator.DeclaredIdentifier : "";
            }
        }
    }



}
