using System.Linq;
using CLanguage.Syntax;
using CLanguage.Interpreter;
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

        [TestMethod]
        public void InternalOperatorPlus ()
        {
            // Struct without operator in body — the operator is resolved
            // externally from the InternalFunction via TryResolveOperatorFunction.
            var mi = new TestMachineInfo ();
            mi.HeaderCode += "struct V { int x; };\n";
            mi.AddInternalFunction ("V V::operator+(V other)", interp => {
                var _this = interp.ReadThis ().PointerValue;
                var thisX = interp.Stack[_this].Int32Value;
                var otherX = interp.ReadArg (0).Int32Value;
                interp.Push (thisX + otherX);
            });

            var code = @"
void main() {
    V a; a.x = 3;
    V b; b.x = 4;
    V c = a + b;
    assertAreEqual(7, c.x);
}";
            Run (code, mi);
        }

        [TestMethod]
        public void InternalOperatorEquals ()
        {
            var mi = new TestMachineInfo ();
            mi.HeaderCode += "struct V { int x; };\n";
            mi.AddInternalFunction ("bool V::operator==(V other)", interp => {
                var _this = interp.ReadThis ().PointerValue;
                var thisX = interp.Stack[_this].Int32Value;
                var otherX = interp.ReadArg (0).Int32Value;
                interp.Push (thisX == otherX ? 1 : 0);
            });

            var code = @"
void main() {
    V a; a.x = 5;
    V b; b.x = 5;
    V c; c.x = 6;
    assertAreEqual(1, a == b);
    assertAreEqual(0, a == c);
}";
            Run (code, mi);
        }

        [TestMethod]
        public void MixedInternalAndCompiledOperators ()
        {
            var mi = new TestMachineInfo ();
            mi.HeaderCode += @"
struct V {
    int x;
    bool operator==(V other);
};
";
            // operator+ is internal (C#), operator== is compiled (C code)
            mi.AddInternalFunction ("V V::operator+(V other)", interp => {
                var _this = interp.ReadThis ().PointerValue;
                // V has 2 NumValues (int x + operator==), so read x at the right offset
                var thisX = interp.Stack[_this].Int32Value;
                var otherX = interp.ReadArg (0).Int32Value;
                // Push struct result (x value + operator== method slot)
                interp.Push (thisX + otherX);
                interp.Push (0);
            });

            var code = @"
bool V::operator==(V other) { return this->x == other.x; }
void main() {
    V a; a.x = 3;
    V b; b.x = 4;
    V c = a + b;
    assertAreEqual(7, c.x);
    V d; d.x = 7;
    assertAreEqual(1, c == d);
    assertAreEqual(0, a == b);
}";
            Run (code, mi);
        }

        [TestMethod]
        public void InternalOperatorWithBasicReturnType ()
        {
            var mi = new TestMachineInfo ();
            mi.HeaderCode += "struct V { int x; };\n";
            mi.AddInternalFunction ("int V::operator<(V other)", interp => {
                var _this = interp.ReadThis ().PointerValue;
                var thisX = interp.Stack[_this].Int32Value;
                var otherX = interp.ReadArg (0).Int32Value;
                interp.Push (thisX < otherX ? 1 : 0);
            });

            var code = @"
void main() {
    V a; a.x = 3;
    V b; b.x = 5;
    assertAreEqual(1, a < b);
    assertAreEqual(0, b < a);
}";
            Run (code, mi);
        }

        [TestMethod]
        public void InternalOperatorChained ()
        {
            var mi = new TestMachineInfo ();
            mi.HeaderCode += "struct V { int x; };\n";
            mi.AddInternalFunction ("V V::operator+(V other)", interp => {
                var _this = interp.ReadThis ().PointerValue;
                var thisX = interp.Stack[_this].Int32Value;
                var otherX = interp.ReadArg (0).Int32Value;
                interp.Push (thisX + otherX);
            });

            var code = @"
void main() {
    V a; a.x = 1;
    V b; b.x = 2;
    V c; c.x = 3;
    V d = a + b + c;
    assertAreEqual(6, d.x);
}";
            Run (code, mi);
        }

        [TestMethod]
        public void InternalOperatorWithConstRef ()
        {
            var mi = new TestMachineInfo ();
            mi.HeaderCode += "struct V { int x; };\n";
            mi.AddInternalFunction ("V V::operator+(const V& other)", interp => {
                var _this = interp.ReadThis ().PointerValue;
                var thisX = interp.Stack[_this].Int32Value;
                // const V& parameter is passed as a pointer
                var otherPtr = interp.ReadArg (0).PointerValue;
                var otherX = interp.Stack[otherPtr].Int32Value;
                interp.Push (thisX + otherX);
            });

            var code = @"
void main() {
    V a; a.x = 3;
    V b; b.x = 4;
    V c = a + b;
    assertAreEqual(7, c.x);
}";
            Run (code, mi);
        }

        [TestMethod]
        public void InternalOperatorWithStructLayout ()
        {
            // Test multi-field struct return and parameter access
            var mi = new TestMachineInfo ();
            mi.HeaderCode += "struct Vec { int x; int y; };\n";
            mi.AddInternalFunction ("Vec Vec::operator+(Vec other)", interp => {
                var _this = interp.ReadThis ().PointerValue;
                var thisX = interp.Stack[_this].Int32Value;
                var thisY = interp.Stack[_this + 1].Int32Value;
                // Multi-value struct parameter: compute base address from frame pointer
                var fp = interp.ActiveFrame!.FP;
                var paramOffset = interp.ActiveFrame.Function.FunctionType.Parameters[0].Offset;
                var otherBase = fp + paramOffset;
                var otherX = interp.Stack[otherBase].Int32Value;
                var otherY = interp.Stack[otherBase + 1].Int32Value;
                interp.Push (thisX + otherX);
                interp.Push (thisY + otherY);
            });

            var code = @"
void main() {
    Vec a; a.x = 1; a.y = 10;
    Vec b; b.x = 2; b.y = 20;
    Vec c = a + b;
    assertAreEqual(3, c.x);
    assertAreEqual(30, c.y);
}";
            Run (code, mi);
        }
        [TestMethod]
        public void ReflectionBasedOperators ()
        {
            // C# class with operator+ exposed via AddGlobalReference
            var mi = new TestMachineInfo ();
            var calc = new TestCalculator ();
            mi.AddGlobalReference ("calc", calc);

            var code = @"
void main() {
    long result = calc + 10;
    assert32AreEqual(20, result);
}";
            Run (code, mi);
        }
    }

    /// <summary>
    /// Test class with C# operators for reflection-based operator detection.
    /// </summary>
    public class TestCalculator
    {
        public static int operator+ (TestCalculator a, int b)
        {
            return b * 2;
        }

        public int Add (int a, int b)
        {
            return a + b;
        }
    }
}
