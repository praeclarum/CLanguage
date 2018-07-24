using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;
using System.Globalization;

namespace CLanguage.Tests
{
    [TestClass]
    public class FloatTests
    {
        CInterpreter Run (string code)
        {
            var fullCode = "void start() { __cinit(); main(); } " + code;
            var i = CLanguageService.CreateInterpreter (fullCode, new ArduinoTestMachineInfo (), printer: new TestPrinter ());
            i.Reset ("start");
            i.Step ();
            return i;
        }

        public void AssertEqual (double f, string code)
        {
            var i = Run (@"
void main () {
    assertDoublesAreEqual (" + f.ToString("#.0###########################", CultureInfo.InvariantCulture)+ @", " + code + @");
}");
        }

        public void AssertEqual (float f, string code)
        {
            var i = Run (@"
void main () {
    assertFloatsAreEqual (" + f.ToString ("#.0###########################f", CultureInfo.InvariantCulture) + @", " + code + @");
}");
        }

        public void AssertTrue (bool expected, string code)
        {
            var i = Run (@"
void main () {
    assertBoolsAreEqual (" + (expected?"true":"false") + @", " + code + @");
}");
        }

        [TestMethod]
        public void FloatArithmetic ()
        {
            AssertEqual (10.0f + 3.01f, "10.0f+3.01f");
            AssertEqual (10.0f - 3.01f, "10.0f-3.01f");
            AssertEqual (10.0f * 3.01f, "10.0f*3.01f");
            AssertEqual (10.0f / 3.01f, "10.0f/3.01f");
        }

        [TestMethod]
        public void DoubleArithmetic ()
        {
            AssertEqual (10.0 + 3.01, "10.0+3.01");
            AssertEqual (10.0 - 3.01, "10.0-3.01");
            AssertEqual (10.0 * 3.01, "10.0*3.01");
            AssertEqual (10.0 / 3.01, "10.0/3.01");
        }

        [TestMethod]
        public void DoubleLogic ()
        {
            AssertTrue (10.0 < 3.01, "10.0<3.01");
            AssertTrue (10.0 > 3.01, "10.0>3.01");
            AssertTrue (10.0 == 3.01, "10.0==3.01");
            AssertTrue (10.0 <= 3.01, "10.0<=3.01");
            AssertTrue (10.0 >= 3.01, "10.0>=3.01");
            AssertTrue (10.0 <= 10.0, "10.0<=10.0");
            AssertTrue (10.0 >= 10.0, "10.0>=10.0");
        }

        [TestMethod]
        public void FloatLogic ()
        {
            AssertTrue (10.0f < 3.01f, "10.0f<3.01f");
            AssertTrue (10.0f > 3.01f, "10.0f>3.01f");
            AssertTrue (10.0f == 3.01f, "10.0f==3.01f");
            AssertTrue (10.0f <= 3.01f, "10.0f<=3.01f");
            AssertTrue (10.0f >= 3.01f, "10.0f>=3.01f");
            AssertTrue (10.0f <= 10.0f, "10.0f<=10.0f");
            AssertTrue (10.0f >= 10.0f, "10.0f>=10.0f");
        }
    }
}
