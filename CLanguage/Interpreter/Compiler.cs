using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ValueType = System.Int32;

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
			exe.Globals.Add (new CompiledVariable ("__zero__", CBasicType.SignedInt));

            //
            // Find Variables, Functions, Types
            //
            foreach (var tu in tus) {
                AddStatementDeclarations (tu);
            }

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
            foreach (var tu in tus) {
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
                foreach (var idecl in multi.InitDeclarators) {
                    if ((multi.Specifiers.StorageClassSpecifier & StorageClassSpecifier.Typedef) != 0) {
                        if (idecl.Declarator != null) {
                            var name = idecl.Declarator.DeclaredIdentifier;
                            //Typedefs[name] = decl;
                        }
                    }
                    else {
                        var ctype = MakeCType (multi.Specifiers, idecl.Declarator);
                        var name = idecl.Declarator.DeclaredIdentifier;

                        if (ctype is CFunctionType && !HasStronglyBoundPointer (idecl.Declarator)) {
                            var ftype = (CFunctionType)ctype;
                            var f = new CompiledFunction (name, ftype);
                            block.Functions.Add (f);
                        }
                        else {
                            if ((ctype is CArrayType) &&
                                (((CArrayType)ctype).LengthExpression == null) &&
                                (idecl.Initializer != null)) {
                                if (idecl.Initializer is StructuredInitializer) {
                                    var atype = (CArrayType)ctype;
                                    var len = 0;
                                    foreach (var i in ((StructuredInitializer)idecl.Initializer).Initializers) {
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
                                    atype.LengthExpression = new ConstantExpression (len);
                                }
                                else {
                                    //Report.Error();
                                }
                            }
                            //var init = GetInitExpression(idecl.Initializer);
                            var vdecl = new CompiledVariable (name, ctype);
                            block.Variables.Add (vdecl);
                        }

                        if (idecl.Initializer != null) {
                            var varExpr = new VariableExpression (name);
                            var initExpr = GetInitializerExpression (idecl.Initializer);
                            block.InitStatements.Add (new ExpressionStatement (new AssignExpression (varExpr, initExpr)));
                        }
                    }
                }
            }
            else if (statement is FunctionDefinition fdef) {
                var ftype = (CFunctionType)MakeCType (fdef.Specifiers, fdef.Declarator);
                var name = fdef.Declarator.DeclaredIdentifier;
                var f = new CompiledFunction (name, ftype, fdef.Body);
                block.Functions.Add (f);
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

        CType MakeCType (DeclarationSpecifiers specs, Declarator decl)
        {
            var type = MakeCType (specs);
            return MakeCType (type, decl);
        }

        CType MakeCType (CType type, Declarator decl)
        {
            if (decl is IdentifierDeclarator) {
                // This is the name
            }
            else if (decl is PointerDeclarator) {
                var pdecl = (PointerDeclarator)decl;
                var isPointerToFunc = false;

                if (pdecl.StrongBinding) {
                    type = MakeCType (type, pdecl.InnerDeclarator);
                    isPointerToFunc = type is CFunctionType;
                }

                var p = pdecl.Pointer;
                while (p != null) {
                    type = new CPointerType (type);
                    type.TypeQualifiers = p.TypeQualifiers;
                    p = p.NextPointer;
                }

                if (!pdecl.StrongBinding) {
                    type = MakeCType (type, pdecl.InnerDeclarator);
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
                    type = new CArrayType (type, adecl.LengthExpression);
                    adecl = adecl.InnerDeclarator as ArrayDeclarator;
                    if (adecl != null && adecl.InnerDeclarator != null) {
                        if (adecl.InnerDeclarator is IdentifierDeclarator) {
                        }
                        else if (!(adecl.InnerDeclarator is ArrayDeclarator)) {
                            type = MakeCType (type, adecl.InnerDeclarator);
                        }
                        else {
                            //throw new NotSupportedException("Unrecognized array syntax");
                        }
                    }
                }
            }
            else if (decl is FunctionDeclarator) {
                type = MakeCFunctionType (type, decl);
            }

            return type;
        }

        private CType MakeCFunctionType (CType type, Declarator decl)
        {
            var fdecl = (FunctionDeclarator)decl;

            var name = decl.DeclaredIdentifier;
            var returnType = type;
            var ftype = new CFunctionType (returnType);
            foreach (var pdecl in fdecl.Parameters) {
                var pt = MakeCType (pdecl.DeclarationSpecifiers, pdecl.Declarator);
                if (!pt.IsVoid) {
                    ftype.Parameters.Add (new CFunctionType.Parameter (pdecl.Name, pt));
                }
            }
            type = ftype;

            type = MakeCType (type, fdecl.InnerDeclarator);

            return type;
        }

        CType MakeCType (DeclarationSpecifiers specs)
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
                    var type = typeName == "float" ? (CBasicType)CBasicType.Float : (typeName == "double" ? (CBasicType)CBasicType.Double : new CIntType (typeName, sign, size));
                    type.TypeQualifiers = specs.TypeQualifiers;
                    return type;
                }
            }

            //
            // Rest
            //
            throw new NotImplementedException ();
        }


        class FunctionContext : EmitContext
        {
			Executable exe;
			CompiledFunction fexe;
			EmitContext context;

			class BlockLocals
			{
				public int StartIndex;
				public int Length;
			}
			List<Block> blocks;
			Dictionary<Block, BlockLocals> blockLocals;
			List<CompiledVariable> allLocals;

			public IEnumerable<CompiledVariable> LocalVariables { get { return allLocals; } }

			public FunctionContext (Executable exe, CompiledFunction fexe, EmitContext context)
                : base (context.MachineInfo, context.Report, fexe)
            {
				this.exe = exe;
				this.fexe = fexe;
				this.context = context;
				blocks = new List<Block> ();
				blockLocals = new Dictionary<Block, BlockLocals> ();
                allLocals = new List<CompiledVariable> ();
            }

            public override ResolvedVariable ResolveVariable (string name)
			{
				//
				// Look for function parameters
				//
                for (var i = 0; i < fexe.FunctionType.Parameters.Count; i++) {
                    if (fexe.FunctionType.Parameters[i].Name == name) {
                        return new ResolvedVariable (VariableScope.Arg, i, fexe.FunctionType.Parameters[i].ParameterType);
					}
				}

				//
				// Look for locals
				//
				foreach (var b in blocks.Reverse<Block> ()) {
					var blocals = blockLocals[b];
					for (var i = 0; i < blocals.Length; i++) {
						var j = blocals.StartIndex + i;
						if (allLocals[j].Name == name) {
							return new ResolvedVariable (VariableScope.Local, j, allLocals[j].VariableType);
						}
					}
				}

				//
				// Look for global variables
				//
				for (var i = 0; i < exe.Globals.Count; i++) {
					if (exe.Globals[i].Name == name) {
						return new ResolvedVariable (VariableScope.Global, i, exe.Globals[i].VariableType);
					}
				}

				//
				// Look for functions
				//
				for (var i = 0; i < exe.Functions.Count; i++) {
					var f = exe.Functions[i];
					if (f.Name == name) {
						return new ResolvedVariable (f, i);
					}
				}

				context.Report.Error (103, "The name '" + name + "' does not exist in the current context");
				return null;
			}

			public override void BeginBlock (Block b)
			{
				blocks.Add (b);
				var locs = new BlockLocals {
					StartIndex = allLocals.Count,
					Length = b.Variables.Count,
				};
				blockLocals[b] = locs;
				allLocals.AddRange (b.Variables);
			}

			public override void EndBlock ()
			{
				blocks.RemoveAt (blocks.Count - 1);
			}

			public override Label DefineLabel ()
			{
				return new Label ();
			}

            public override void EmitLabel(Label l)
            {
				l.Index = fexe.Instructions.Count;
            }

			public override void Emit (Instruction instruction)
			{
				fexe.Instructions.Add (instruction);
			}
        }
    }
}
