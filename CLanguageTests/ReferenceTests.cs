using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Types;
using static CLanguage.CLanguageService;

namespace CLanguage.Tests
{
    [TestClass]
    public class ReferenceTests : TestsBase
    {
        [TestMethod]
        public void PassByReference ()
        {
            var code = @"
void inc(int& x) { x = x + 1; }
void main() {
    int a = 10;
    inc(a);
    assertAreEqual(11, a);
}";
            Run (code);
        }

        [TestMethod]
        public void PassStructByConstReference ()
        {
            var code = @"
struct S { int a; int b; };
int sum(const S& s) { return s.a + s.b; }
void main() {
    S s;
    s.a = 3;
    s.b = 4;
    assertAreEqual(7, sum(s));
}";
            Run (code);
        }

        [TestMethod]
        public void ReferenceDoesNotCopyStruct ()
        {
            var code = @"
struct S { int x; };
void modify(S& s) { s.x = 42; }
void main() {
    S s;
    s.x = 0;
    modify(s);
    assertAreEqual(42, s.x);
}";
            Run (code);
        }

        [TestMethod]
        public void ConstRefAcceptsRvalue ()
        {
            var code = @"
int identity(const int& x) { return x; }
void main() {
    assertAreEqual(42, identity(42));
}";
            Run (code);
        }

        [TestMethod]
        public void ReferenceModifiesCallerVariable ()
        {
            var code = @"
void swap(int& a, int& b) {
    int temp = a;
    a = b;
    b = temp;
}
void main() {
    int x = 1;
    int y = 2;
    swap(x, y);
    assertAreEqual(2, x);
    assertAreEqual(1, y);
}";
            Run (code);
        }

        [TestMethod]
        public void ReferenceTypeCreation ()
        {
            var intType = CBasicType.SignedInt;
            var refType = new CReferenceType (intType);
            Assert.AreEqual (1, refType.NumValues);
            Assert.AreEqual ("signed int&", refType.ToString ());
            Assert.AreEqual (refType, new CReferenceType (intType));
            Assert.AreNotEqual (refType, new CReferenceType (CBasicType.Float));
        }

        [TestMethod]
        public void ReferenceScoreCastTo ()
        {
            var intType = CBasicType.SignedInt;
            var refType = new CReferenceType (intType);
            // Reference to same reference: perfect
            Assert.AreEqual (1000, refType.ScoreCastTo (new CReferenceType (intType)));
            // Reference to inner type: high score
            Assert.AreEqual (900, refType.ScoreCastTo (intType));
        }
    }
}
