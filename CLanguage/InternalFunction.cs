using System;

namespace CLanguage
{
	public class InternalFunction : IFunction
	{
		public string Name { get; private set; }
		public CFunctionType FunctionType { get; private set; }

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

		public void Step (ExecutionState state)
		{

		}
	}
}

