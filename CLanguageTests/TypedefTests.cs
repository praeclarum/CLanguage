using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;
using System.Globalization;

namespace CLanguage.Tests
{
    [TestClass]
    public class TypedefTests
    {
        CInterpreter Run (string code)
        {
            var fullCode = "void start() { __cinit(); main(); } " + code;
            var i = CLanguageService.CreateInterpreter (fullCode, new ArduinoTestMachineInfo (), printer: new TestPrinter ());
            i.Reset ("start");
            i.Step ();
            return i;
        }

        public void AssertTrue (bool expected, string code)
        {
            var i = Run (@"
void main () {
    assertBoolsAreEqual (" + (expected ? "true" : "false") + @", " + code + @");
}");
        }

        [TestMethod, Ignore]
        public void DeclareGlobal ()
        {
            Run (@"
typedef int Foo;
Foo one = 1;
void main () {
    assertAreEqual (1, one);
}");
        }
    }
}
