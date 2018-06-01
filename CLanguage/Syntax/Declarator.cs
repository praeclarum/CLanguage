using System.Collections.Generic;
using CLanguage.Types;

namespace CLanguage.Syntax
{
    public abstract class Declarator
    {
        public abstract string DeclaredIdentifier { get; }
        public bool StrongBinding { get; set; }
        public Declarator InnerDeclarator { get; set; }
    }

    public class IdentifierDeclarator : Declarator
    {
        public string Identifier { get; private set; }

        public override string DeclaredIdentifier {
            get {
                return Identifier;
            }
        }

        public IdentifierDeclarator (string id)
        {
            Identifier = id;
        }

        public override string ToString ()
        {
            return Identifier;
        }
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

    public class FunctionDeclarator : Declarator
    {
        public List<ParameterDeclaration> Parameters { get; set; }

        public override string DeclaredIdentifier {
            get {
                return (InnerDeclarator != null) ? InnerDeclarator.DeclaredIdentifier : "";
            }
        }
    }

    public class Pointer
    {
        public TypeQualifiers TypeQualifiers { get; set; }
        public Pointer NextPointer { get; set; }

        public Pointer (TypeQualifiers qual, Pointer p)
        {
            TypeQualifiers = qual;
            NextPointer = p;
        }

        public Pointer (TypeQualifiers qual)
        {
            TypeQualifiers = qual;
        }
    }

    public class PointerDeclarator : Declarator
    {
        public Pointer Pointer { get; private set; }

        public override string DeclaredIdentifier {
            get {
                return (InnerDeclarator != null) ? InnerDeclarator.DeclaredIdentifier : "";
            }
        }

        public PointerDeclarator (Pointer pointer, Declarator decl)
        {
            Pointer = pointer;
            InnerDeclarator = decl;
        }
    }
}
