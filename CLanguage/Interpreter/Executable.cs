using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CLanguage.Syntax;
using ValueType = System.Int32;

namespace CLanguage.Interpreter
{
	public class Executable
	{
		public MachineInfo MachineInfo { get; private set; }

		public List<BaseFunction> Functions { get; private set; }
		public List<CompiledVariable> Globals { get; private set; }

		public Executable (MachineInfo machineInfo)
		{
			MachineInfo = machineInfo;
			Functions = new List<BaseFunction> ();
            Globals = new List<CompiledVariable> ();

			Functions.AddRange (machineInfo.InternalFunctions.Cast<BaseFunction> ());
		}

        public int GetConstantMemory (string stringConstant)
        {
            var index = Globals.Count;
            var v = new CompiledVariable ("__c" + Globals.Count, Types.CPointerType.PointerToConstChar);
            Globals.Add (v);
            return index;
        }
    }
}

