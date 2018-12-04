using System;
using System.Linq;
using CLanguage.Interpreter;
using CLanguage.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CLanguage.CLanguageService;

namespace CLanguage.Tests
{
    [TestClass]
    public class EnumTests : TestsBase
    {
        [TestMethod]
        public void NamedUnnumbered ()
        {
            Run (@"
enum Numbers { ZERO, ONE, TWO };
void main() {
    assertAreEqual(0, ZERO);
    assertAreEqual(1, ONE);
    assertAreEqual(2, TWO);
}
");
        }

        [TestMethod]
        public void NamedNumbered ()
        {
            Run (@"
enum Numbers { ONE = 1, ZERO = 0, H = 100, X, Y = 1000, Z };
void main() {
    assertAreEqual(1, ONE);
    assertAreEqual(0, ZERO);
    assertAreEqual(100, H);
    assertAreEqual(101, X);
    assertAreEqual(1000, Y);
    assertAreEqual(1001, Z);
}
");
        }

        [TestMethod]
        public void UnnamedNumbered ()
        {
            Run (@"
enum { ONE = 1, ZERO = 0, H = 100, X, Y = -1000, Z, V25 = 2 * 10 + 5 };
void main() {
    assertAreEqual(1, ONE);
    assertAreEqual(0, ZERO);
    assertAreEqual(100, H);
    assertAreEqual(101, X);
    assertAreEqual(-1000, Y);
    assertAreEqual(-999, Z);
    assertAreEqual(25, V25);
}
");
        }

        [TestMethod, Ignore]
        public void UnnamedRecursive ()
        {
            Run (@"
enum { ZERO, ONE, OTHER_ONE = ONE, THREE = 2 + ONE };
void main() {
    assertAreEqual(1, ONE);
    assertAreEqual(OTHER_ONE, OTHER_ONE);
    assertAreEqual(3, THREE);
}
");
        }

        [TestMethod]
        public void GlobalInit ()
        {
            Run (@"
enum Numbers { ZERO, ONE };
enum Numbers one = ONE;
void main() {
    assertAreEqual(1, one);
}
");
        }


    }
}

