using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;

namespace CLanguage.Tests
{
    [TestClass]
    public class OverloadTests
    {
        CInterpreter Run (string code)
        {
            var fullCode = "void start() { __cinit(); main(); } " + code;
            var i = CLanguageService.CreateInterpreter (fullCode, new ArduinoTestMachineInfo (), printer: new TestPrinter ());
            i.Reset ("start");
            i.Step ();
            return i;
        }

        [TestMethod]
        public void GlobalScoring ()
        {
            var i = Run (@"
int f(int x) { return 1; }
int f(double x) { return 2; }
void main () {
    assertAreEqual (1, f(0));
    assertAreEqual (2, f(0.0));
}");
        }

        [TestMethod]
        public void MemberScoring ()
        {
            var i = Run (@"
void main () {
    assertAreEqual (1, test.f(0));
    assertAreEqual (2, test.f(0.0));
}");
        }
    }
}
