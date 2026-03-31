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
    }
}
