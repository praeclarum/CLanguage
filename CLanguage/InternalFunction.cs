using System;

namespace CLanguage
{
	public delegate void InternalFunctionAction (ExecutionState state);

	public class InternalFunction : BaseFunction
	{
		public InternalFunctionAction Action { get; set; }

		public InternalFunction (string prototype, InternalFunctionAction action = null)
		{
			var report = new Report (new TextWriterReportPrinter (Console.Out));
			var parser = new CParser ();
			var pp = new Preprocessor (report);
			pp.AddCode ("<Internal>", prototype + ";");
			var tu = parser.ParseTranslationUnit (new Lexer (pp), report);
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

