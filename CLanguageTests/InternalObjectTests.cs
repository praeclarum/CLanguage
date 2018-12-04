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
            readonly List<long> stack = new List<long> ();
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

        void TestCalc (string code)
        {
            var mi = new ArduinoTestMachineInfo ();
            mi.AddGlobalReference ("c", new Calc ());
            Run ("void main() { " + code + "}", mi);
        }

        [TestMethod]
        public void Int8 ()
        {
            TestCalc (@"
    c.PushInt8(-1);
    assertAreEqual(-1, c.PopInt8());
");
        }

        [TestMethod]
        public void UInt8 ()
        {
            TestCalc (@"
    c.PushUInt8(345);
    assertAreEqual(89, c.PopUInt8());
");
        }

        [TestMethod]
        public void Int16 ()
        {
            TestCalc (@"
    c.PushInt16(-345);
    assertAreEqual(-345, c.PopInt16());
");
        }

        [TestMethod]
        public void UInt16 ()
        {
            TestCalc (@"
    c.PushUInt16(345);
    assertU16AreEqual(345, c.PopUInt16());
");
        }

        [TestMethod]
        public void Int32 ()
        {
            TestCalc (@"
    c.PushInt32(-8112000);
    assert32AreEqual(-8112000, c.PopInt32());
");
        }

        [TestMethod]
        public void UInt32 ()
        {
            TestCalc (@"
    c.PushUInt32(678000);
    assertU32AreEqual(678000, c.PopUInt32());
");
        }

        [TestMethod]
        public void Single ()
        {
            TestCalc (@"
    c.PushSingle(-42.0);
    assertAreEqual(-42.0, c.PopSingle());
");
        }

        [TestMethod]
        public void Double ()
        {
            TestCalc (@"
    c.PushDouble(-42.0);
    assertAreEqual(-42.0, c.PopDouble());
");
        }

        [TestMethod]
        public void Char ()
        {
            TestCalc (@"
    c.PushChar('x');
    assertAreEqual('x', c.PopChar());
");
        }
    }
}

