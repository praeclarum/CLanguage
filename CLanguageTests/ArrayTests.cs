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
            i.Run ();
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

        [TestMethod]
        public void GlobalInitFloatsToInts ()
        {
            var i = Run (@"
int a[2] = { 2.0f, 3.0f };
void main () {
    assertAreEqual (2, a[0]);
    assertAreEqual (3, a[1]);
}");
        }

        [TestMethod]
        public void LocalInitFloatsToInts ()
        {
            var i = Run (@"
void main () {
    int a[2] = { 2.0f, 3.0f };
    assertAreEqual (2, a[0]);
    assertAreEqual (3, a[1]);
}");
        }

        [TestMethod]
        public void LocalInitDoublesToInts ()
        {
            var i = Run (@"
void main () {
    int a[3] = { 1.5, 2.9, 3.1 };
    assertAreEqual (1, a[0]);
    assertAreEqual (2, a[1]);
    assertAreEqual (3, a[2]);
}");
        }

        [TestMethod]
        public void GlobalInitMultidimensional ()
        {
            var i = Run (@"
int a[2][3] = { {1, 2, 3}, {4, 5, 6} };
void main () {
    assertAreEqual (1, a[0][0]);
    assertAreEqual (2, a[0][1]);
    assertAreEqual (3, a[0][2]);
    assertAreEqual (4, a[1][0]);
    assertAreEqual (5, a[1][1]);
    assertAreEqual (6, a[1][2]);
}");
        }

        [TestMethod]
        public void LocalInitMultidimensional ()
        {
            var i = Run (@"
void main () {
    int a[2][3] = { {10, 20, 30}, {40, 50, 60} };
    assertAreEqual (10, a[0][0]);
    assertAreEqual (20, a[0][1]);
    assertAreEqual (30, a[0][2]);
    assertAreEqual (40, a[1][0]);
    assertAreEqual (50, a[1][1]);
    assertAreEqual (60, a[1][2]);
}");
        }

        [TestMethod]
        public void LocalInitIntsToFloats ()
        {
            var i = Run (@"
void main () {
    float a[2] = { 1, 2 };
    assertFloatsAreEqual (1.0f, a[0]);
    assertFloatsAreEqual (2.0f, a[1]);
}");
        }
    }
}
