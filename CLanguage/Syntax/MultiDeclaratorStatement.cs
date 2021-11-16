using System;
using System.Collections.Generic;
using CLanguage.Interpreter;
using CLanguage.Types;
using CLanguage.Compiler;
using System.Diagnostics;

namespace CLanguage.Syntax
{
    public class MultiDeclaratorStatement : Statement
    {
        public DeclarationSpecifiers Specifiers;
        public List<InitDeclarator>? InitDeclarators;

        public override bool AlwaysReturns => false;

        public MultiDeclaratorStatement (DeclarationSpecifiers specifiers, List<InitDeclarator>? initDeclarators)
        {
            Specifiers = specifiers;
            InitDeclarators = initDeclarators;
        }

        public override string ToString ()
        {
            return string.Join (" ", Specifiers.TypeSpecifiers) + (InitDeclarators != null ? " " + string.Join (", ", InitDeclarators) : "");
        }

        protected override void DoEmit (EmitContext ec)
        {
            var multi = this;
            if (multi.InitDeclarators != null) {
                foreach (var idecl in multi.InitDeclarators) {
                    if ((multi.Specifiers.StorageClassSpecifier & StorageClassSpecifier.Typedef) != 0) {
                    }
                    else {
                        if (idecl.Initializer != null) {
                            var name = idecl.Declarator.DeclaredIdentifier;
                            var varExpr = new VariableExpression (name, Location.Null, Location.Null);
                            var initExpr = GetInitializerExpression (idecl.Initializer);
                            new ExpressionStatement (new AssignExpression (varExpr, initExpr)).Emit (ec);
                        }

                    }
                }
            }
        }

        public override void AddDeclarationToBlock (BlockContext context)
        {
            var multi = this;
            var block = context.Block;
            if (multi.InitDeclarators != null) {
                foreach (var idecl in multi.InitDeclarators) {
                    if ((multi.Specifiers.StorageClassSpecifier & StorageClassSpecifier.Typedef) != 0) {
                        var name = idecl.Declarator.DeclaredIdentifier;
                        var ttype = context.MakeCType (multi.Specifiers, idecl.Declarator, idecl.Initializer, block);
                        block.Typedefs[name] = ttype;
                    }
                    else {
                        CType ctype = context.MakeCType (multi.Specifiers, idecl.Declarator, idecl.Initializer, block);
                        var name = idecl.Declarator.DeclaredIdentifier;

                        if (ctype is CFunctionType ftype && !HasStronglyBoundPointer (idecl.Declarator)) {

                            // Ctors look like function definitions
                            if (ftype.ReturnType is CStructType ctorDeclType && idecl.Initializer == null && idecl.Declarator is FunctionDeclarator ctorDecl && ctorDecl.CouldBeCtorCall) {
                                //var varExpr = new VariableExpression (name, Location.Null, Location.Null);
                                block.AddVariable (name, ctorDeclType);
                                //throw new NotImplementedException ("Can't call ctors yet");
                                //block.InitStatements.Add (new ExpressionStatement (new AssignExpression (varExpr, initExpr)));
                            }
                            else {

                                var nameContext = (idecl.Declarator.InnerDeclarator is IdentifierDeclarator ndecl && ndecl.Context.Count > 0) ?
                                    string.Join ("::", ndecl.Context) : "";
                                var f = new CompiledFunction (name, nameContext, ftype);
                                block.Functions.Add (f);
                            }
                        }
                        else {
                            if ((ctype is CArrayType atype) &&
                                (atype.Length == null) &&
                                (idecl.Initializer != null)) {
                                if (idecl.Initializer is StructuredInitializer structInit) {
                                    var len = 0;
                                    foreach (var i in structInit.Initializers) {
                                        if (i.Designation == null) {
                                            len++;
                                        }
                                        else {
                                            foreach (var de in i.Designation.Designators) {
                                                // TODO: Pay attention to designators
                                                len++;
                                            }
                                        }
                                    }
                                    atype = new CArrayType (atype.ElementType, len);
                                }
                                else {
                                    //Report.Error();
                                }
                            }
                            //var init = GetInitExpression(idecl.Initializer);
                            block.AddVariable (name, ctype ?? CBasicType.SignedInt);
                        }

                        if (idecl.Initializer != null) {
                            var varExpr = new VariableExpression (name, Location.Null, Location.Null);
                            var initExpr = GetInitializerExpression (idecl.Initializer);
                            block.InitStatements.Add (new ExpressionStatement (new AssignExpression (varExpr, initExpr)));
                        }
                    }
                }
            }
            else {
                var ctype = context.MakeCType (multi.Specifiers, null, block);
                if (ctype is CStructType structType) {
                    var n = structType.Name;
                    if (!string.IsNullOrEmpty (n)) {
                        block.Structures[n] = structType;
                    }
                }
                else if (ctype is CEnumType enumType) {
                    var n = enumType.Name;
                    if (string.IsNullOrEmpty (n)) {
                        n = "e" + enumType.GetHashCode ();
                    }
                    block.Enums[n] = enumType;
                }
            }
        }

        static Expression GetInitializerExpression (Initializer init)
        {
            if (init is ExpressionInitializer) {
                return ((ExpressionInitializer)init).Expression;
            }
            else if (init is StructuredInitializer) {
                var sinit = (StructuredInitializer)init;

                var sexpr = new StructureExpression ();

                foreach (var i in sinit.Initializers) {
                    var e = GetInitializerExpression (i);

                    if (i.Designation == null || i.Designation.Designators.Count == 0) {
                        var ie = new StructureExpression.Item (null, GetInitializerExpression (i));
                        sexpr.Items.Add (ie);
                    }
                    else {

                        foreach (var d in i.Designation.Designators) {
                            var ie = new StructureExpression.Item (d.ToString (), e);
                            sexpr.Items.Add (ie);
                        }
                    }
                }

                return sexpr;
            }
            else {
                throw new NotSupportedException (init.ToString ());
            }
        }

        static bool HasStronglyBoundPointer (Declarator? d)
        {
            if (d == null)
                return false;
            else if (d is PointerDeclarator && ((PointerDeclarator)d).StrongBinding)
                return true;
            else
                return HasStronglyBoundPointer (d.InnerDeclarator);
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
        public Initializer? Initializer;

        public InitDeclarator (Declarator declarator, Initializer? initializer)
        {
            Declarator = declarator;
            Initializer = initializer;
        }

        public override string ToString ()
        {
            return Declarator.ToString ();
        }
    }
}
