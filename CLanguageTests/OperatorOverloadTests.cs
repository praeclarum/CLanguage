using System.Linq;
using CLanguage.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class OperatorOverloadTests : TestsBase
    {
        [TestMethod]
        public void ParseMemberOperatorPlus ()
        {
            var tu = Parse (@"
struct V {
    int x;
    int operator+(int other);
};
void main() {}
");
            var structSpec = tu.Statements.OfType<MultiDeclaratorStatement> ()
                .SelectMany (s => s.Specifiers.TypeSpecifiers)
                .FirstOrDefault (ts => ts.Kind == TypeSpecifierKind.Struct && ts.Name == "V");
            Assert.IsNotNull (structSpec, "struct V should be found");
            Assert.IsNotNull (structSpec.Body, "struct V should have a body");
            var opDecl = structSpec.Body.Statements.OfType<MultiDeclaratorStatement> ()
                .Where (m => {
                    var d = m.InitDeclarators?.FirstOrDefault ()?.Declarator;
                    return GetDeclaredIdentifier (d) == "operator+";
                })
                .FirstOrDefault ();
            Assert.IsNotNull (opDecl, "operator+ should be declared as a member of V");
        }

        [TestMethod]
        public void ParseExternalOperatorPlus ()
        {
            var tu = Parse (@"
struct V { int x; };
V V::operator+(V other);
void main() {}
");
            Assert.IsNotNull (tu);
            var funcDecl = tu.Statements.OfType<MultiDeclaratorStatement> ()
                .Where (m => {
                    var d = m.InitDeclarators?.FirstOrDefault ()?.Declarator;
                    return GetDeclaredIdentifier (d) == "operator+";
                })
                .FirstOrDefault ();
            Assert.IsNotNull (funcDecl, "V::operator+ should be parsed as an external declaration");
        }

        [TestMethod]
        public void ParseOperatorEquals ()
        {
            Parse (@"
struct V {
    int x;
    bool operator==(int other);
};
void main() {}
");
        }

        [TestMethod]
        public void ParseOperatorSubscript ()
        {
            Parse (@"
struct V {
    int data[10];
    int operator[](int index);
};
void main() {}
");
        }

        [TestMethod]
        public void ParseFreeStandingOperator ()
        {
            var tu = Parse (@"
struct V { int x; };
V operator+(V a, V b);
void main() {}
");
            Assert.IsNotNull (tu);
            var funcDecl = tu.Statements.OfType<MultiDeclaratorStatement> ()
                .Where (m => {
                    var d = m.InitDeclarators?.FirstOrDefault ()?.Declarator;
                    return GetDeclaredIdentifier (d) == "operator+";
                })
                .FirstOrDefault ();
            Assert.IsNotNull (funcDecl, "Free-standing operator+ should be parsed");
        }

        [TestMethod]
        public void ParseMultipleOperators ()
        {
            Parse (@"
struct V {
    int x;
    int operator+(int other);
    int operator-(int other);
    int operator*(int other);
    bool operator==(int other);
    bool operator!=(int other);
    bool operator<(int other);
    int operator[](int i);
    int operator+=(int other);
};
void main() {}
");
        }

        [TestMethod]
        public void ParseOperatorCallParens ()
        {
            Parse (@"
struct Functor {
    int operator()(int x);
};
void main() {}
");
        }

        [TestMethod]
        public void ParseOperatorBitwiseAndShift ()
        {
            Parse (@"
struct V {
    int x;
    int operator&(int other);
    int operator|(int other);
    int operator^(int other);
    int operator<<(int other);
    int operator>>(int other);
    int operator~();
};
void main() {}
");
        }

        [TestMethod]
        public void ParseOperatorIncDec ()
        {
            Parse (@"
struct V {
    int x;
    int operator++();
    int operator--();
};
void main() {}
");
        }

        [TestMethod]
        public void ParseOperatorAssignment ()
        {
            Parse (@"
struct V {
    int x;
    int operator=(int other);
    int operator+=(int other);
    int operator-=(int other);
    int operator*=(int other);
    int operator/=(int other);
};
void main() {}
");
        }

        [TestMethod]
        public void ParseOperatorWithSelfTypeAtTopLevel ()
        {
            // Verify that operator declarations with struct types work at top level
            var tu = Parse (@"
struct V { int x; };
V operator+(V a, V b);
bool operator==(V a, V b);
V operator-(V a, V b);
void main() {}
");
            Assert.IsNotNull (tu);
            var opNames = tu.Statements.OfType<MultiDeclaratorStatement> ()
                .Select (m => GetDeclaredIdentifier (m.InitDeclarators?.FirstOrDefault ()?.Declarator))
                .Where (n => n != null && n.StartsWith ("operator"))
                .ToList ();
            Assert.AreEqual (3, opNames.Count, "Should have 3 operator declarations");
            CollectionAssert.Contains (opNames, "operator+");
            CollectionAssert.Contains (opNames, "operator==");
            CollectionAssert.Contains (opNames, "operator-");
        }

        [TestMethod]
        public void ParseOperatorContextForExternalDefinition ()
        {
            // Verify that V::operator+ produces correct Context/Identifier
            var tu = Parse (@"
struct V { int x; };
V V::operator+(V other);
void main() {}
");
            var funcDecl = tu.Statements.OfType<MultiDeclaratorStatement> ()
                .SelectMany (m => m.InitDeclarators ?? Enumerable.Empty<InitDeclarator> ())
                .Select (id => id.Declarator)
                .Select (d => FindIdentifierDeclarator (d))
                .FirstOrDefault (id => id != null && id.Identifier == "operator+");
            Assert.IsNotNull (funcDecl, "Should find operator+ declarator");
            Assert.AreEqual (1, funcDecl.Context.Count, "Context should have 1 entry");
            Assert.AreEqual ("V", funcDecl.Context[0], "Context should be V");
            Assert.AreEqual ("operator+", funcDecl.Identifier, "Identifier should be operator+");
        }

        static string GetDeclaredIdentifier (Declarator d)
        {
            while (d != null) {
                if (d is IdentifierDeclarator id)
                    return id.Identifier;
                d = d.InnerDeclarator;
            }
            return null;
        }

        static IdentifierDeclarator FindIdentifierDeclarator (Declarator d)
        {
            while (d != null) {
                if (d is IdentifierDeclarator id)
                    return id;
                d = d.InnerDeclarator;
            }
            return null;
        }

        [TestMethod]
        public void MemberOperatorPlus ()
        {
            var code = @"
struct V {
    int x;
    V operator+(V other);
};
V V::operator+(V other) {
    V r;
    r.x = this->x + other.x;
    return r;
}
void main() {
    V a; a.x = 3;
    V b; b.x = 4;
    V c = a + b;
    assertAreEqual(7, c.x);
}";
            Run (code);
        }

        [TestMethod]
        public void OperatorEquals ()
        {
            var code = @"
struct V {
    int x;
    bool operator==(V other);
};
bool V::operator==(V other) { return this->x == other.x; }
void main() {
    V a; a.x = 5;
    V b; b.x = 5;
    V c; c.x = 6;
    assertAreEqual(1, a == b);
    assertAreEqual(0, a == c);
}";
            Run (code);
        }

        [TestMethod]
        public void ChainedOperators ()
        {
            var code = @"
struct V {
    int x;
    V operator+(V other);
};
V V::operator+(V other) {
    V r;
    r.x = this->x + other.x;
    return r;
}
void main() {
    V a; a.x = 1;
    V b; b.x = 2;
    V c; c.x = 3;
    V d = a + b + c;
    assertAreEqual(6, d.x);
}";
            Run (code);
        }

        [TestMethod]
        public void FreeStandingOperatorExecution ()
        {
            var code = @"
struct V { int x; };
V operator+(V a, V b) {
    V r;
    r.x = a.x + b.x;
    return r;
}
void main() {
    V a; a.x = 10;
    V b; b.x = 20;
    V c = a + b;
    assertAreEqual(30, c.x);
}";
            Run (code);
        }

        [TestMethod]
        public void MixedTypeOperator ()
        {
            var code = @"
struct V {
    int x;
    V operator+(int n);
};
V V::operator+(int n) {
    V r;
    r.x = this->x + n;
    return r;
}
void main() {
    V a; a.x = 5;
    V b = a + 10;
    assertAreEqual(15, b.x);
}";
            Run (code);
        }

        [TestMethod]
        public void OperatorSubscriptExecution ()
        {
            var code = @"
struct Vec {
    int data[3];
    int operator[](int i);
};
int Vec::operator[](int i) {
    return this->data[i];
}
void main() {
    Vec v;
    v.data[0] = 10;
    v.data[1] = 20;
    v.data[2] = 30;
    assertAreEqual(20, v[1]);
}";
            Run (code);
        }

        [TestMethod]
        public void ComparisonOperatorsExecution ()
        {
            var code = @"
struct V {
    int x;
    bool operator<(V other);
    bool operator>(V other);
    bool operator!=(V other);
};
bool V::operator<(V other) { return this->x < other.x; }
bool V::operator>(V other) { return this->x > other.x; }
bool V::operator!=(V other) { return this->x != other.x; }
void main() {
    V a; a.x = 3;
    V b; b.x = 5;
    assertAreEqual(1, a < b);
    assertAreEqual(0, a > b);
    assertAreEqual(1, a != b);
}";
            Run (code);
        }

        [TestMethod]
        public void UnaryOperatorMinus ()
        {
            var code = @"
struct V {
    int x;
    V operator-();
};
V V::operator-() {
    V r;
    r.x = -(this->x);
    return r;
}
void main() {
    V a; a.x = 5;
    V b = -a;
    assertAreEqual(-5, b.x);
}";
            Run (code);
        }

        [TestMethod]
        public void NoOperatorOverloadError ()
        {
            // Struct without operator+ should still give error 19
            // Error 30 cascades from attempting to cast struct to arithmetic type
            var code = @"
struct V { int x; };
void main() {
    V a; a.x = 1;
    V b; b.x = 2;
    V c = a + b;
}";
            Run (code, 19, 30);
        }

        [TestMethod]
        public void AllArithmeticOperators ()
        {
            var code = @"
struct V {
    int x;
    V operator+(V o);
    V operator-(V o);
    V operator*(V o);
    V operator/(V o);
    V operator%(V o);
};
V V::operator+(V o) { V r; r.x = this->x + o.x; return r; }
V V::operator-(V o) { V r; r.x = this->x - o.x; return r; }
V V::operator*(V o) { V r; r.x = this->x * o.x; return r; }
V V::operator/(V o) { V r; r.x = this->x / o.x; return r; }
V V::operator%(V o) { V r; r.x = this->x % o.x; return r; }
void main() {
    V a; a.x = 20;
    V b; b.x = 3;
    V r1 = a + b; assertAreEqual(23, r1.x);
    V r2 = a - b; assertAreEqual(17, r2.x);
    V r3 = a * b; assertAreEqual(60, r3.x);
    V r4 = a / b; assertAreEqual(6, r4.x);
    V r5 = a % b; assertAreEqual(2, r5.x);
}";
            Run (code);
        }

        [TestMethod]
        public void ConstRefOperator ()
        {
            var code = @"
struct V {
    int x;
    V operator+(const V& other);
};
V V::operator+(const V& other) {
    V r;
    r.x = this->x + other.x;
    return r;
}
void main() {
    V a; a.x = 3;
    V b; b.x = 4;
    V c = a + b;
    assertAreEqual(7, c.x);
}";
            Run (code);
        }

        [TestMethod]
        public void ConstRefComparisonOperator ()
        {
            var code = @"
struct V {
    int x;
    bool operator==(const V& other);
    bool operator<(const V& other);
};
bool V::operator==(const V& other) { return this->x == other.x; }
bool V::operator<(const V& other) { return this->x < other.x; }
void main() {
    V a; a.x = 5;
    V b; b.x = 5;
    V c; c.x = 10;
    assertAreEqual(1, a == b);
    assertAreEqual(0, a == c);
    assertAreEqual(1, a < c);
    assertAreEqual(0, c < a);
}";
            Run (code);
        }

        [TestMethod]
        public void ClassWithCtorAndOperator ()
        {
            // Class with constructor and operator declared in body, defined externally
            var code = @"
class C {
public:
    int x;
    C(int x);
    C operator+(C other);
};
C::C(int x) { this->x = x; }
C C::operator+(C other) {
    C r(this->x + other.x);
    return r;
}
void main() {
    C a(3);
    C b(4);
    C c = a + b;
    assertAreEqual(7, c.x);
}";
            Run (code);
        }
    }
}
