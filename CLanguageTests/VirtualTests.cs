using System;
using System.Linq;
using CLanguage.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class VirtualTests : TestsBase
    {
        [TestMethod, Ignore]
        public void BaseWithVirtualFunction ()
        {
            Run (@"
class B {
    virtual int f() { return 42; }
}

void main()
{
    B b;
    assertAreEqual(43, b.f());
}
");
        }

        [TestMethod]
        public void ParseVirtualMethodDeclaration ()
        {
            Parse (@"
class B {
    virtual int f();
};
void main() {}
");
        }

        [TestMethod]
        public void ParsePureVirtualMethod ()
        {
            Parse (@"
class B {
    virtual int f() = 0;
};
void main() {}
");
        }

        [TestMethod]
        public void ParseOverrideMethod ()
        {
            Parse (@"
class A {
    virtual int f();
};
class B : public A {
    int f() override;
};
void main() {}
");
        }

        [TestMethod]
        public void ParseVirtualOverrideMethod ()
        {
            Parse (@"
class A {
    virtual int f();
};
class B : public A {
    virtual int f() override;
};
void main() {}
");
        }

        [TestMethod]
        public void ParseInheritancePublic ()
        {
            Parse (@"
class A {
    int x;
};
class B : public A {
    int y;
};
void main() {}
");
        }

        [TestMethod]
        public void ParseInheritancePrivate ()
        {
            Parse (@"
class A {
    int x;
};
class B : private A {
    int y;
};
void main() {}
");
        }

        [TestMethod]
        public void ParseInheritanceProtected ()
        {
            Parse (@"
class A {
    int x;
};
class B : protected A {
    int y;
};
void main() {}
");
        }

        [TestMethod]
        public void ParseInheritanceNoAccess ()
        {
            Parse (@"
class A {
    int x;
};
class B : A {
    int y;
};
void main() {}
");
        }

        [TestMethod]
        public void VirtualMethodSetsIsVirtual ()
        {
            var exe = Compile (@"
class B {
    virtual int f();
};
B b;
void main() {}
");
            var bVar = exe.Globals.FirstOrDefault (g => g.Name == "b");
            Assert.IsNotNull (bVar);
            var bType = bVar!.VariableType as CStructType;
            Assert.IsNotNull (bType);
            var method = bType!.Members.OfType<CStructMethod> ().FirstOrDefault (m => m.Name == "f");
            Assert.IsNotNull (method);
            Assert.IsTrue (method!.IsVirtual);
        }

        [TestMethod]
        public void InheritanceSetsBaseType ()
        {
            var exe = Compile (@"
class A {
public:
    int x;
};
class B : public A {
public:
    int y;
};
B b;
void main() {}
");
            var bVar = exe.Globals.FirstOrDefault (g => g.Name == "b");
            Assert.IsNotNull (bVar);
            var bType = bVar!.VariableType as CStructType;
            Assert.IsNotNull (bType);
            Assert.IsNotNull (bType!.BaseType);
            Assert.AreEqual ("A", bType.BaseType!.Name);
        }

        [TestMethod]
        public void ParseMultipleVirtualMethods ()
        {
            Parse (@"
class Shape {
public:
    virtual int area();
    virtual int perimeter();
};
void main() {}
");
        }

        [TestMethod]
        public void ParsePureVirtualClass ()
        {
            Parse (@"
class Shape {
public:
    virtual int area() = 0;
    virtual int perimeter() = 0;
};
void main() {}
");
        }

        [TestMethod]
        public void ParseVirtualWithVisibility ()
        {
            Parse (@"
class B {
public:
    virtual int f();
private:
    int x;
};
void main() {}
");
        }
    }
}
