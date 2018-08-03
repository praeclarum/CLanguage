using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;

namespace CLanguage.Tests
{
    [TestClass]
    public class ArrayTests
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
        public void GlobalInitInts ()
        {
            var i = Run (@"
int a[] = { 0, 100, 200, 300, };
void main () {
    assertAreEqual (0, a[0]);
    assertAreEqual (100, a[1]);
    assertAreEqual (200, a[2]);
    assertAreEqual (300, a[3]);
}");
        }
    }
}
