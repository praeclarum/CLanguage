using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Syntax;
using CLanguage.Types;
using CLanguage.Parser;

namespace CLanguage.Interpreter
{
    public class Compiler
    {
		EmitContext context;

		List<TranslationUnit> tus;

		public Compiler (EmitContext context)
		{
			this.context = context;
			tus = new List<TranslationUnit> ();
		}

		public Compiler (MachineInfo mi, Report report)
			: this (new EmitContext (mi, report))
        {
        }

        public void Add (TranslationUnit translationUnit)
        {
            tus.Add (translationUnit);
        }

        public void AddCode (string name, string code)
        {
            var pp = new Preprocessor (context.Report);
            pp.AddCode ("machine.h", context.MachineInfo.HeaderCode);
            pp.AddCode (name, code);
            var lexer = new Lexer (pp);
            var parser = new CParser ();
            Add (parser.ParseTranslationUnit (lexer));
        }

		public Executable Compile ()
		{
			var exe = new Executable (context.MachineInfo);

			// Put something at the zero address so we don't get 0 addresses of globals
			exe.AddGlobal ("__zero__", CBasicType.SignedInt);

            //
            // Find Variables, Functions, Types
            //
            var cinitBody = new Block ();
            foreach (var tu in tus) {
                AddStatementDeclarations (tu);
                cinitBody.Statements.AddRange (tu.InitStatements);
            }

            //
            // Generate a function to init globals
            //
            exe.Functions.Add (new CompiledFunction ("__cinit", CFunctionType.VoidProcedure, cinitBody));

            //
            // Link everything together
            // This is done before compilation to make sure everything is visible (for recursion)
            //
            foreach (var tu in tus) {
                exe.Globals.AddRange (tu.Variables);
                exe.Functions.AddRange (tu.Functions.Where (x => x.Body != null));
            }

            //
            // Compile functions
            //
            foreach (var f in exe.Functions.OfType<CompiledFunction> ()) {
                AddStatementDeclarations (f.Body);

				var c = new FunctionContext (exe, f, context);
				f.Body.Emit (c);
				f.LocalVariables.AddRange (c.LocalVariables);

				// Make sure it returns
				if (f.Body.Statements.Count == 0 || !f.Body.AlwaysReturns) {
					if (f.FunctionType.ReturnType.IsVoid) {
						c.Emit (OpCode.Return);
					}
					else {
						context.Report.Error (161, "'" + f.Name + "' not all code paths return a value");
					}
				}
			}

			return exe;
		}

        void AddStatementDeclarations (Block block)
        {
            foreach (var s in block.Statements) {
                AddStatementDeclarations (s, block);
            }
        }

        void AddStatementDeclarations (Statement statement, Block block)
        {
            if (statement is MultiDeclaratorStatement multi) {
                if (multi.InitDeclarators != null) {
                    foreach (var idecl in multi.InitDeclarators) {
                        if ((multi.Specifiers.StorageClassSpecifier & StorageClassSpecifier.Typedef) != 0) {
                            if (idecl.Declarator != null) {
                                var name = idecl.Declarator.DeclaredIdentifier;
                                //Typedefs[name] = decl;
                            }
                        }
                        else {
                            var ctype = MakeCType (multi.Specifiers, idecl.Declarator, block);
                            var name = idecl.Declarator.DeclaredIdentifier;

                            if (ctype is CFunctionType ftype && !HasStronglyBoundPointer (idecl.Declarator)) {
                                var nameContext = (idecl.Declarator.InnerDeclarator is IdentifierDeclarator ndecl && ndecl.Context.Count > 0) ?
                                    string.Join ("::", ndecl.Context) : "";
                                var f = new CompiledFunction (name, nameContext, ftype);
                                block.Functions.Add (f);
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
                                block.AddVariable (name, ctype);
                            }

                            if (idecl.Initializer != null) {
                                var varExpr = new VariableExpression (name);
                                var initExpr = GetInitializerExpression (idecl.Initializer);
                                block.InitStatements.Add (new ExpressionStatement (new AssignExpression (varExpr, initExpr)));
                            }
                        }
                    }
                }
                else {
                    var ctype = MakeCType (multi.Specifiers, block);
                    if (ctype is CStructType structType) {
                        var n = structType.Name;
                        if (!string.IsNullOrEmpty (n)) {
                            block.Structures[n] = structType;
                        }
                    }
                }
            }
            else if (statement is FunctionDefinition fdef) {
                if (MakeCType (fdef.Specifiers, fdef.Declarator, block) is CFunctionType ftype) {
                    var name = fdef.Declarator.DeclaredIdentifier;
                    var f = new CompiledFunction (name, ftype, fdef.Body);
                    block.Functions.Add (f);
                }
            }
            else if (statement is ForStatement fors) {
                AddStatementDeclarations (fors.InitBlock);
            }
            else if (statement is Block subblock) {
                AddStatementDeclarations (subblock);
            }
        }

        Expression GetInitializerExpression (Initializer init)
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
                        var ie = new StructureExpression.Item ();
                        ie.Expression = GetInitializerExpression (i);
                        sexpr.Items.Add (ie);
                    }
                    else {

                        foreach (var d in i.Designation.Designators) {
                            var ie = new StructureExpression.Item ();
                            ie.Field = d.ToString ();
                            ie.Expression = e;
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

        FunctionDeclarator GetFunctionDeclarator (Declarator d)
        {
            if (d == null) return null;
            else if (d is FunctionDeclarator) return (FunctionDeclarator)d;
            else return GetFunctionDeclarator (d.InnerDeclarator);
        }

        bool HasStronglyBoundPointer (Declarator d)
        {
            if (d == null) return false;
            else if (d is PointerDeclarator && ((PointerDeclarator)d).StrongBinding) return true;
            else return HasStronglyBoundPointer (d.InnerDeclarator);
        }

        CType MakeCType (DeclarationSpecifiers specs, Declarator decl, Block block)
        {
            var type = MakeCType (specs, block);
            return MakeCType (type, decl, block);
        }

        CType MakeCType (CType type, Declarator decl, Block block)
        {
            if (decl is IdentifierDeclarator) {
                // This is the name
            }
            else if (decl is PointerDeclarator) {
                var pdecl = (PointerDeclarator)decl;
                var isPointerToFunc = false;

                if (pdecl.StrongBinding) {
                    type = MakeCType (type, pdecl.InnerDeclarator, block);
                    isPointerToFunc = type is CFunctionType;
                }

                var p = pdecl.Pointer;
                while (p != null) {
                    type = new CPointerType (type);
                    type.TypeQualifiers = p.TypeQualifiers;
                    p = p.NextPointer;
                }

                if (!pdecl.StrongBinding) {
                    type = MakeCType (type, pdecl.InnerDeclarator, block);
                }

                //
                // Remove 1 level of pointer indirection if this is
                // a pointer to a function since functions are themselves
                // pointers
                //
                if (isPointerToFunc) {
                    type = ((CPointerType)type).InnerType;
                }
            }
            else if (decl is ArrayDeclarator) {
                var adecl = (ArrayDeclarator)decl;

                while (adecl != null) {
                    int? len = null;
                    if (adecl.LengthExpression is ConstantExpression clen) {
                        len = clen.EmitValue.Int32Value;
                    }
                    else {
                        context.Report.Error (2057, "Expected constant expression");
                        len = 0;
                    }
                    type = new CArrayType (type, len);
                    adecl = adecl.InnerDeclarator as ArrayDeclarator;
                    if (adecl != null && adecl.InnerDeclarator != null) {
                        if (adecl.InnerDeclarator is IdentifierDeclarator) {
                        }
                        else if (!(adecl.InnerDeclarator is ArrayDeclarator)) {
                            type = MakeCType (type, adecl.InnerDeclarator, block);
                        }
                        else {
                            //throw new NotSupportedException("Unrecognized array syntax");
                        }
                    }
                }
            }
            else if (decl is FunctionDeclarator) {
                type = MakeCFunctionType (type, decl, block);
            }

            return type;
        }

        CType MakeCFunctionType (CType returnType, Declarator decl, Block block)
        {
            var fdecl = (FunctionDeclarator)decl;

            bool isInstance = decl.InnerDeclarator is IdentifierDeclarator ident && ident.Context.Count > 0;

            var name = decl.DeclaredIdentifier;
            var ftype = new CFunctionType (returnType, isInstance);
            foreach (var pdecl in fdecl.Parameters) {
                var pt = MakeCType (pdecl.DeclarationSpecifiers, pdecl.Declarator, block);
                if (!pt.IsVoid) {
                    ftype.AddParameter (pdecl.Name, pt);
                }
            }

            var type = MakeCType (ftype, fdecl.InnerDeclarator, block);

            return type;
        }

        CType MakeCType (DeclarationSpecifiers specs, Block block)
        {
            //
            // Try for Basic. The TypeSpecifiers are recorded in reverse from what is actually declared
            // in code.
            //
            var basicTs = specs.TypeSpecifiers.FirstOrDefault (x => x.Kind == TypeSpecifierKind.Builtin);
            if (basicTs != null) {
                if (basicTs.Name == "void") {
                    return CType.Void;
                }
                else {
                    var sign = Signedness.Signed;
                    var size = "";
                    TypeSpecifier trueTs = null;

                    foreach (var ts in specs.TypeSpecifiers) {
                        if (ts.Name == "unsigned") {
                            sign = Signedness.Unsigned;
                        }
                        else if (ts.Name == "signed") {
                            sign = Signedness.Signed;
                        }
                        else if (ts.Name == "short" || ts.Name == "long") {
                            if (size.Length == 0) size = ts.Name;
                            else size = size + " " + ts.Name;
                        }
                        else {
                            trueTs = ts;
                        }
                    }

                    var typeName = trueTs == null ? "int" : trueTs.Name;
                    var type = typeName == "float" ? (CBasicType)CBasicType.Float 
                        : (typeName == "double" ? (CBasicType)CBasicType.Double 
                           : (typeName == "bool" ? (CBasicType)CBasicType.Bool 
                              : new CIntType (typeName, sign, size)));
                    type.TypeQualifiers = specs.TypeQualifiers;
                    return type;
                }
            }

            //
            // Structs and Classes
            //
            var structTs = specs.TypeSpecifiers.FirstOrDefault (x => x.Kind == TypeSpecifierKind.Struct || x.Kind == TypeSpecifierKind.Class);
            if (structTs != null) {
                if (structTs.Body != null) {
                    var st = new CStructType ();
                    st.Name = structTs.Name;
                    foreach (var s in structTs.Body.Statements) {
                        AddStructMember (st, s, block);
                    }
                    return st;
                }
                else {
                    // Lookup
                    var name = structTs.Name;
                    if (block.Structures.TryGetValue (name, out var structType)) {
                        return structType;
                    }
                    else {
                        context.Report.Error (246, "The struct '{0}' could not be found", name);
                        return CBasicType.SignedInt;
                    }
                }
            }

            //
            // Rest
            //
            throw new NotImplementedException ();
        }

        void AddStructMember (CStructType st, Statement s, Block block)
        {
            if (s is MultiDeclaratorStatement multi) {
                if (multi.InitDeclarators != null) {
                    foreach (var i in multi.InitDeclarators) {
                        var type = MakeCType (multi.Specifiers, i.Declarator, block);
                        if (type is CFunctionType functionType) {
                            var name = i.Declarator.DeclaredIdentifier;
                            st.Members.Add (new CStructMethod { Name = name, MemberType = type });
                        }
                        else {
                            throw new NotSupportedException ($"Cannot add `{i}` to struct");
                        }
                    }
                }
            }
            else {
                throw new NotSupportedException ($"Cannot add statement `{s}` to struct");
            }
        }
    }
}
