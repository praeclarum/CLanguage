using System;
using System.Collections.Generic;
using System.IO;

namespace CLanguage
{
	public class CompiledFunction : IFunction
	{
		public string Name { get; private set; }
		public CFunctionType FunctionType { get; private set; }
		public List<VariableDeclaration> LocalVariables { get; private set; }
		public List<Instruction> Instructions { get; private set; }

		public CompiledFunction (string name, CFunctionType functionType)
		{
			Name = name;
			FunctionType = functionType;
			LocalVariables = new List<VariableDeclaration> ();
			Instructions = new List<Instruction> ();
		}

		public override string ToString ()
		{
			return Name;
		}

		public string Assembler {
			get {
				var w = new StringWriter ();
				for (var i = 0; i < Instructions.Count; i++) {
					w.WriteLine ("{0}: {1}", i, Instructions[i]);
				}
				return w.ToString ();
			}
		}

		public void Step (ExecutionState state)
		{

		}
	}
}

