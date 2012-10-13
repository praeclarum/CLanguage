using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ValueType = System.Int32;

namespace CLanguage
{
	public class Executable
	{
		public MachineInfo MachineInfo { get; private set; }

		public List<BaseFunction> Functions { get; private set; }
		public List<VariableDeclaration> Globals { get; private set; }

		public Executable (MachineInfo machineInfo)
		{
			MachineInfo = machineInfo;
			Functions = new List<BaseFunction> ();
			Globals = new List<VariableDeclaration> ();

			Functions.AddRange (machineInfo.InternalFunctions.Cast<BaseFunction> ());
		}
	}
}

