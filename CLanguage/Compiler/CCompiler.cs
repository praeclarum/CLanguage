using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Syntax;
using CLanguage.Types;
using CLanguage.Parser;
using CLanguage.Interpreter;
using System.Diagnostics;

namespace CLanguage.Compiler
{
    public class CCompiler
    {
        CompilerOptions options;

        readonly Dictionary<string, LexedDocument> lexedDocuments = new Dictionary<string, LexedDocument> ();

		List<TranslationUnit> tus;

        public CompilerOptions Options => options;

        public CCompiler ()
            : this (new CompilerOptions ())
        {
        }

        public CCompiler (CompilerOptions options)
		{
            this.options = options;
			tus = new List<TranslationUnit> ();

            ProcessDocument (new Document ("_machine.h", options.MachineInfo.GeneratedHeaderCode));
            foreach (var kv in options.MachineInfo.SystemHeadersCode) {
                ProcessDocument (new Document (kv.Key, kv.Value));
            }
            foreach (var d in options.Documents) {
                ProcessDocument (d);
            }
        }

		public CCompiler (MachineInfo mi, Report report)
			: this (new CompilerOptions (mi, report, noDocs))
        {
        }

        static readonly Document[] noDocs = new Document[0];

        public void Add (TranslationUnit translationUnit)
        {
            tus.Add (translationUnit);
        }

        void ProcessDocument (Document document)
        {
            var lexed = new LexedDocument (document, options.Report);
            lexedDocuments[document.Path] = lexed;

            if (document.IsCompilable) {
                var parser = new CParser ();

                var name = System.IO.Path.GetFileNameWithoutExtension (document.Path);

                Add (parser.ParseTranslationUnit (options.Report, name, Include, lexedDocuments["_machine.h"].Tokens, lexed.Tokens));
            }
        }

        Token[]? Include (string path, bool relative)
        {
            if (lexedDocuments.TryGetValue(path, out var doc)) {
                return doc.Tokens;
            }
            return null;
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
            try {
                return CompileExecutable ();
            }
            catch (Exception ex) {
                Debug.WriteLine (ex);
                options.Report.Error (9000, "Compiler error: " + ex.Message);
                return new Executable (options.MachineInfo);
            }
        }

        public static Executable Compile (string code)
        {
            var compiler = new Compiler.CCompiler ();
            compiler.AddCode ("main.c", code);
            var exe = compiler.Compile ();
            if (compiler.Options.Report.Errors.Any (x => x.IsError)) {
                var m = string.Join ("\n", compiler.Options.Report.Errors.Where (x => x.IsError));
                throw new ArgumentException (m, nameof (code));
            }
            return exe;
        }

        Executable CompileExecutable ()
        {
            var exe = new Executable (options.MachineInfo);
            var exeContext = new ExecutableContext (exe, options.Report);

            // Put something at the zero address so we don't get 0 addresses of globals
            exe.AddGlobal ("__zero__", CBasicType.SignedInt);

            //
            // Find Variables, Functions, Types
            //
            var exeInitBody = new Block (VariableScope.Local);
            var tucs = tus.Select (x => new TranslationUnitContext (x, exeContext));
            var tuInits = new List<(CompiledFunction, EmitContext)> ();
            foreach (var tuc in tucs) {
                var tu = tuc.TranslationUnit;
                AddStatementDeclarations (tuc);
                if (tu.InitStatements.Count > 0) {
                    var tuInitBody = new Block (VariableScope.Local);
                    tuInitBody.AddStatements (tu.InitStatements);
                    var tuInit = new CompiledFunction ($"__{tu.Name}__cinit", "", CFunctionType.VoidProcedure, tuInitBody);
                    exeInitBody.AddStatement (new ExpressionStatement (new FuncallExpression (new VariableExpression (tuInit.Name, Location.Null, Location.Null))));
                    tuInits.Add ((tuInit, tuc));
                    exe.Functions.Add (tuInit);
                }
            }

            //
            // Generate a function to init globals
            //
            var exeInit = new CompiledFunction ($"__cinit", "", CFunctionType.VoidProcedure, exeInitBody);
            exe.Functions.Add (exeInit);

            //
            // Link everything together
            // This is done before compilation to make sure everything is visible (for recursion)
            //
            var functionsToCompile = new List<(CompiledFunction, EmitContext)> { (exeInit, exeContext) };
            functionsToCompile.AddRange (tuInits);
            foreach (var tuc in tucs) {
                var tu = tuc.TranslationUnit;
                foreach (var g in tu.Variables) {
                    var v = exe.AddGlobal (g.Name, g.VariableType);
                    v.InitialValue = g.InitialValue;
                }
                var funcs = tu.Functions.Where (x => x.Body != null).ToList ();
                exe.Functions.AddRange (funcs);
                functionsToCompile.AddRange (funcs.Select (x => (x, (EmitContext)tuc)));
            }

            //
            // Compile functions
            //
            foreach (var (f, pc) in functionsToCompile) {
                var body = f.Body;
                if (body == null)
                    continue;
                var fc = new FunctionContext (exe, f, pc);
                AddStatementDeclarations (fc);
				body.Emit (fc);
				f.LocalVariables.AddRange (fc.LocalVariables);

				// Make sure it returns
				if (body.Statements.Count == 0 || !body.AlwaysReturns) {
					if (f.FunctionType.ReturnType.IsVoid) {
						fc.Emit (OpCode.Return);
					}
					else {
						options.Report.Error (161, "'" + f.Name + "' not all code paths return a value");
					}
				}
			}

			return exe;
		}

        void AddStatementDeclarations (BlockContext context)
        {
            var block = context.Block;
            foreach (var s in block.Statements) {
                s.AddDeclarationToBlock (context);
            }
        }

        FunctionDeclarator? GetFunctionDeclarator (Declarator? d)
        {
            if (d == null) return null;
            else if (d is FunctionDeclarator) return (FunctionDeclarator)d;
            else return GetFunctionDeclarator (d.InnerDeclarator);
        }

        
    }
}
