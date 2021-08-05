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

        [TestMethod]
        public void Visibility ()
        {
            Run (@"
class C {
    int x;
public:
    int y;
};
C c;
void main() {
    int z = c.y;
}
");
        }

        [TestMethod]
        public void FieldReadAndWrite ()
        {
            Run (@"
class C {
public:
    int x;
    int y;
};
C c;
void main() {
    int z;
    c.x = 42;
    c.y = 1000;
    z = c.x + c.y + 5000;
    assertAreEqual(42, c.x);
    assertAreEqual(1000, c.y);
    assertAreEqual(6042, z);
}
");
        }

        [TestMethod, Ignore]
        public void InlineConstructor ()
        {
            Run (@"
class C {
    int x;
public:
    C(int x) { this->x = x; }
    int getX() { return x; }
};
C c(42);
void main() {
    assertAreEqual(42, c.getX());
}
");
        }

    }
}
