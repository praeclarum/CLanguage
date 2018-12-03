using System;
using CLanguage.Interpreter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace CLanguage.Tests
{
    public class TestMachineInfo : MachineInfo
    {
        public TestMachineInfo ()
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
            HeaderCode = "";

            AddInternalFunction ("void assertAreEqual (int expected, int actual)", AssertAreEqual);
            AddInternalFunction ("void assertBoolsAreEqual (bool expected, bool actual)", AssertBoolsAreEqual);
            AddInternalFunction ("void assertFloatsAreEqual (float expected, float actual)", AssertFloatsAreEqual);
            AddInternalFunction ("void assertDoublesAreEqual (double expected, double actual)", AssertDoublesAreEqual);
        }

        static void AssertAreEqual (CInterpreter state)
        {
            var expected = state.ReadArg (0);
            var actual = state.ReadArg (1);
            Assert.AreEqual ((int)expected, (int)actual);
        }

        static void AssertFloatsAreEqual (CInterpreter state)
        {
            var expected = state.ReadArg (0);
            var actual = state.ReadArg (1);
            Assert.AreEqual ((float)expected, (float)actual, 1.0e-6);
        }

        static void AssertDoublesAreEqual (CInterpreter state)
        {
            var expected = state.ReadArg (0);
            var actual = state.ReadArg (1);
            Assert.AreEqual ((double)expected, (double)actual, 1.0e-12);
        }

        static void AssertBoolsAreEqual (CInterpreter state)
        {
            var expected = state.ReadArg (0);
            var actual = state.ReadArg (1);
            Assert.AreEqual ((int)expected, (int)actual);
        }
    }
}

