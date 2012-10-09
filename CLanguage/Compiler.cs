using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class Compiler
    {
		CompilerContext context;
		List<TranslationUnit> tus;

        public Compiler(Report report, MachineInfo m)
        {
            context = new CompilerContext (report, m);
			tus = new List<TranslationUnit> ();
        }

		public Executable Compile ()
		{
			var exe = new Executable ();

			foreach (var tu in tus) {
				foreach (var fdecl in tu.Functions) {
					var fexe = exe.Functions.FirstOrDefault (x => x.Name == fdecl.Name);
					if (fexe == null) {
						fexe = new Executable.Function (fdecl.Name);
						exe.Functions.Add (fexe);
					}
					if (fdecl.Body != null) {
						var c = new FunctionContext (fdecl, fexe, context);
						fdecl.Body.Emit (c);
					}
				}
			}

			return exe;
		}

        public void Add(TranslationUnit translationUnit)
        {
			tus.Add (translationUnit);
        }

        public void AddCode(string code)
        {
            var pp = new Preprocessor ();
            pp.AddCode("stdin", code);
            var lexer = new Lexer(pp);
            var parser = new CParser();
            Add(parser.ParseTranslationUnit(lexer, context.Report));
        }

        class FunctionContext : EmitContext
        {
			FunctionDeclaration fdecl;
			Executable.Function fexe;
			CompilerContext context;

			public FunctionContext (FunctionDeclaration fdecl, Executable.Function fexe, CompilerContext context)
                : base (fdecl, context.Report)
            {
				this.fdecl = fdecl;
				this.fexe = fexe;
				this.context = context;
            }

            public override ResolvedVariable ResolveVariable (string name)
			{
				for (var i = 0; i < fdecl.ParameterInfos.Count; i++) {
					if (fdecl.ParameterInfos[i].Name == name) {
						return new ResolvedVariable (VariableScope.Arg, i);
					}
				}
				return null;
			}

            public override void EmitBranchIfFalse(Label l)
            {
				fexe.Instructions.Add (new BranchIfFalseInstruction (l));
            }

            public override void EmitJump(Label l)
            {
				fexe.Instructions.Add (new JumpInstruction (l));
            }

			public override Label DefineLabel ()
			{
				return new Label ();
			}

            public override void EmitLabel(Label l)
            {
				l.Index = fexe.Instructions.Count;
            }

			public override void EmitReturn ()
			{
				fexe.Instructions.Add (new ReturnInstruction ());
			}

            public override void EmitUnop(Unop op)
            {
                Console.WriteLine("  " + op);
            }

            public override void EmitVariable (ResolvedVariable variable)
			{
				if (variable.Scope == VariableScope.Arg) {
					fexe.Instructions.Add (new LoadArgInstruction (variable.Index));
				}
				else {
					throw new NotImplementedException (variable.Scope.ToString ());
				}
            }

            public override void EmitBinop(Binop op)
            {
				fexe.Instructions.Add (new BinopInstruction (op));
            }

            public override void EmitCall(CFunctionType type, int argsCount)
            {
                Console.WriteLine("  -> " + type + "/" + argsCount);
				throw new NotImplementedException ();
            }

            public override void EmitConstant(object value, CType type)
            {
				fexe.Instructions.Add (new PushInstruction (value, type));
            }

            public override void EmitAssign(Expression left)
            {
                Console.WriteLine("  => " + left);
				//_body.Add (Ops.Assign ());
				throw new NotImplementedException ();
            }

            public override void EmitPop()
            {
            }
        }

        class TranslationUnitContext : EmitContext
        {
            Compiler compiler;
			Executable exe;

            public TranslationUnitContext (Compiler compiler, Executable exe)
                : base (null, compiler.context.Report)
            {
                this.compiler = compiler;
				this.exe = exe;
            }

            public override void DeclareFunction (FunctionDeclaration fdecl)
			{

            }

            public override void DeclareVariable (VariableDeclaration v)
            {
            }
        }
    }
}
