using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CLanguage.Syntax;
using CLanguage.Types;

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

        public CompiledVariable AddGlobal (string name, CType type)
        {
            var last = Globals.LastOrDefault ();
            var offset = last == null ? 0 : last.Offset + last.VariableType.NumValues;
            var v = new CompiledVariable (name, offset, type);
            Globals.Add (v);
            return v;
        }

        public Value GetConstantMemory (string stringConstant)
        {
            var index = Globals.Count;
            var v = AddGlobal ("__c" + Globals.Count, CPointerType.PointerToConstChar);
            return Value.GlobalPointer (v.Offset);
        }
    }
}

