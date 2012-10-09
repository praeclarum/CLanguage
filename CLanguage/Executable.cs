using System;
using System.Collections.ObjectModel;
using System.IO;

namespace CLanguage
{
	public class Executable
	{
		public class Function
		{
			public string Name { get; private set; }
			public ObservableCollection<Instruction> Instructions { get; private set; }
			public Function (string name)
			{
				Name = name;
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

		public ObservableCollection<Function> Functions { get; private set; }

		public Executable ()
		{
			Functions = new ObservableCollection<Function> ();
		}
	}
}

