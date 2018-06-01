using System;
using CLanguage.Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CLanguage.Tests
{
    public class ArduinoTestMachineInfo : MachineInfo
	{
        public TestArduino Arduino = new TestArduino ();

		public ArduinoTestMachineInfo ()
		{
            CharSize = 1;
            ShortIntSize = 2;
            IntSize = 2;
            LongIntSize = 4;
            LongLongIntSize = 8;
            FloatSize = 4;
            DoubleSize = 8;
            LongDoubleSize = 8;
            PointerSize = 2;
            HeaderCode = @"
#define HIGH 1
#define LOW 0
#define INPUT 0
#define INPUT_PULLUP 2
#define OUTPUT 1
#define true 1
#define false 0
//struct SerialClass {
//};
//struct SerialClass Serial;
";

            InternalFunctions.Add (new InternalFunction (this, "void pinMode (int pin, int mode)", Arduino.PinMode));
            InternalFunctions.Add (new InternalFunction (this, "void digitalWrite (int pin, int value)"));
            InternalFunctions.Add (new InternalFunction (this, "void analogWrite (int pin, int value)"));
            InternalFunctions.Add (new InternalFunction (this, "void delay (unsigned long ms)"));
            InternalFunctions.Add (new InternalFunction (this, "void assertAreEqual (int expected, int actual)", AssertAreEqual));
            //InternalFunctions.Add (new InternalFunction (this, "void SerialClass::setup (int baud)", Arduino.SerialSetup));
		}

		static void AssertAreEqual (ExecutionState state)
		{
			var expected = state.ActiveFrame.Args[0];
			var actual = state.ActiveFrame.Args[1];
            Assert.AreEqual (expected, actual);
		}

        public class TestArduino
        {
            public Pin[] Pins = Enumerable.Range (0, 32).Select (x => new Pin { Index = x }).ToArray ();

            public void PinMode (ExecutionState state)
            {
                var pin = state.ActiveFrame.Args[0];
                var mode = state.ActiveFrame.Args[1];
                Pins[pin].Mode = mode;
            }

            public void SerialSetup (ExecutionState state)
            {
            }

            public class Pin
            {
                public int Index;
                public int Mode;
            }
        }
	}
}

