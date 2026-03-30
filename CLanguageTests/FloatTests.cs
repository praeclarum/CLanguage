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
            i.Run ();
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

        [TestMethod]
        public void LargeIntegerConstantToDouble ()
        {
            // Issue: integer constants that don't fit in 16-bit int
            // should be promoted to long before casting to double
            var i = Run (@"
double f(double x) { return 10.0 * x; }
void main () {
    assertDoublesAreEqual (2400000.0, f(240000));
}");
        }

        [TestMethod]
        public void IntegerConstantToDoubleParam ()
        {
            var i = Run (@"
double f(double x) { return x; }
void main () {
    assertDoublesAreEqual (240000.0, f(240000));
}");
        }

        [TestMethod]
        public void IntegerConstantArithmeticWithDouble ()
        {
            AssertEqual (100000.0 + 1.5, "100000 + 1.5");
            AssertEqual (100000.0 * 2.0, "100000 * 2.0");
        }
    }
}
