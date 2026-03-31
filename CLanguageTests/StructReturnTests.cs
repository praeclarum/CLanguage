using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class StructReturnTests : TestsBase
    {
        [TestMethod]
        public void ReturnStructByValue ()
        {
            Run (@"
struct Point { int x; int y; };
Point make(int x, int y) {
    Point p;
    p.x = x;
    p.y = y;
    return p;
}
void main() {
    Point p = make(3, 4);
    assertAreEqual(3, p.x);
    assertAreEqual(4, p.y);
}");
        }

        [TestMethod]
        public void ReturnStructFromNestedCall ()
        {
            Run (@"
struct Point { int x; int y; };
Point make(int x, int y) {
    Point p;
    p.x = x;
    p.y = y;
    return p;
}
Point add(Point a, Point b) {
    Point r;
    r.x = a.x + b.x;
    r.y = a.y + b.y;
    return r;
}
void main() {
    Point p = add(make(1, 2), make(3, 4));
    assertAreEqual(4, p.x);
    assertAreEqual(6, p.y);
}");
        }

        [TestMethod]
        public void ChainedStructReturn ()
        {
            Run (@"
struct V { int x; };
V make(int x) { V v; v.x = x; return v; }
V add(V a, V b) { V r; r.x = a.x + b.x; return r; }
void main() {
    V result = add(add(make(1), make(2)), make(3));
    assertAreEqual(6, result.x);
}");
        }

        [TestMethod]
        public void AccessFieldOfReturnedStruct ()
        {
            Run (@"
struct Point { int x; int y; };
Point make(int x, int y) {
    Point p;
    p.x = x;
    p.y = y;
    return p;
}
void main() {
    int x = make(5, 10).x;
    assertAreEqual(5, x);
}");
        }

        [TestMethod]
        public void ReturnStructWithSingleField ()
        {
            Run (@"
struct Wrapper { int value; };
Wrapper wrap(int v) { Wrapper w; w.value = v; return w; }
void main() {
    Wrapper w = wrap(42);
    assertAreEqual(42, w.value);
}");
        }
    }
}
