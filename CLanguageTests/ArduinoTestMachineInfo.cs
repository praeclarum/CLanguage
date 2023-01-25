using System;
using CLanguage.Interpreter;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using CLanguage.Types;
using CLanguage.Compiler;

namespace CLanguage.Tests
{
    public class ArduinoTestMachineInfo : TestMachineInfo
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
#define A0 0
#define A1 1
#define A2 2
#define A3 3
#define A4 4
#define A5 5
#define DEC 10
#define HEX 16
#define OCT 8
#define BIN 2
#define NOTE_C4 403
#define NOTE_G3 307
#define NOTE_A3 301
#define NOTE_B3 302
#define bitRead(x, n) ((x & (1 << n)) != 0)
typedef bool boolean;
typedef unsigned char byte;
typedef unsigned short word;
struct SerialClass {
    void begin(int baud);
    //void print(char value);
    //void print(int value);
    void print(const char *value);
    void println();
    void println(int value, int bas);
    void println(int value);
	void println(char value);
    void println(unsigned long value);
    void println(double value);
    void println(float value);
    void println(const char *value);
};
struct MemberTest {
    int f();
    int f(char testme);
    int f(int testme);
    int f(int testme, int bas);
    int f(float testme);
    int f(double testme);
    int f(const char *testme);
};
struct MemberTest test;
struct SerialClass Serial;
struct CtorTest {
    int x;
    CtorTest(int x);
};
struct WireClass {
    void onReceive(void (*callback)(int x));
};
struct WireClass Wire;
";

            AddInternalFunction ("void pinMode (int pin, int mode)", Arduino.PinMode);
            AddInternalFunction ("void digitalWrite (int pin, int value)", Arduino.DigitalWrite);
            AddInternalFunction ("int digitalRead (int pin)", Arduino.DigitalRead);
            AddInternalFunction ("void analogWrite (int pin, int value)");
            AddInternalFunction ("int analogRead (int pin)", Arduino.AnalogRead);
            AddInternalFunction ("int map (int value, int fromLow, int fromHigh, int toLow, int toHigh)", Arduino.Map);
            AddInternalFunction ("int constrain (int x, int a, int b)", Arduino.Constrain);
            AddInternalFunction ("void delay (unsigned long ms)");
            AddInternalFunction ("void tone (int pin, int note, int duration)");
            AddInternalFunction ("void noTone (int pin)");
            AddInternalFunction ("long millis ()", Arduino.Millis);
            AddInternalFunction ("void SerialClass::begin (int baud)", Arduino.SerialBegin);
            //AddInternalFunction ("void SerialClass::print (char value)", Arduino.SerialPrintC);
            //AddInternalFunction ("void SerialClass::print (int value)", Arduino.SerialPrintI);
            AddInternalFunction ("void SerialClass::print (const char *value)", Arduino.SerialPrintS);
            AddInternalFunction ("void SerialClass::println ()", Arduino.SerialPrintln);
            AddInternalFunction ("void SerialClass::println (int value, int bas)", Arduino.SerialPrintlnBas);
            AddInternalFunction ("void SerialClass::println (int value)", Arduino.SerialPrintlnI);
            AddInternalFunction ("void SerialClass::println (char value)", Arduino.SerialPrintlnC);
            AddInternalFunction ("void SerialClass::println (unsigned long value)", Arduino.SerialPrintlnUL);
            AddInternalFunction ("void SerialClass::println (float value)", Arduino.SerialPrintlnF);
            AddInternalFunction ("void SerialClass::println (double value)", Arduino.SerialPrintlnD);
            AddInternalFunction ("void SerialClass::println (const char* value)", Arduino.SerialPrintlnS);
            AddInternalFunction ("int MemberTest::f ()", x => x.Push (0));
            AddInternalFunction ("int MemberTest::f (char)", x => x.Push (1));
            AddInternalFunction ("int MemberTest::f (int)", x => x.Push (2));
            AddInternalFunction ("int MemberTest::f (int, int)", x => x.Push (22));
            AddInternalFunction ("int MemberTest::f (float)", x => x.Push (3));
            AddInternalFunction ("int MemberTest::f (double)", x => x.Push (4));
            AddInternalFunction ("int MemberTest::f (const char*)", x => x.Push (6));
            AddInternalFunction ("void CtorTest::CtorTest (int)", x => {
                var _this = x.ReadThis ().PointerValue;
                var arg = x.ReadArg (0);
                x.Stack[_this] = arg;
            });
            AddInternalFunction ("void voidCallbackTest (void (*callback)(int x, int y), int xx, int yy)", x => {
                var callback = x.ReadArg (0);
                var xx = x.ReadArg (1);
                var yy = x.ReadArg (2);
                x.RunFunction (callback, xx, yy, 1_000_000);
            });
            AddInternalFunction ("int intCallbackTest (int (*callback)(int x, int y), int xx, int yy)", x => {
                var callback = x.ReadArg (0);
                var xx = x.ReadArg (1);
                var yy = x.ReadArg (2);
                var result = x.RunFunction (callback, xx, yy, 1_000_000);
                x.Push (result);
            });
            AddInternalFunction ("void WireClass::onReceive(void (*callback)(int x))", x => {
                var callback = x.ReadArg (0);
                var result = x.RunFunction (callback, 22, 1_000_000);
                x.Push (result);
            });
        }

        public override ResolvedVariable GetUnresolvedVariable (string name, CType[] argTypes, EmitContext context)
        {
            if (name.Length == 9 && name[0] == 'B') {
                byte b = 0;
                for (var i = 0; i < 8; i++) {
                    var c = name[i + 1];
                    var on = false;
                    if (c == '0') { }
                    else if (c == '1')
                        on = true;
                    else
                        return base.GetUnresolvedVariable (name, argTypes, context);
                    if (on) {
                        b = (byte)(b | (1 << (7 - i)));
                    }
                }
                return new ResolvedVariable (b, CBasicType.UnsignedChar);
            }
            return base.GetUnresolvedVariable (name, argTypes, context);
        }

        public class TestArduino
        {
            readonly Stopwatch stopwatch = new Stopwatch ();

            public Pin[] Pins = Enumerable.Range (0, 32).Select (x => new Pin { Index = x }).ToArray ();

            public StringWriter SerialOut = new StringWriter ();

            public TestArduino ()
            {
                stopwatch.Start ();
            }

            public void Millis (CInterpreter state)
            {
                state.Push ((int)stopwatch.ElapsedMilliseconds);
            }

            public void Map (CInterpreter state)
            {
                state.Push (0);
            }

            public void Constrain (CInterpreter state)
            {
                state.Push (0);
            }

            public void PinMode (CInterpreter state)
            {
                var pin = state.ReadArg(0).Int16Value;
                var mode = state.ReadArg(1).Int16Value;
                Pins[pin].Mode = mode;
            }

            public void AnalogRead (CInterpreter state)
            {
                var pin = state.ReadArg(0).Int16Value;
                var value = Pins[pin].AnalogValue;
                state.Push (value);
            }

            public void DigitalRead (CInterpreter state)
            {
                var pin = state.ReadArg(0).Int16Value;
                var value = Pins[pin].DigitalValue;
                state.Push (value);
            }

            public void DigitalWrite (CInterpreter state)
            {
                var pin = state.ReadArg(0).Int16Value;
                var value = state.ReadArg(1).Int16Value;
                Pins[pin].DigitalValue = value;
            }

            public void SerialBegin (CInterpreter state)
            {
            }

            public void SerialPrintlnII (CInterpreter state)
            {
                var v = state.ReadArg(0).Int16Value;
                SerialOut.WriteLine (v);
            }

            public void SerialPrintlnBas (CInterpreter state)
            {
                var p = state.ReadArg (0).Int32Value;
                var b = state.ReadArg (1).Int32Value;
                var s = Convert.ToString (p, b);
                SerialOut.WriteLine (s);
            }

            public void SerialPrintlnI (CInterpreter state)
            {
                var v = state.ReadArg(0).Int16Value;
                SerialOut.WriteLine (v);
            }

            public void SerialPrintlnUL (CInterpreter state)
            {
                var v = state.ReadArg(0).UInt32Value;
                SerialOut.WriteLine (v);
            }

            public void SerialPrintlnC (CInterpreter state)
            {
                var v = state.ReadArg(0).CharValue;
                SerialOut.WriteLine (v);
            }

            public void SerialPrintlnF (CInterpreter state)
            {
                var v = state.ReadArg(0).Float32Value;
                SerialOut.WriteLine (v);
            }

            public void SerialPrintlnD (CInterpreter state)
            {
                var v = state.ReadArg(0).Float64Value;
                SerialOut.WriteLine (v);
            }

            public void SerialPrintS (CInterpreter state)
            {
                var p = state.ReadArg (0).PointerValue;
                SerialOut.Write (state.ReadString (p));
            }

            public void SerialPrintln (CInterpreter state)
            {
                SerialOut.WriteLine ();
            }

            public void SerialPrintlnS (CInterpreter state)
            {
                var p = state.ReadArg (0).PointerValue;
                SerialOut.WriteLine (state.ReadString (p));
            }

            public class Pin
            {
                public int Index;
                public int Mode;
                public int DigitalValue;
                public int AnalogValue = 42;
            }
        }
	}
}

