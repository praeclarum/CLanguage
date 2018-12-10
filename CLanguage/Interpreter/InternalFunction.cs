using System;
using CLanguage.Parser;

namespace CLanguage.Interpreter
{
	public delegate void InternalFunctionAction (CInterpreter state);

	public class InternalFunction : BaseFunction
	{
		public InternalFunctionAction Action { get; set; }

        public InternalFunction (MachineInfo machineInfo, string prototype, InternalFunctionAction action = null)
		{
			var report = new Report (new Report.TextWriterPrinter (Console.Out));
			var parser = new CParser ();
			var tu = parser.ParseTranslationUnit ("_internal.h", prototype + ";", ((_, __) => null), report);
            var compiler = new Compiler.CCompiler (machineInfo, report);
            compiler.Add (tu);
            var exe = compiler.Compile ();
            if (tu.Functions.Count == 0) {
                throw new Exception ("Failed to parse function prototype: " + prototype);
            }
			var f = tu.Functions[0];

			Name = f.Name;
            NameContext = f.NameContext;
			FunctionType = f.FunctionType;
			Action = action;
		}

		public override string ToString ()
		{
			return Name;
		}

		public override void Step (CInterpreter state)
		{
			if (Action != null) {
				Action (state);
			}
			state.Return ();
		}
	}
}

