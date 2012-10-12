using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CLanguage
{
    public class MachineInfo
    {
        public int CharSize { get; set; }
        public int ShortIntSize { get; set; }
        public int IntSize { get; set; }
        public int LongIntSize { get; set; }
        public int LongLongIntSize { get; set; }
        public int FloatSize { get; set; }
        public int DoubleSize { get; set; }
        public int LongDoubleSize { get; set; }
        public int PointerSize { get; set; }

		public ObservableCollection<IFunction> InternalFunctions { get; set; }

		public MachineInfo ()
		{
			InternalFunctions = new ObservableCollection<IFunction> ();
		}

        public static readonly MachineInfo WindowsX86 = new MachineInfo
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

		public static readonly MachineInfo Arduino = new ArduinoMachineInfo ();
    }

	public class ArduinoMachineInfo : MachineInfo
	{
		public ArduinoMachineInfo ()
		{
			CharSize = 1;
			ShortIntSize = 2;
			IntSize = 4;
			LongIntSize = 4;
			LongLongIntSize = 8;
			FloatSize = 4;
			DoubleSize = 8;
			LongDoubleSize = 8;
			PointerSize = 4;
			InternalFunctions = new ObservableCollection<IFunction> {
				new InternalFunction ("void pinMode (int pin, int mode)"),
				new InternalFunction ("void digitalWrite (int pin, int value)"),
				new InternalFunction ("void analogWrite (int pin, int value)"),
				new InternalFunction ("void delay (unsigned long ms)"),
			};
		}
	}
}
