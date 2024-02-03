using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;
using System.Globalization;

namespace CLanguage.Tests
{
    [TestClass]
    public class LogicTests
    {
        CInterpreter Run (string code)
        {
            var fullCode = "void start() { __cinit(); main(); } " + code;
            var i = CLanguageService.CreateInterpreter (fullCode, new ArduinoTestMachineInfo (), printer: new TestPrinter ());
            i.Reset ("start");
            i.Run ();
            return i;
        }

        public void AssertTrue (bool expected, string code)
        {
            var i = Run (@"
void main () {
    assertBoolsAreEqual (" + (expected ? "true" : "false") + @", " + code + @");
}");
        }

        [TestMethod]
        public void And ()
        {
            AssertTrue (false && false, "false && false");
            AssertTrue (false && true, "false && true");
            AssertTrue (true && false, "true && false");
            AssertTrue (true && true, "true && true");
        }

        [TestMethod]
        public void Or ()
        {
            AssertTrue (false || false, "false || false");
            AssertTrue (false || true, "false || true");
            AssertTrue (true || false, "true || false");
            AssertTrue (true || true, "true || true");
        }
    }
}
