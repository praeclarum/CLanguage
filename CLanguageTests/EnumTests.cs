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
enum Numbers { ZERO, ONE, TWO }; \
void main() {
    assertAreEqual(0, ZERO);
    assertAreEqual(0, ONE);
    assertAreEqual(0, TWO);
}
");
        }
    }
}

