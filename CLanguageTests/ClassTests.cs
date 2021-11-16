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
    c.x = 42;
    c.y = 1000;
    int z = c.x + c.y + 5000;
    assertAreEqual(42, c.x);
    assertAreEqual(1000, c.y);
    assertAreEqual(6042, z);
}
");
        }

        [TestMethod]
        public void LocalFieldReadAndWrite ()
        {
            Run (@"
class C {
public:
    int x;
    int y;
};
void main() {
    C c;
    c.x = 42;
    c.y = 1000;
    int z = c.x + c.y + 5000;
    assertAreEqual(42, c.x);
    assertAreEqual(1000, c.y);
    assertAreEqual(6042, z);
}
");
        }

        [TestMethod]
        public void GlobalConstructor ()
        {
            Run (@"
class C {
public:
    int x;
    C(int x);
};
C::C(int x) { this->x = x; }
C c(42);
void main() {
    assertAreEqual(42, c->x);
}
");
        }

        [TestMethod]
        public void LocalConstructor ()
        {
            Run (@"
class C {
public:
    int x;
    C(int x);
};
C::C(int x) { this->x = x; }
void main() {
    C c(42);
    assertAreEqual(42, c->x);
}
");
        }

        [TestMethod]
        public void LocalDestructor ()
        {
            Run (@"
class C {
    int x;
public:
    ~C();
};
C::~C() { this->x = 0; }
void main() {
    C c;
}
");
        }
    }
}
