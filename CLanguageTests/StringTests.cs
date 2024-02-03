using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;

namespace CLanguage.Tests
{
    [TestClass]
    public class StringTests
    {
        CInterpreter Run (string code)
        {
            var fullCode = "void start() { __cinit(); main(); } " + code;
            var i = CLanguageService.CreateInterpreter (fullCode, new ArduinoTestMachineInfo (), printer: new TestPrinter ());
            i.Reset ("start");
            i.Run ();
            return i;
        }

        [TestMethod]
        public void SingleChar ()
        {
            var i = Run (@"
char f = 'f';
void main () {
    assertAreEqual ('f', f);
}");
        }

        [TestMethod]
        public void NullTerminated ()
        {
            var i = Run (@"
char *bar = ""bar"";
void main () {
    assertAreEqual ('b', bar[0]);
    assertAreEqual ('a', bar[1]);
    assertAreEqual ('r', bar[2]);
    assertAreEqual (0, bar[3]);
}");
        }

        [TestMethod]
        public void NullTerminatedEmpty ()
        {
            var i = Run (@"
char *bar = """";
void main () {
    assertAreEqual (0, bar[0]);
}");
        }

        [TestMethod]
        public void Newline ()
        {
            var i = Run (@"
char *bar = ""b\nr"";
void main () {
    assertAreEqual ('b', bar[0]);
    assertAreEqual ('\n', bar[1]);
    assertAreEqual ('r', bar[2]);
    assertAreEqual (0, bar[3]);
}");
        }

        [TestMethod]
        public void Multiline ()
        {
            var i = Run (@"
char *bar = ""a\
b\   
c"";
void main () {
    assertAreEqual ('a', bar[0]);
    assertAreEqual ('b', bar[1]);
    assertAreEqual ('c', bar[2]);
    assertAreEqual (0, bar[3]);
}");
        }
    }
}
