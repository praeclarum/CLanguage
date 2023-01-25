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
int f(char x) { return 1; }
int f(int x) { return 2; }
int f(float x) { return 3; }
int f(double x) { return 4; }
int f(unsigned long x) { return 5; }
int f(const char *x) { return 6; }
void main () {
    char msg[5];
    assertAreEqual (1, f((char)0));
    assertAreEqual (2, f(0));
    assertAreEqual (3, f(0.0f));
    assertAreEqual (4, f(0.0));
    assertAreEqual (5, f(0ul));
    assertAreEqual (6, f(""hello""));
    assertAreEqual (6, f(msg));
    assertAreEqual (1, f('h'));
}");
        }

        [TestMethod]
        public void MemberScoring ()
        {
            var i = Run (@"
void main () {
    char msg[5];
    assertAreEqual (0, test.f());
    assertAreEqual (1, test.f((char)0));
    assertAreEqual (2, test.f(0));
    assertAreEqual (22, test.f(0, 0));
    assertAreEqual (3, test.f(0.0f));
    assertAreEqual (4, test.f(0.0));
    assertAreEqual (6, test.f(""hello""));
    assertAreEqual (6, test.f(msg));
}");
        }

        [TestMethod]
        public void DefaultValue ()
        {
            var i = Run (@"
int f(int x, int y = 1000) { return x + y; }
double f(double x, double y = 3.14) { return (x + y); }
void main () {
    assertAreEqual (1000, f(0));
    assertAreEqual (999, f(-1));
    assertAreEqual (2, f(0, 2));
    assertAreEqual (1, f(-1, 2));
    assertDoublesAreEqual (3.14, f(0.0));
    assertDoublesAreEqual (2.14, f(-1.0));
    assertDoublesAreEqual (2, f(0.0, 2));
    assertDoublesAreEqual (1, f(-1.0, 2));
}");
        }

        [TestMethod]
        public void GlobalCharVsIntVariable ()
        {
            var i = Run (@"
int f(char x) { return 1; }
int f(int x) { return 2; }
void main () {
    char cval = 'F';
    int ival = 42;
    assertAreEqual (1, f(cval));
    assertAreEqual (2, f(ival));
}");
        }

        [TestMethod]
        public void MemberCharVsIntVariable ()
        {
            var i = Run (@"
void main () {
    char cval = 'F';
    int ival = 42;
    assertAreEqual (1, test.f(cval));
    assertAreEqual (2, test.f(ival));
}");
        }

        [TestMethod]
        public void PrintlnConstCharPtr ()
        {
            var i = Run (@"
void main () {
    Serial.println(""hello"");
}");
            Assert.AreEqual("hello\n", ((ArduinoTestMachineInfo)i.Executable.MachineInfo).Arduino.SerialOut.ToString());
        }
    }
}
