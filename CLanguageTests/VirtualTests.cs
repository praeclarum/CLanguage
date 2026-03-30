using System;
using System.Linq;
using CLanguage.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class VirtualTests : TestsBase
    {
        [TestMethod]
        public void BaseWithVirtualFunction ()
        {
            Run (@"
class B {
public:
    virtual int f();
};
int B::f() { return 42; }

void main()
{
    B b;
    assertAreEqual(42, b.f());
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

        [TestMethod]
        public void ParseMultipleBaseSpecifiers ()
        {
            // Multiple base specifiers should parse but compiler reports an error
            Compile (@"
class A { int x; };
class B { int y; };
class C : public A, public B {
    int z;
};
void main() {}
", 1500);
        }

        // ---- Phase 3: Vtable allocation and population ----

        [TestMethod]
        public void VtableGlobalAllocatedForPolymorphicType ()
        {
            var exe = Compile (@"
class B {
public:
    virtual int f();
};
int B::f() { return 42; }
B b;
void main() {}
");
            var vtableGlobal = exe.Globals.FirstOrDefault (g => g.Name == "__vtable_B");
            Assert.IsNotNull (vtableGlobal, "Vtable global should be allocated");
            Assert.IsNotNull (vtableGlobal!.InitialValue, "Vtable should have initial values");
            Assert.AreEqual (1, vtableGlobal.InitialValue!.Length, "Vtable should have 1 slot");
        }

        [TestMethod]
        public void VtableNotAllocatedForNonPolymorphicType ()
        {
            var exe = Compile (@"
class C {
public:
    int x;
};
C c;
void main() {}
");
            var vtableGlobal = exe.Globals.FirstOrDefault (g => g.Name == "__vtable_C");
            Assert.IsNull (vtableGlobal, "No vtable should be allocated for non-polymorphic type");
        }

        [TestMethod]
        public void GlobalVptrSetForPolymorphicVariable ()
        {
            var exe = Compile (@"
class B {
public:
    virtual int f();
};
int B::f() { return 42; }
B b;
void main() {}
");
            var vtableGlobal = exe.Globals.FirstOrDefault (g => g.Name == "__vtable_B");
            Assert.IsNotNull (vtableGlobal);
            var bGlobal = exe.Globals.FirstOrDefault (g => g.Name == "b");
            Assert.IsNotNull (bGlobal);
            Assert.IsNotNull (bGlobal!.InitialValue);
            Assert.AreEqual (vtableGlobal!.StackOffset, bGlobal.InitialValue![0].PointerValue,
                "b's vptr should point to B's vtable");
        }

        [TestMethod]
        public void DerivedVtableOverridesBaseSlot ()
        {
            var exe = Compile (@"
class A {
public:
    virtual int f();
};
int A::f() { return 1; }
class B : public A {
public:
    int f() override;
};
int B::f() { return 2; }
B b;
void main() {}
");
            var vtableA = exe.Globals.FirstOrDefault (g => g.Name == "__vtable_A");
            var vtableB = exe.Globals.FirstOrDefault (g => g.Name == "__vtable_B");
            Assert.IsNotNull (vtableA);
            Assert.IsNotNull (vtableB);
            Assert.IsNotNull (vtableA!.InitialValue);
            Assert.IsNotNull (vtableB!.InitialValue);
            // Both vtables have 1 slot but point to different functions
            Assert.AreEqual (1, vtableA.InitialValue!.Length);
            Assert.AreEqual (1, vtableB.InitialValue!.Length);
            Assert.AreNotEqual (vtableA.InitialValue[0].PointerValue,
                vtableB.InitialValue[0].PointerValue,
                "Derived vtable should have overridden function pointer");
        }

        // ---- Phase 3: Virtual dispatch at runtime ----

        [TestMethod]
        public void VirtualCallOnBaseObject ()
        {
            Run (@"
class B {
public:
    int x;
    virtual int f();
};
int B::f() { return 42; }
void main() {
    B b;
    b.x = 10;
    assertAreEqual(42, b.f());
}
");
        }

        [TestMethod]
        public void VirtualCallOnGlobalBaseObject ()
        {
            Run (@"
class B {
public:
    int x;
    virtual int f();
};
int B::f() { return 42; }
B b;
void main() {
    b.x = 10;
    assertAreEqual(42, b.f());
}
");
        }

        [TestMethod]
        public void VirtualCallDispatchesToDerived ()
        {
            Run (@"
class A {
public:
    int x;
    virtual int f();
};
int A::f() { return 1; }
class B : public A {
public:
    int f() override;
};
int B::f() { return 2; }
void main() {
    B b;
    assertAreEqual(2, b.f());
}
");
        }

        [TestMethod]
        public void VirtualCallInheritedMethodNotOverridden ()
        {
            Run (@"
class A {
public:
    virtual int f();
    virtual int g();
};
int A::f() { return 10; }
int A::g() { return 20; }
class B : public A {
public:
    int f() override;
};
int B::f() { return 100; }
void main() {
    B b;
    assertAreEqual(100, b.f());
    assertAreEqual(20, b.g());
}
");
        }

        [TestMethod]
        public void ThreeLevelInheritanceChain ()
        {
            Run (@"
class A {
public:
    virtual int f();
};
int A::f() { return 1; }
class B : public A {
public:
    int f() override;
};
int B::f() { return 2; }
class C : public B {
public:
    int f() override;
};
int C::f() { return 3; }
void main() {
    A a;
    B b;
    C c;
    assertAreEqual(1, a.f());
    assertAreEqual(2, b.f());
    assertAreEqual(3, c.f());
}
");
        }

        [TestMethod]
        public void VirtualCallWithFieldAccess ()
        {
            Run (@"
class A {
public:
    int x;
    virtual int getX();
};
int A::getX() { return this->x; }
class B : public A {
public:
    int getX() override;
};
int B::getX() { return this->x + 100; }
void main() {
    A a;
    a.x = 5;
    assertAreEqual(5, a.getX());
    B b;
    b.x = 5;
    assertAreEqual(105, b.getX());
}
");
        }

        [TestMethod]
        public void NonVirtualMethodOnPolymorphicType ()
        {
            Run (@"
class B {
public:
    int x;
    virtual int f();
    int g();
};
int B::f() { return 42; }
int B::g() { return 99; }
void main() {
    B b;
    b.x = 0;
    assertAreEqual(42, b.f());
    assertAreEqual(99, b.g());
}
");
        }

        [TestMethod]
        public void VirtualCallMultipleMethods ()
        {
            Run (@"
class Shape {
public:
    virtual int area();
    virtual int perimeter();
};
int Shape::area() { return 0; }
int Shape::perimeter() { return 0; }
class Rect : public Shape {
public:
    int w;
    int h;
    int area() override;
    int perimeter() override;
};
int Rect::area() { return this->w * this->h; }
int Rect::perimeter() { return 2 * (this->w + this->h); }
void main() {
    Rect r;
    r.w = 3;
    r.h = 4;
    assertAreEqual(12, r.area());
    assertAreEqual(14, r.perimeter());
}
");
        }

        [TestMethod]
        public void NonVirtualTypesUnchangedCodegen ()
        {
            // Ensure non-virtual struct produces no vtable overhead
            var exe = Compile (@"
struct Point {
    int x;
    int y;
};
Point p;
void main() {
    p.x = 1;
    p.y = 2;
    assertAreEqual(1, p.x);
    assertAreEqual(2, p.y);
}
");
            var vtableGlobal = exe.Globals.FirstOrDefault (g => g.Name.StartsWith ("__vtable_"));
            Assert.IsNull (vtableGlobal, "Non-polymorphic code should produce no vtable globals");
            // Also verify pure_virtual_called trap is not added
            var trapFunc = exe.Functions.FirstOrDefault (f => f.Name == "__pure_virtual_called");
            Assert.IsNull (trapFunc, "Non-polymorphic code should not add trap function");
        }

        // ---- Phase 4: Polymorphism through base pointer ----

        [TestMethod]
        public void PolymorphismThroughBasePointer ()
        {
            Run (@"
class Base {
public:
    virtual int value();
};
int Base::value() { return 1; }
class Derived : public Base {
public:
    int value() override;
};
int Derived::value() { return 2; }
void main() {
    Derived d;
    Base* b = &d;
    assertAreEqual(2, b->value());
}
");
        }

        [TestMethod]
        public void ThreeLevelInheritanceViaBasePointer ()
        {
            Run (@"
class A {
public:
    virtual int f();
};
int A::f() { return 1; }
class B : public A {
public:
    int f() override;
};
int B::f() { return 2; }
class C : public B {
public:
    int f() override;
};
int C::f() { return 3; }
void main() {
    C c;
    A* a = &c;
    assertAreEqual(3, a->f());
}
");
        }

        [TestMethod]
        public void PartialOverrideViaBasePointer ()
        {
            Run (@"
class Base {
public:
    virtual int f();
    virtual int g();
};
int Base::f() { return 1; }
int Base::g() { return 2; }
class Derived : public Base {
public:
    int f() override;
};
int Derived::f() { return 10; }
void main() {
    Derived d;
    Base* b = &d;
    assertAreEqual(10, b->f());
    assertAreEqual(2, b->g());
}
");
        }

        [TestMethod]
        public void MultipleVirtualMethodsViaBasePointer ()
        {
            Run (@"
class Shape {
public:
    virtual int area();
    virtual int perimeter();
};
int Shape::area() { return 0; }
int Shape::perimeter() { return 0; }
class Rect : public Shape {
public:
    int w;
    int h;
    int area() override;
    int perimeter() override;
};
int Rect::area() { return this->w * this->h; }
int Rect::perimeter() { return 2 * (this->w + this->h); }
void main() {
    Rect r;
    r.w = 3;
    r.h = 4;
    Shape* s = &r;
    assertAreEqual(12, s->area());
    assertAreEqual(14, s->perimeter());
}
");
        }

        // ---- Phase 4: Non-virtual regression ----

        [TestMethod]
        public void NonVirtualStructFieldAccess ()
        {
            Run (@"
struct Point {
    int x;
    int y;
};
void main() {
    Point p;
    p.x = 3;
    p.y = 4;
    assertAreEqual(3, p.x);
    assertAreEqual(4, p.y);
}
");
        }

        [TestMethod]
        public void NonVirtualClassWithMethods ()
        {
            Run (@"
class Counter {
public:
    int n;
    void inc();
    int get();
};
void Counter::inc() { this->n = this->n + 1; }
int Counter::get() { return this->n; }
void main() {
    Counter c;
    c.n = 0;
    c.inc();
    c.inc();
    c.inc();
    assertAreEqual(3, c.get());
}
");
        }

        // ---- Phase 4: Object layout ----

        [TestMethod]
        public void FieldAccessOnPolymorphicObject ()
        {
            Run (@"
class Base {
public:
    int x;
    virtual int getX();
};
int Base::getX() { return this->x; }
void main() {
    Base b;
    b.x = 42;
    assertAreEqual(42, b.getX());
}
");
        }

        [TestMethod]
        public void DerivedFieldsAccessible ()
        {
            Run (@"
class Base {
public:
    int x;
    virtual int f();
};
int Base::f() { return this->x; }
class Derived : public Base {
public:
    int y;
    int f() override;
};
int Derived::f() { return this->x + this->y; }
void main() {
    Derived d;
    d.x = 10;
    d.y = 20;
    assertAreEqual(30, d.f());
}
");
        }

        // ---- Phase 4: Edge cases ----

        [TestMethod]
        public void VirtualMethodWithArguments ()
        {
            Run (@"
class Base {
public:
    virtual int add(int a, int b);
};
int Base::add(int a, int b) { return a + b; }
class Derived : public Base {
public:
    int add(int a, int b) override;
};
int Derived::add(int a, int b) { return a * b; }
void main() {
    Derived d;
    Base* b = &d;
    assertAreEqual(12, b->add(3, 4));
}
");
        }

        [TestMethod]
        public void VirtualCallWithArgumentsDirect ()
        {
            Run (@"
class Base {
public:
    virtual int add(int a, int b);
};
int Base::add(int a, int b) { return a + b; }
class Derived : public Base {
public:
    int add(int a, int b) override;
};
int Derived::add(int a, int b) { return a * b; }
void main() {
    Derived d;
    assertAreEqual(12, d.add(3, 4));
}
");
        }

        [TestMethod]
        public void NewVirtualMethodInDerived ()
        {
            Run (@"
class Base {
public:
    virtual int f();
};
int Base::f() { return 1; }
class Derived : public Base {
public:
    int f() override;
    virtual int g();
};
int Derived::f() { return 2; }
int Derived::g() { return 3; }
void main() {
    Derived d;
    assertAreEqual(2, d.f());
    assertAreEqual(3, d.g());
}
");
        }

        [TestMethod]
        public void DerivedPointerToBasePointerCast ()
        {
            // Verifies that Derived* can be implicitly assigned to Base*
            Run (@"
class Base {
public:
    virtual int f();
};
int Base::f() { return 1; }
class Derived : public Base {
public:
    int f() override;
};
int Derived::f() { return 2; }
void main() {
    Derived d;
    Base* bp = &d;
    assertAreEqual(2, bp->f());
}
");
        }

        // ---- Phase 4: Compile-time error tests ----

        [TestMethod]
        public void OverrideWithoutBaseMatchIsError ()
        {
            // 'g' does not exist in Base, so 'override' should fail
            Compile (@"
class Base {
public:
    virtual int f();
};
int Base::f() { return 1; }
class Derived : public Base {
public:
    int g() override;
};
int Derived::g() { return 2; }
void main() {}
", 9000);
        }

        [TestMethod]
        public void ConcreteClassOverridingPureVirtual ()
        {
            Run (@"
class A {
public:
    virtual int f() = 0;
};
class B : public A {
public:
    int f() override;
};
int B::f() { return 42; }
void main() {
    B b;
    assertAreEqual(42, b.f());
}
");
        }

        // ---- Phase 4: Interop safety ----

        [TestMethod]
        public void NonVirtualInteropUnchanged ()
        {
            // Verify that AddGlobalReference with non-virtual type still works
            var mi = new ArduinoTestMachineInfo ();
            var helper = new InteropHelper ();
            mi.AddGlobalReference ("h", helper);
            Run (@"void main() { h.store(42); assert32AreEqual(42, h.load()); }", mi);
            Assert.AreEqual (42, helper.Value);
        }

        class InteropHelper
        {
            public int Value { get; set; }
            public void Store (int x) { Value = x; }
            public int Load () { return Value; }
        }

        [TestMethod]
        public void NonVirtualGlobalStructLayoutUnchanged ()
        {
            // Verify that non-polymorphic global struct still occupies the same layout
            var exe = Compile (@"
struct Vec2 {
    int x;
    int y;
};
Vec2 v;
void main() {
    v.x = 7;
    v.y = 8;
    assertAreEqual(7, v.x);
    assertAreEqual(8, v.y);
}
");
            var vVar = exe.Globals.FirstOrDefault (g => g.Name == "v");
            Assert.IsNotNull (vVar);
            var vType = vVar!.VariableType as CStructType;
            Assert.IsNotNull (vType);
            // Non-polymorphic: no vtable, no vptr overhead
            Assert.IsFalse (vType!.IsPolymorphic);
            Assert.AreEqual (2, vType.NumValues, "Non-polymorphic struct should have 2 value slots (x, y)");
        }
    }
}
