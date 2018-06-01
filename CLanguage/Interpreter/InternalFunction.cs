using System;
using CLanguage.Parser;

namespace CLanguage.Interpreter
{
	public delegate void InternalFunctionAction (ExecutionState state);

	public class InternalFunction : BaseFunction
	{
		public InternalFunctionAction Action { get; set; }

        public InternalFunction (MachineInfo machineInfo, string prototype, InternalFunctionAction action = null)
		{
			var report = new Report (new Report.TextWriterPrinter (Console.Out));
			var parser = new CParser ();
			var pp = new Preprocessor (report);
			pp.AddCode ("<Internal>", prototype + ";");
			var tu = parser.ParseTranslationUnit (new Lexer (pp));
            Compiler compiler = new Compiler (machineInfo, report);
            compiler.Add (tu);
            var exe = compiler.Compile ();
            if (tu.Functions.Count == 0) {
                throw new Exception ("Failed to parse function prototype: " + prototype);
            }
			var f = tu.Functions[0];
			Name = f.Name;
			FunctionType = f.FunctionType;

			Action = action;
		}

		public override string ToString ()
		{
			return Name;
		}

		public override void Step (ExecutionState state)
		{
			if (Action != null) {
				Action (state);
			}
			state.Return ();
		}
	}
}

