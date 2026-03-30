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

        [TestMethod]
        public void AndWithFunctionCalls ()
        {
            // Issue #51: function calls in && expressions produce wrong results
            Run (@"
bool returnsFalse () { return false; }
bool returnsTrue () { return true; }
void main () {
    assertBoolsAreEqual (false, returnsFalse() && returnsTrue());
    assertBoolsAreEqual (false, returnsTrue() && returnsFalse());
    assertBoolsAreEqual (true, returnsTrue() && returnsTrue());
    assertBoolsAreEqual (false, returnsFalse() && returnsFalse());
}");
        }

        [TestMethod]
        public void OrWithFunctionCalls ()
        {
            Run (@"
bool returnsFalse () { return false; }
bool returnsTrue () { return true; }
void main () {
    assertBoolsAreEqual (true, returnsFalse() || returnsTrue());
    assertBoolsAreEqual (true, returnsTrue() || returnsFalse());
    assertBoolsAreEqual (true, returnsTrue() || returnsTrue());
    assertBoolsAreEqual (false, returnsFalse() || returnsFalse());
}");
        }

        [TestMethod]
        public void AndShortCircuitPreventsRightSideEffects ()
        {
            // When left side of && is false, right side must NOT execute
            Run (@"
int x = 0;
void sideEffect () { x = 1; }
bool returnsFalse () { return false; }
void main () {
    bool result = returnsFalse() && (sideEffect(), true);
    assertBoolsAreEqual (false, result);
    assertAreEqual (0, x);
}");
        }

        [TestMethod]
        public void AndNoShortCircuitExecutesRightSide ()
        {
            // When left side of && is true, right side MUST execute
            Run (@"
int x = 0;
void sideEffect () { x = 1; }
bool returnsTrue () { return true; }
void main () {
    bool result = returnsTrue() && (sideEffect(), true);
    assertBoolsAreEqual (true, result);
    assertAreEqual (1, x);
}");
        }

        [TestMethod]
        public void OrShortCircuitPreventsRightSideEffects ()
        {
            // When left side of || is true, right side must NOT execute
            Run (@"
int x = 0;
void sideEffect () { x = 1; }
bool returnsTrue () { return true; }
void main () {
    bool result = returnsTrue() || (sideEffect(), true);
    assertBoolsAreEqual (true, result);
    assertAreEqual (0, x);
}");
        }

        [TestMethod]
        public void OrNoShortCircuitExecutesRightSide ()
        {
            // When left side of || is false, right side MUST execute
            Run (@"
int x = 0;
void sideEffect () { x = 1; }
bool returnsFalse () { return false; }
void main () {
    bool result = returnsFalse() || (sideEffect(), true);
    assertBoolsAreEqual (true, result);
    assertAreEqual (1, x);
}");
        }

        [TestMethod]
        public void NestedAndOr ()
        {
            Run (@"
bool returnsFalse () { return false; }
bool returnsTrue () { return true; }
void main () {
    assertBoolsAreEqual (true, returnsTrue() && returnsTrue() && returnsTrue());
    assertBoolsAreEqual (false, returnsTrue() && returnsFalse() && returnsTrue());
    assertBoolsAreEqual (false, returnsFalse() && returnsTrue() && returnsTrue());
    assertBoolsAreEqual (true, returnsFalse() || returnsFalse() || returnsTrue());
    assertBoolsAreEqual (false, returnsFalse() || returnsFalse() || returnsFalse());
    assertBoolsAreEqual (true, returnsTrue() || returnsFalse() || returnsFalse());
}");
        }

        [TestMethod]
        public void MixedAndOrWithFunctions ()
        {
            Run (@"
bool returnsFalse () { return false; }
bool returnsTrue () { return true; }
void main () {
    assertBoolsAreEqual (true, (returnsTrue() || returnsFalse()) && returnsTrue());
    assertBoolsAreEqual (false, (returnsFalse() && returnsTrue()) || returnsFalse());
    assertBoolsAreEqual (true, returnsFalse() || (returnsTrue() && returnsTrue()));
    assertBoolsAreEqual (false, returnsTrue() && (returnsFalse() || returnsFalse()));
}");
        }

        [TestMethod]
        public void BoolFunctionWithNotAndAnd ()
        {
            // Exact pattern from issue #51: isX() && !isY()
            Run (@"
bool isReadingI2C () { return false; }
bool availableI2C () { return true; }
void main () {
    assertBoolsAreEqual (false, isReadingI2C() && !availableI2C());
    assertBoolsAreEqual (false, !isReadingI2C() && availableI2C() && isReadingI2C());
    assertBoolsAreEqual (true, !isReadingI2C() && availableI2C());
    assertBoolsAreEqual (true, !isReadingI2C() || availableI2C());
}");
        }

        [TestMethod]
        public void NonBooleanTypesInLogicalExpressions ()
        {
            Run (@"
void main () {
    int x = 5;
    int y = 0;
    int z = 3;
    assertBoolsAreEqual (false, x && y);
    assertBoolsAreEqual (true, x && z);
    assertBoolsAreEqual (true, x || y);
    assertBoolsAreEqual (false, y && x);
    assertBoolsAreEqual (true, y || x);
}");
        }

        [TestMethod]
        public void LogicalExpressionsInIfConditions ()
        {
            Run (@"
bool returnsFalse () { return false; }
bool returnsTrue () { return true; }
void main () {
    int result = 0;
    if (returnsTrue() && returnsTrue()) { result = 1; }
    assertAreEqual (1, result);

    result = 0;
    if (returnsFalse() && returnsTrue()) { result = 1; }
    assertAreEqual (0, result);

    result = 0;
    if (returnsFalse() || returnsTrue()) { result = 1; }
    assertAreEqual (1, result);

    result = 0;
    if (returnsFalse() || returnsFalse()) { result = 1; }
    assertAreEqual (0, result);
}");
        }

        [TestMethod]
        public void LogicalExpressionsInWhileConditions ()
        {
            Run (@"
int counter = 0;
bool keepGoing () { return counter < 3; }
bool alwaysTrue () { return true; }
void main () {
    while (keepGoing() && alwaysTrue()) {
        counter = counter + 1;
    }
    assertAreEqual (3, counter);
}");
        }

        [TestMethod]
        public void AndOrResultUsedInAssignment ()
        {
            Run (@"
bool returnsFalse () { return false; }
bool returnsTrue () { return true; }
void main () {
    bool a = returnsTrue() && returnsFalse();
    bool b = returnsTrue() || returnsFalse();
    bool c = returnsFalse() && returnsTrue();
    bool d = returnsFalse() || returnsTrue();
    assertBoolsAreEqual (false, a);
    assertBoolsAreEqual (true, b);
    assertBoolsAreEqual (false, c);
    assertBoolsAreEqual (true, d);
}");
        }

        [TestMethod]
        public void RepeatedLogicalOpsInLoop ()
        {
            // Stress test: ensure repeated evaluations in a loop don't corrupt the stack
            Run (@"
bool returnsFalse () { return false; }
bool returnsTrue () { return true; }
void main () {
    int i;
    for (i = 0; i < 10; i = i + 1) {
        assertBoolsAreEqual (false, returnsFalse() && returnsTrue());
        assertBoolsAreEqual (true, returnsTrue() || returnsFalse());
        assertBoolsAreEqual (true, returnsTrue() && !returnsFalse());
        assertBoolsAreEqual (false, returnsFalse() || returnsFalse());
    }
}");
        }
    }
}
