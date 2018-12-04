using System;
using System.Linq;
using CLanguage.Interpreter;
using CLanguage.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CLanguage.CLanguageService;
using System.Collections.Generic;

namespace CLanguage.Tests
{
    [TestClass]
    public class InternalObjectTests : TestsBase
    {
        class Calc
        {
            public string Title { get; set; }
            readonly List<long> stack = new List<long> ();
            public int Count => stack.Count;
            public void PushInt64 (long x)
            {
                stack.Add (x);
            }
            public long PopInt64 ()
            {
                var x = stack[stack.Count - 1];
                stack.RemoveAt (stack.Count - 1);
                return x;
            }
            public void PushChar (char x) => PushInt64 (x);
            public char PopChar () => (char)PopInt64 ();
            public void PushInt8 (sbyte x) => PushInt64 (x);
            public sbyte PopInt8 () => (sbyte)PopInt64 ();
            public void PushUInt8 (byte x) => PushInt64 (x);
            public byte PopUInt8 () => (byte)PopInt64 ();
            public void PushInt16 (short x) => PushInt64 (x);
            public short PopInt16 () => (short)PopInt64 ();
            public void PushUInt16 (ushort x) => PushInt64 (x);
            public ushort PopUInt16 () => (ushort)PopInt64 ();
            public void PushInt32 (int x) => PushInt64 (x);
            public int PopInt32 () => (int)PopInt64 ();
            public void PushUInt32 (uint x) => PushInt64 (x);
            public uint PopUInt32 () => (uint)PopInt64 ();
            public void PushUInt64 (ulong x) => PushInt64 ((long)x);
            public ulong PopUInt64 () => (ulong)PopInt64 ();
            public void PushSingle (float x) => PushInt64 ((long)x);
            public float PopSingle () => (float)PopInt64 ();
            public void PushDouble (double x) => PushInt64 ((long)x);
            public double PopDouble () => (double)PopInt64 ();
        }

        Calc TestReference (string code)
        {
            var mi = new ArduinoTestMachineInfo ();
            var c = new Calc ();
            mi.AddGlobalReference ("c", c);
            Run ("void main() { " + code + "}", mi);
            return c;
        }

        Calc TestMethods (string code)
        {
            var mi = new ArduinoTestMachineInfo ();
            var c = new Calc ();
            mi.AddGlobalMethods (c);
            Run ("void main() { " + code.Replace ("c.", "") + "}", mi);
            return c;
        }

        Calc TestCalc (string code)
        {
            TestReference (code);
            return TestMethods (code);
        }

        [TestMethod]
        public void Int8 ()
        {
            TestCalc (@"
    c.pushInt8(-1);
    assertAreEqual(-1, c.popInt8());
");
        }

        [TestMethod]
        public void UInt8 ()
        {
            TestCalc (@"
    c.pushUInt8(345);
    assertAreEqual(89, c.popUInt8());
");
        }

        [TestMethod]
        public void Int16 ()
        {
            TestCalc (@"
    c.pushInt16(-345);
    assertAreEqual(-345, c.popInt16());
");
        }

        [TestMethod]
        public void UInt16 ()
        {
            TestCalc (@"
    c.pushUInt16(345);
    assertU16AreEqual(345, c.popUInt16());
");
        }

        [TestMethod]
        public void Int32 ()
        {
            TestCalc (@"
    c.pushInt32(-8112000);
    assert32AreEqual(-8112000, c.popInt32());
");
        }

        [TestMethod]
        public void UInt32 ()
        {
            TestCalc (@"
    c.pushUInt32(678000);
    assertU32AreEqual(678000, c.popUInt32());
");
        }

        [TestMethod]
        public void Single ()
        {
            TestCalc (@"
    c.pushSingle(-42.0);
    assertAreEqual(-42.0, c.popSingle());
");
        }

        [TestMethod]
        public void Double ()
        {
            TestCalc (@"
    c.pushDouble(-42.0);
    assertAreEqual(-42.0, c.popDouble());
");
        }

        [TestMethod]
        public void Char ()
        {
            TestCalc (@"
    c.pushChar('x');
    assertAreEqual('x', c.popChar());
");
        }

        [TestMethod]
        public void StringPropertySetter ()
        {
            var c = TestCalc (@"
    c.setTitle(""hello"");
");
            Assert.AreEqual ("hello", c.Title);
        }

        [TestMethod]
        public void PropertyGetter ()
        {
            var c = TestCalc (@"
    c.pushChar('a');
    c.pushChar('b');
    assertAreEqual(2, c.getCount());
");
        }
    }
}

