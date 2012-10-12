using System;
using System.Collections.Generic;
using System.IO;

using ValueType = System.Int32;

namespace CLanguage
{
	public class Executable
	{
		public MachineInfo MachineInfo { get; private set; }

		public List<Function> Functions { get; private set; }
		public List<Global> Globals { get; private set; }

		public Executable (MachineInfo machineInfo)
		{
			MachineInfo = machineInfo;
			Functions = new List<Function> ();
			Globals = new List<Global> ();
		}

		public class Function : IFunction
		{
			public string Name { get; private set; }
			public CFunctionType FunctionType { get; private set; }
			public List<VariableDeclaration> LocalVariables { get; private set; }
			public List<Instruction> Instructions { get; private set; }
			public Function (string name, CFunctionType functionType)
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
		}

		public class Global
		{
			public string Name { get; private set; }
			public CType VariableType { get; private set; }

			public Global (string name, CType variableType)
			{
				Name = name;
				VariableType = variableType;
			}
		}
	}
}

