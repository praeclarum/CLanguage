using System;

namespace CLanguage
{
	public class InternalFunction : BaseFunction
	{
		public InternalFunction (string prototype)
		{
			var parser = new CParser ();
			var pp = new Preprocessor ();
			pp.AddCode ("<Internal>", prototype + ";");
			var tu = parser.ParseTranslationUnit (new Lexer (pp), new Report (new TextWriterReportPrinter (Console.Out)));
			var f = tu.Functions[0];
			Name = f.Name;
			FunctionType = f.FunctionType;
		}

		public override string ToString ()
		{
			return Name;
		}

		public override void Step (ExecutionState state)
		{
			throw new NotImplementedException ();
		}
	}
}

