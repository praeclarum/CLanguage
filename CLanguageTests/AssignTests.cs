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

