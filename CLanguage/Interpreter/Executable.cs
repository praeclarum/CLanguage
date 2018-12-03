using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CLanguage.Syntax;
using CLanguage.Types;
using System.Text;

namespace CLanguage.Interpreter
{
	public class Executable
	{
		public MachineInfo MachineInfo { get; private set; }

		public List<BaseFunction> Functions { get; private set; }

        readonly List<CompiledVariable> globals = new List<CompiledVariable> ();
        public IReadOnlyList<CompiledVariable> Globals => globals;

        readonly List<CompiledType> globalTypes = new List<CompiledType> ();
        public IReadOnlyList<CompiledType> GlobalTypes => globalTypes;

        public Executable (MachineInfo machineInfo)
		{
			MachineInfo = machineInfo;
			Functions = new List<BaseFunction> ();

			Functions.AddRange (machineInfo.InternalFunctions.Cast<BaseFunction> ());
		}

        public CompiledVariable AddGlobal (string name, CType type)
        {
            var last = Globals.LastOrDefault ();
            var offset = last == null ? 0 : last.Offset + last.VariableType.NumValues;
            var v = new CompiledVariable (name, offset, type);
            globals.Add (v);
            return v;
        }

        public CType AddGlobalType (string name, CType type)
        {
            var v = new CompiledType (name, type);
            globalTypes.Add (v);
            return type;
        }

        public Value GetConstantMemory (string stringConstant)
        {
            var index = Globals.Count;
            var bytes = Encoding.UTF8.GetBytes (stringConstant);
            var len = bytes.Length + 1;
            var type = new CArrayType (CBasicType.SignedChar, len);
            var v = AddGlobal ("__c" + Globals.Count, type);
            v.InitialValue = bytes.Concat (new byte[] { 0 }).Select (x => (Value)x).ToArray ();
            return Value.Pointer (v.Offset);
        }
    }
}

