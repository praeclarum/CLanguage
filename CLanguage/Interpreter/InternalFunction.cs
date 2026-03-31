using System;
using CLanguage.Parser;
using CLanguage.Syntax;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
	public delegate void InternalFunctionAction (CInterpreter state);

	public class InternalFunction : BaseFunction
	{
		public InternalFunctionAction Action { get; set; }

        public InternalFunction (string name, string nameContext, CFunctionType functionType)
        {
            Name = name;
            NameContext = nameContext;
            FunctionType = functionType;
            Action = _ => { };
        }

        public InternalFunction (MachineInfo machineInfo, string prototype, InternalFunctionAction? action = null)
		{
			var report = new Report ();
			var parser = new CParser ();
			var tu = parser.ParseTranslationUnit ("_internal.h", prototype + ";", ((_, __) => null), report);
            var compiler = new Compiler.CCompiler (machineInfo, report);
            compiler.Add (tu);
            var exe = compiler.Compile ();
            if (tu.Functions.Count == 0) {
                // Retry with machine headers so the prototype can reference
                // struct types defined in HeaderCode (needed for operators
                // with struct parameters/return types).
                report = new Report ();
                parser = new CParser ();
                var headerDoc = new LexedDocument (new Document ("_machine.h", machineInfo.GeneratedHeaderCode), report);
                var protoDoc = new LexedDocument (new Document ("_internal.h", prototype + ";"), report);
                tu = parser.ParseTranslationUnit (report, "_internal",
                    ((_, __) => null), headerDoc.Tokens, protoDoc.Tokens);
                compiler = new Compiler.CCompiler (machineInfo, report);
                compiler.Add (tu);
                exe = compiler.Compile ();
                if (tu.Functions.Count == 0) {
                    throw new Exception ("Failed to parse function prototype: " + prototype);
                }
            }
			// The prototype function is the last one in the TU. When parsed
			// without headers, it's the only function (index 0). When parsed
			// with headers, struct member method declarations stay in the
			// struct body block — only top-level declarations appear in
			// tu.Functions — so the prototype is still the last entry.
			var f = tu.Functions[tu.Functions.Count - 1];

			Name = f.Name;
            NameContext = f.NameContext;
			FunctionType = f.FunctionType;
            if (action != null) {
                Action = action;
            }
            else {
                Action = _ => { };
            }
		}

		public override void Step (CInterpreter state, ExecutionFrame frame)
		{
            var a = Action;
            if (a != null) {
                a (state);
            }
            if (state.YieldedValue == 0)
    			state.Return ();
		}
	}
}

