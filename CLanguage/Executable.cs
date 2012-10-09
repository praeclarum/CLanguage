using System;
using System.Collections.ObjectModel;
using System.IO;

namespace CLanguage
{
	public class Executable
	{
		public class Function : IFunction
		{
			public string Name { get; private set; }
			public CFunctionType FunctionType { get; private set; }

			public ObservableCollection<Instruction> Instructions { get; private set; }
			public Function (string name, CFunctionType functionType)
			{
				Name = name;
				FunctionType = functionType;
				Instructions = new ObservableCollection<Instruction> ();
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

		public ObservableCollection<Function> Functions { get; private set; }
		public ObservableCollection<Global> Globals { get; private set; }

		public Executable ()
		{
			Functions = new ObservableCollection<Function> ();
			Globals = new ObservableCollection<Global> ();
		}
	}
}

