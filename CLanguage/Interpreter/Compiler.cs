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

        CompilerOptions options;

        readonly Dictionary<string, LexedDocument> lexedDocuments = new Dictionary<string, LexedDocument> ();

		List<TranslationUnit> tus;

		public Compiler (CompilerOptions options)
		{
            this.options = options;
			this.context = new EmitContext (options.MachineInfo, options.Report);
			tus = new List<TranslationUnit> ();

            ProcessDocument (new Document ("_machine.h", options.MachineInfo.HeaderCode));
            foreach (var d in options.Documents) {
                ProcessDocument (d);
            }
        }

		public Compiler (MachineInfo mi, Report report)
			: this (new CompilerOptions (mi, report, Array.Empty<Document> ()))
        {
        }

        public void Add (TranslationUnit translationUnit)
        {
            tus.Add (translationUnit);
        }

        void ProcessDocument (Document document)
        {
            var lexed = new LexedDocument (document, options.Report);
            lexedDocuments[document.Path] = lexed;

            if (document.IsCompilable) {
                var pp = new Preprocessor (context.Report);
                pp.AddCode ("machine.h", context.MachineInfo.HeaderCode);
                pp.AddCode (document.Path, document.Content);
                var lexer = new Lexer (pp);
                var parser = new CParser ();
                Add (parser.ParseTranslationUnit (lexer));
            }
        }

        public void AddCode (string name, string code)
        {
            AddDocument (new Document (name, code));
        }

        public void AddDocument (Document document)
        {
            ProcessDocument (document);
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
                foreach (var g in tu.Variables) {
                    var v = exe.AddGlobal (g.Name, g.VariableType);
                    v.InitialValue = g.InitialValue;
                }
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
                            var ctype = context.MakeCType (multi.Specifiers, idecl.Declarator, idecl.Initializer, block);
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
                    var ctype = context.MakeCType (multi.Specifiers, null, block);
                    if (ctype is CStructType structType) {
                        var n = structType.Name;
                        if (!string.IsNullOrEmpty (n)) {
                            block.Structures[n] = structType;
                        }
                    }
                }
            }
            else if (statement is FunctionDefinition fdef) {
                if (context.MakeCType (fdef.Specifiers, fdef.Declarator, null, block) is CFunctionType ftype) {
                    var name = fdef.Declarator.DeclaredIdentifier;
                    var f = new CompiledFunction (name, ftype, fdef.Body);
                    block.Functions.Add (f);
                }
            }
            else if (statement is ForStatement fors) {
                AddStatementDeclarations (fors.InitBlock);
                AddStatementDeclarations (fors.LoopBody);
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
    }
}
