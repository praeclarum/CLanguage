using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ValueType = System.Int32;

using CLanguage.Ast;

namespace CLanguage
{
    public class Compiler
    {
		CompilerContext context;

		List<TranslationUnit> tus;

		public Compiler (CompilerContext context)
		{
			this.context = context;
			tus = new List<TranslationUnit> ();
		}

		public Compiler (MachineInfo mi, Report report)
			: this (new CompilerContext (mi, report))
        {
        }

		public Executable Compile ()
		{
			var exe = new Executable (context.MachineInfo);

			// Put something at the zero address so we don't get 0 addresses of globals
			exe.Globals.Add (new VariableDeclaration ("__zero__", CBasicType.SignedInt));

			foreach (var tu in tus) {

				exe.Globals.AddRange (tu.Variables);

				foreach (var fdecl in tu.Functions) {
					var fexe = exe.Functions.FirstOrDefault (x => x.Name == fdecl.Name);
					if (fexe == null) {
						fexe = new CompiledFunction (fdecl.Name, fdecl.FunctionType);
						exe.Functions.Add (fexe);
					}
					var cfexe = fexe as CompiledFunction;
					if (cfexe != null && fdecl.Body != null) {
						var c = new FunctionContext (exe, fdecl, cfexe, context);
						fdecl.Body.Emit (c);
						cfexe.LocalVariables.AddRange (c.LocalVariables);

						// Make sure it returns
						if (fdecl.Body.Statements.Count == 0 || !fdecl.Body.AlwaysReturns) {
							if (fdecl.FunctionType.ReturnType.IsVoid) {
								c.Emit (OpCode.Return);
							}
							else {
								context.Report.Error (161, "'" + fdecl.Name + "' not all code paths return a value");
							}
						}
					}
				}
			}

			return exe;
		}

        public void Add (TranslationUnit translationUnit)
        {
			tus.Add (translationUnit);
        }

		public void AddCode (string code)
		{
			AddCode("stdin", code);
		}

        public void AddCode (string name, string code)
        {
            var pp = new Preprocessor (context.Report);
			pp.AddCode ("machine.h", context.MachineInfo.HeaderCode);
            pp.AddCode (name, code);
            var lexer = new Lexer (pp);
            var parser = new CParser ();
            Add (parser.ParseTranslationUnit (lexer, context.Report));
        }

        class FunctionContext : EmitContext
        {
			Executable exe;
			FunctionDeclaration fdecl;
			CompiledFunction fexe;
			CompilerContext context;

			class BlockLocals
			{
				public int StartIndex;
				public int Length;
			}
			List<Block> blocks;
			Dictionary<Block, BlockLocals> blockLocals;
			List<VariableDeclaration> allLocals;

			public IEnumerable<VariableDeclaration> LocalVariables { get { return allLocals; } }

			public FunctionContext (Executable exe, FunctionDeclaration fdecl, CompiledFunction fexe, CompilerContext context)
                : base (context.MachineInfo, context.Report, fdecl)
            {
				this.exe = exe;
				this.fdecl = fdecl;
				this.fexe = fexe;
				this.context = context;
				blocks = new List<Block> ();
				blockLocals = new Dictionary<Block, BlockLocals> ();
				allLocals = new List<VariableDeclaration> ();
            }

            public override ResolvedVariable ResolveVariable (string name)
			{
				//
				// Look for function parameters
				//
				for (var i = 0; i < fdecl.ParameterInfos.Count; i++) {
					if (fdecl.ParameterInfos[i].Name == name) {
						return new ResolvedVariable (VariableScope.Arg, i, fdecl.FunctionType.Parameters[i].ParameterType);
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
