using System;
using System.Linq;
using CLanguage.Interpreter;
using CLanguage.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CLanguage.CLanguageService;

namespace CLanguage.Tests
{
	[TestClass]
	public class PreprocessorTests
	{
        CInterpreter Run (string code, params int[] expectedErrors)
        {
            var fullCode = "void start() { __cinit(); main(); } " + code;
            var printer = new TestPrinter (expectedErrors);
            var i = CLanguageService.CreateInterpreter (fullCode, new TestMachineInfo (), printer: printer);
            printer.CheckForErrors ();
            i.Reset ("start");
            i.Step ();
            return i;
        }

        [TestMethod]
        public void AssignToDefines ()
        {
            var exe = Compile (@"
#define INPUT 1
#define OUTPUT 0
#define HIGH 255
#define LOW 0

int input = INPUT;
int output = OUTPUT;
int high = HIGH;
int low = LOW;

");
            Assert.AreEqual (5, exe.Globals.Count);
        }

        [TestMethod]
        public void DefineWithSimpleArg ()
        {
            Run (@"
#define ID(x) x
void main() {
    assertAreEqual(42, ID(42));
}
");
        }

        [TestMethod]
        public void DefineParamIncompleteArgs ()
        {
            Run (@"
#define ID(x x
void main() {
    assertAreEqual(42, ID(42));
}
", 1001, 103, 2064);
        }
    }
}

