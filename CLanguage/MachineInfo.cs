using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using CLanguage.Interpreter;
using CLanguage.Types;

namespace CLanguage
{
    public class MachineInfo
    {
        public int CharSize { get; set; } = 1;
        public int ShortIntSize { get; set; } = 2;
        public int IntSize { get; set; } = 4;
        public int LongIntSize { get; set; } = 4;
        public int LongLongIntSize { get; set; } = 8;
        public int FloatSize { get; set; } = 4;
        public int DoubleSize { get; set; } = 8;
        public int LongDoubleSize { get; set; } = 8;
        public int PointerSize { get; set; } = 4;

		public string HeaderCode { get; set; }

		public Collection<BaseFunction> InternalFunctions { get; set; }

		public MachineInfo ()
		{
			InternalFunctions = new Collection<BaseFunction> ();
			HeaderCode = "";
		}

        public void AddInternalFunction (string prototype, InternalFunctionAction action = null)
        {
            InternalFunctions.Add (new InternalFunction (this, prototype, action));
        }

        public static readonly MachineInfo Windows32 = new MachineInfo
        {
            CharSize = 1,
            ShortIntSize = 2,
            IntSize = 4,
            LongIntSize = 4,
            LongLongIntSize = 8,
            FloatSize = 4,
            DoubleSize = 8,
            LongDoubleSize = 8,
            PointerSize = 4,
        };

		public static readonly MachineInfo Mac64 = new MachineInfo
		{
			CharSize = 1,
			ShortIntSize = 2,
			IntSize = 4,
			LongIntSize = 8,
			LongLongIntSize = 8,
			FloatSize = 4,
			DoubleSize = 8,
			LongDoubleSize = 8,
			PointerSize = 8,
		};
    }
}
