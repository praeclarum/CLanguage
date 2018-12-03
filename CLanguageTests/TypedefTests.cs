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
            var i = CLanguageService.CreateInterpreter (fullCode, new TestMachineInfo (), printer: new TestPrinter ());
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

        [TestMethod]
        public void GlobalInt ()
        {
            Run (@"
typedef int Foo;
Foo one = 1;
void main () {
    assertAreEqual (1, one);
}");
        }

        [TestMethod]
        public void LocalInt ()
        {
            Run (@"
typedef int Foo;
void main () {
    Foo one = 1;
    assertAreEqual (1, one);
}");
        }

        [TestMethod]
        public void ForInt ()
        {
            Run (@"
typedef int Foo;
void main () {
    int n = 0;
    for (Foo one = 0; one < 10; one++) {
        n++;
    }
    assertAreEqual (10, n);
}");
        }
    }
}
