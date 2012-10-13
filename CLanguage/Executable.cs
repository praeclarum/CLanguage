using System;
using System.Collections.Generic;
using System.IO;

using ValueType = System.Int32;

namespace CLanguage
{
	public class Executable
	{
		public MachineInfo MachineInfo { get; private set; }

		public List<CompiledFunction> Functions { get; private set; }
		public List<Global> Globals { get; private set; }

		public Executable (MachineInfo machineInfo)
		{
			MachineInfo = machineInfo;
			Functions = new List<CompiledFunction> ();
			Globals = new List<Global> ();
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

