using System;
using System.Linq;
using CLanguage.Interpreter;
using CLanguage.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CLanguage.CLanguageService;

namespace CLanguage.Tests
{
    [TestClass]
    public class AssignTests : TestsBase
    {
        [TestMethod]
        public void GlobalAssignBits ()
        {
            Run (@"
int x = 0;
void main() {
    x |= 0xCC;
    x &= 0xF0;
    x ^= 0xFF;
    assertAreEqual(63, x);
}
");
        }

        [TestMethod]
        public void GlobalAssignDoubles ()
        {
            Run (@"
double x = 0.0;
void main() {
    x += 100;
    x -= 50;
    x /= 10;
    x *= 1000000.0;
    assertDoublesAreEqual(5000000.0, x);
}
");
        }

        [TestMethod]
        public void GlobalAssignLogic ()
        {
            Run (@"
bool x = false;
void main() {
    x ||= true;
    x &&= true;
    assertBoolsAreEqual(true, x);
}
");
        }

        [TestMethod]
        public void GlobalAssign ()
        {
            Run (@"
int x = 0;
void main() {
    x = 1234;
    assertAreEqual(1234, x);
}
");
        }

        [TestMethod]
        public void GlobalAfterArray ()
        {
            Run (@"
int a[] = {111, 222};
int x = 0;
void main() {
    x = 1234;
    int i = 0;
    for (i = 0; i < 10; i++) {
    }
    assertAreEqual(1234, x);
    assertAreEqual(10, i);
}
");
        }
    }
}

