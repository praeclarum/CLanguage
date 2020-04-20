using System;
using CLanguage.Parser;
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
                throw new Exception ("Failed to parse function prototype: " + prototype);
            }
			var f = tu.Functions[0];

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

