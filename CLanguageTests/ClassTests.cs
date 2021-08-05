using System;
using System.Linq;
using CLanguage.Interpreter;
using CLanguage.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CLanguage.CLanguageService;

namespace CLanguage.Tests
{
    [TestClass]
    public class ClassTests : TestsBase
    {
        [TestMethod]
        public void EmptyClass ()
        {
            Run (@"
class C {
};
C c;
void main() {
    assertAreEqual(0, sizeof(c));
    assertAreEqual(0, sizeof(C));
}
");
        }

        [TestMethod]
        public void TypdefClass ()
        {
            Run (@"
typedef class C {
    int x;
} OtherC;
OtherC c;
void main() {
    assertAreEqual(2, sizeof(c));
    assertAreEqual(2, sizeof(OtherC));
}
");
        }
    }
}
