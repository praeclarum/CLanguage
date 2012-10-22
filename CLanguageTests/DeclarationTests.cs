using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#elif VS_UNIT_TESTING
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#endif

namespace CLanguage.Tests
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/C_variable_types_and_declarations
    /// </summary>
    [TestClass]
    public class DeclarationTests
    {

        TranslationUnit Parse(string code)
        {
			var report = new Report(new TestPrinter ());
            var pp = new Preprocessor(report);
            pp.AddCode("stdin", code);
            var lexer = new Lexer(pp);
            var parser = new CParser();
            return parser.ParseTranslationUnit(lexer, report);
        }

        [TestMethod]
        public void Basic()
        {
            var tu = Parse("int cat;");
            Assert.AreEqual(1, tu.Variables.Count);
            var v = tu.Variables[0];
            Assert.AreEqual("cat", v.Name);
            Assert.IsInstanceOf<CBasicType>(v.VariableType);
        }

        [TestMethod]
        public void SignednessBasic()
        {
            var tu = Parse("unsigned int x; signed int y; int z; unsigned char grey;signed char white;");
            Assert.AreEqual(5, tu.Variables.Count);
            var x = tu.Variables[0];
            var y = tu.Variables[1];
            var z = tu.Variables[2];
            var grey = tu.Variables[3];
            var white = tu.Variables[4];
            Assert.AreEqual(Signedness.Unsigned, ((CBasicType)x.VariableType).Signedness);
            Assert.AreEqual(Signedness.Signed, ((CBasicType)y.VariableType).Signedness);
            Assert.AreEqual(Signedness.Signed, ((CBasicType)z.VariableType).Signedness);
            Assert.AreEqual(Signedness.Unsigned, ((CBasicType)grey.VariableType).Signedness);
            Assert.AreEqual(Signedness.Signed, ((CBasicType)white.VariableType).Signedness);
        }

        [TestMethod]
        public void SignednessNoBasic()
        {
            var tu = Parse("unsigned x; signed y;");
            var x = (CBasicType)tu.Variables[0].VariableType;
            var y = (CBasicType)tu.Variables[1].VariableType;
            Assert.AreEqual(Signedness.Unsigned, x.Signedness);
            Assert.AreEqual("int", x.Name);
            Assert.AreEqual(Signedness.Signed, y.Signedness);
            Assert.AreEqual("int", y.Name);
        }

        [TestMethod]
        public void SizeBasic()
        {
            var tu = Parse("short int yellow;long int orange;long long int red;long brown;long double black;");
            var yellow = (CBasicType)tu.Variables[0].VariableType;
            var orange = (CBasicType)tu.Variables[1].VariableType;
            var red = (CBasicType)tu.Variables[2].VariableType;
            var brown = (CBasicType)tu.Variables[3].VariableType;
            var black = (CBasicType)tu.Variables[4].VariableType;
            Assert.AreEqual("short", yellow.Size);
            Assert.AreEqual("int", yellow.Name);
            Assert.AreEqual("long", orange.Size);
            Assert.AreEqual("int", orange.Name);
            Assert.AreEqual("long long", red.Size);
            Assert.AreEqual("int", red.Name);
            Assert.AreEqual("long", brown.Size);
            Assert.AreEqual("int", brown.Name);
            Assert.AreEqual("long", black.Size);
            Assert.AreEqual("double", black.Name);
        }

        [TestMethod]
        public void QualifiedBasic()
        {
            var tu = Parse("const int cat;");
            var v = tu.Variables[0];
            Assert.IsInstanceOf<CBasicType>(v.VariableType);
            Assert.AreEqual(TypeQualifiers.Const, v.VariableType.TypeQualifiers);
        }

        [TestMethod]
        public void Pointer()
        {
            var tu = Parse("char *square;");
            var v = tu.Variables[0];
            Assert.AreEqual("square", v.Name);
            Assert.IsInstanceOf<CPointerType>(v.VariableType);
            Assert.IsInstanceOf<CBasicType>(((CPointerType)v.VariableType).InnerType);
            Assert.AreEqual("char", ((CBasicType)((CPointerType)v.VariableType).InnerType).Name);
        }

        [TestMethod]
        public void VoidPointer()
        {
            var tu = Parse("void *triangle;");
            var v = tu.Variables[0];
            Assert.AreEqual("triangle", v.Name);
            Assert.IsInstanceOf<CPointerType>(v.VariableType);
            Assert.IsInstanceOf<CVoidType>(((CPointerType)v.VariableType).InnerType);
            Assert.IsTrue(((CPointerType)v.VariableType).InnerType.IsVoid);
        }

        [TestMethod]
        public void PointerSeparation()
        {
            var tu = Parse("long* first, second;");
            var v = tu.Variables[0];
            var v1 = tu.Variables[1];
            Assert.AreEqual("first", v.Name);
            Assert.IsInstanceOf<CPointerType>(v.VariableType);
            Assert.IsInstanceOf<CBasicType>(((CPointerType)v.VariableType).InnerType);
            Assert.AreEqual("int", ((CBasicType)((CPointerType)v.VariableType).InnerType).Name);
            Assert.AreEqual("second", v1.Name);
            Assert.IsInstanceOf<CBasicType>(v1.VariableType);
            Assert.AreEqual("int", ((CBasicType)v1.VariableType).Name);
        }

        [TestMethod]
        public void NonConstPointerToConstChar()
        {
            var tu = Parse("const char *kite;");
            var v = tu.Variables[0];
            Assert.AreEqual("kite", v.Name);
            Assert.IsInstanceOf<CPointerType>(v.VariableType);
            var pt = (CPointerType)v.VariableType;
            Assert.AreEqual(TypeQualifiers.None, pt.TypeQualifiers);
            Assert.IsInstanceOf<CBasicType>(pt.InnerType);
            Assert.AreEqual("char", ((CBasicType)pt.InnerType).Name);
            Assert.AreEqual(TypeQualifiers.Const, ((CBasicType)pt.InnerType).TypeQualifiers);
        }

        [TestMethod]
        public void ConstPointerToChar()
        {
            var tu = Parse("char * const pentagon;");
            var v = tu.Variables[0];
            Assert.AreEqual("pentagon", v.Name);
            Assert.IsInstanceOf<CPointerType>(v.VariableType);
            var pt = (CPointerType)v.VariableType;
            Assert.AreEqual(TypeQualifiers.Const, pt.TypeQualifiers);
            Assert.IsInstanceOf<CBasicType>(pt.InnerType);
            Assert.AreEqual("char", ((CBasicType)pt.InnerType).Name);
            Assert.AreEqual(TypeQualifiers.None, ((CBasicType)pt.InnerType).TypeQualifiers);
        }

        [TestMethod]
        public void ConstPointerToConstChar1()
        {
            var tu = Parse("char const * const hexagon;");
            var v = tu.Variables[0];
            Assert.AreEqual("hexagon", v.Name);
            Assert.IsInstanceOf<CPointerType>(v.VariableType);
            var pt = (CPointerType)v.VariableType;
            Assert.AreEqual(TypeQualifiers.Const, pt.TypeQualifiers);
            Assert.IsInstanceOf<CBasicType>(pt.InnerType);
            Assert.AreEqual("char", ((CBasicType)pt.InnerType).Name);
            Assert.AreEqual(TypeQualifiers.Const, ((CBasicType)pt.InnerType).TypeQualifiers);
        }

        [TestMethod]
        public void ConstPointerToConstChar2()
        {
            var tu = Parse("const char * const hexagon;");
            var v = tu.Variables[0];
            Assert.AreEqual("hexagon", v.Name);
            Assert.IsInstanceOf<CPointerType>(v.VariableType);
            var pt = (CPointerType)v.VariableType;
            Assert.AreEqual(TypeQualifiers.Const, pt.TypeQualifiers);
            Assert.IsInstanceOf<CBasicType>(pt.InnerType);
            Assert.AreEqual("char", ((CBasicType)pt.InnerType).Name);
            Assert.AreEqual(TypeQualifiers.Const, ((CBasicType)pt.InnerType).TypeQualifiers);
        }

        [TestMethod]
        public void PointerToPointer()
        {
            var tu = Parse("char **septagon;");
            var v = tu.Variables[0];
            Assert.AreEqual("septagon", v.Name);
            Assert.IsInstanceOf<CPointerType>(v.VariableType);
            var pt = (CPointerType)v.VariableType;
            Assert.IsInstanceOf<CPointerType>(pt.InnerType);
            var pt1 = (CPointerType)pt.InnerType;
            Assert.IsInstanceOf<CBasicType>(pt1.InnerType);
            Assert.AreEqual("char", ((CBasicType)pt1.InnerType).Name);
        }

        [TestMethod]
        public void PointerToConstPointerToConstBasic()
        {
            var tu = Parse("unsigned long const int * const *octagon;");
            var v = tu.Variables[0];
            Assert.AreEqual("octagon", v.Name);

            Assert.IsInstanceOf<CPointerType>(v.VariableType);
            var pt = (CPointerType)v.VariableType;
            Assert.AreEqual(TypeQualifiers.None, pt.TypeQualifiers);

            Assert.IsInstanceOf<CPointerType>(pt.InnerType);
            var pt1 = (CPointerType)pt.InnerType;
            Assert.AreEqual(TypeQualifiers.Const, pt1.TypeQualifiers);

            Assert.IsInstanceOf<CBasicType>(pt1.InnerType);
            var b = (CBasicType)pt1.InnerType;
            Assert.AreEqual(TypeQualifiers.Const, b.TypeQualifiers);
            Assert.AreEqual("int", b.Name);
        }

        [TestMethod]
        public void Array()
        {
            var tu = Parse("int cat[10];");
            var t = (CArrayType)tu.Variables[0].VariableType;
            Assert.IsInstanceOf<ConstantExpression>(t.LengthExpression);
            Assert.AreEqual(10, ((ConstantExpression)t.LengthExpression).Value);
            Assert.IsInstanceOf<CBasicType>(t.ElementType);
        }

        [TestMethod]
        public void ArrayOfArrays()
        {
            var tu = Parse("double dog[5][12];");
            var a = (CArrayType)tu.Variables[0].VariableType;
            Assert.IsInstanceOf<ConstantExpression>(a.LengthExpression);
            Assert.AreEqual(5, ((ConstantExpression)a.LengthExpression).Value);

            var a1 = (CArrayType)a.ElementType;
            Assert.IsInstanceOf<ConstantExpression>(a1.LengthExpression);
            Assert.AreEqual(12, ((ConstantExpression)a1.LengthExpression).Value);

            Assert.IsInstanceOf<CBasicType>(a1.ElementType);
            Assert.AreEqual("double", ((CBasicType)a1.ElementType).Name);
        }

        [TestMethod]
        public void ArrayOfPointers()
        {
            var tu = Parse("char *mice[10];");
            Assert.IsInstanceOf<CArrayType>(tu.Variables[0].VariableType);
            var a = (CArrayType)tu.Variables[0].VariableType;
            Assert.IsInstanceOf<ConstantExpression>(a.LengthExpression);
            Assert.AreEqual(10, ((ConstantExpression)a.LengthExpression).Value);

            Assert.IsInstanceOf<CPointerType>(a.ElementType);
            var p = (CPointerType)a.ElementType;

            Assert.IsInstanceOf<CBasicType>(p.InnerType);
            Assert.AreEqual("char", ((CBasicType)p.InnerType).Name);
        }

        [TestMethod]
        public void PointerToArray()
        {
            var tu = Parse("double (*elephant)[20];");
            Assert.IsInstanceOf<CPointerType>(tu.Variables[0].VariableType);
            var p = (CPointerType)tu.Variables[0].VariableType;

            Assert.IsInstanceOf<CArrayType>(p.InnerType);
            var a = (CArrayType)p.InnerType;
            Assert.IsInstanceOf<ConstantExpression>(a.LengthExpression);
            Assert.AreEqual(20, ((ConstantExpression)a.LengthExpression).Value);

            Assert.IsInstanceOf<CBasicType>(a.ElementType);
            Assert.AreEqual("double", ((CBasicType)a.ElementType).Name);
        }

        [TestMethod]
        public void ArrayOfPointersToArray()
        {
            var tu = Parse("int (*a[5])[42];");
            Assert.IsInstanceOf<CArrayType>(tu.Variables[0].VariableType);
            var a = (CArrayType)tu.Variables[0].VariableType;
            Assert.IsInstanceOf<ConstantExpression>(a.LengthExpression);
            Assert.AreEqual(5, ((ConstantExpression)a.LengthExpression).Value);

            Assert.IsInstanceOf<CPointerType>(a.ElementType);
            var p = (CPointerType)a.ElementType;

            Assert.IsInstanceOf<CArrayType>(p.InnerType);
        }

        [TestMethod]
        public void PointerToArrayOfPointers()
        {
            var tu = Parse("int *(*crocodile)[15];");
            var v = tu.Variables[0];
            Assert.AreEqual("crocodile", v.Name);
            Assert.IsInstanceOf<CPointerType>(v.VariableType);
            var p = (CPointerType)v.VariableType;

            Assert.IsInstanceOf<CArrayType>(p.InnerType);
            var a = (CArrayType)p.InnerType;
            Assert.IsInstanceOf<ConstantExpression>(a.LengthExpression);
            Assert.AreEqual(15, ((ConstantExpression)a.LengthExpression).Value);

            Assert.IsInstanceOf<CPointerType>(a.ElementType);
            var p2 = (CPointerType)a.ElementType;

            Assert.IsInstanceOf<CBasicType>(p2.InnerType);
            Assert.AreEqual("int", ((CBasicType)p2.InnerType).Name);
        }

        [TestMethod]
        public void FunctionNoArgName()
        {
            var codes = new string[] {
                "long int bat(char);",
                "long int bat(char) {}"
            };
            foreach (var code in codes)
            {
                var tu = Parse(code);
                Assert.AreEqual(1, tu.Functions.Count);
                var f = tu.Functions[0];
                Assert.AreEqual("bat", f.Name);

                Assert.IsInstanceOf<CBasicType>(f.FunctionType.ReturnType);
                Assert.AreEqual("int", ((CBasicType)f.FunctionType.ReturnType).Name);

                Assert.AreEqual(1, f.ParameterInfos.Count);
                Assert.AreEqual("", f.ParameterInfos[0].Name);
                Assert.IsInstanceOf<CBasicType>(f.FunctionType.Parameters[0].ParameterType);
            }
        }

        [TestMethod]
        public void FunctionPointerReturnVoidArg()
        {
            var codes = new string[] {
                "char *wicket(void);",
                "char *wicket(void) {}"
            };
            foreach (var code in codes)
            {
                var tu = Parse(code);
                Assert.AreEqual(1, tu.Functions.Count);
                var f = tu.Functions[0];
                Assert.AreEqual("wicket", f.Name);

                Assert.IsInstanceOf<CPointerType>(f.FunctionType.ReturnType);
                Assert.AreEqual("char", ((CBasicType)((CPointerType)f.FunctionType.ReturnType).InnerType).Name);

                Assert.AreEqual(0, f.ParameterInfos.Count);
            }
        }

        [TestMethod]
        public void FunctionWithFunctionArg()
        {
            var codes = new string[] {
                "int crowd(char p1, int (*p2)(void));",
                "int crowd(char p1, int p2(void));",
                "int crowd(char p1, int (*p2)(void)) {}",
                "int crowd(char p1, int p2(void)) {}"
            };
            foreach (var code in codes)
            {
                var tu = Parse(code);
                Assert.AreEqual(1, tu.Functions.Count);
                var f = tu.Functions[0];
                Assert.AreEqual("crowd", f.Name);

                Assert.IsInstanceOf<CBasicType>(f.FunctionType.ReturnType);
                Assert.AreEqual("int", ((CBasicType)f.FunctionType.ReturnType).Name);

                Assert.AreEqual(2, f.ParameterInfos.Count);
                var p1 = f.FunctionType.Parameters[0];
                var p2 = f.FunctionType.Parameters[1];

                Assert.IsInstanceOf<CBasicType>(p1.ParameterType);
                Assert.AreEqual("char", ((CBasicType)p1.ParameterType).Name);

                Assert.IsInstanceOf<CFunctionType>(p2.ParameterType);
                Assert.AreEqual("int", ((CBasicType)((CFunctionType)p2.ParameterType).ReturnType).Name);
            }
        }

        [TestMethod]
        public void FunctionWithFunctionArgReturningPointer()
        {
            var codes = new string[] {
                "int crowd(char p1, int *(*p2)(void));",
                "int crowd(char p1, int *p2(void));",
                "int crowd(char p1, int *(*p2)(void)) {}",
                "int crowd(char p1, int *p2(void)) {}"
            };
            foreach (var code in codes)
            {
                var tu = Parse(code);
                Assert.AreEqual(1, tu.Functions.Count);
                var f = tu.Functions[0];
                var p1 = f.FunctionType.Parameters[0];
                var p2 = f.FunctionType.Parameters[1];

                Assert.IsInstanceOf<CBasicType>(p1.ParameterType);
                Assert.AreEqual("char", ((CBasicType)p1.ParameterType).Name);

                Assert.IsInstanceOf<CFunctionType>(p2.ParameterType);
                Assert.IsInstanceOf<CPointerType>(((CFunctionType)p2.ParameterType).ReturnType);
            }
        }

        [TestMethod]
        public void PointerToFunction()
        {
            var code = "int (**f)();";

            var tu = Parse(code);
            Assert.AreEqual(1, tu.Variables.Count);
            var fv = tu.Variables[0];
            Assert.AreEqual("f", fv.Name);

            Assert.IsInstanceOf<CPointerType>(fv.VariableType);
            var fp = (CPointerType)fv.VariableType;

            Assert.IsInstanceOf<CFunctionType>(fp.InnerType);
            var f = (CFunctionType)fp.InnerType;

            Assert.AreEqual(0, f.Parameters.Count);
        }

        [TestMethod]
        public void FunctionReturningFunction()
        {
            var code = "long int *(*boundary(double size))(int x, int y);";

            var tu = Parse(code);
            Assert.AreEqual(1, tu.Variables.Count);
            var fv = tu.Variables[0];
            Assert.AreEqual("boundary", fv.Name);

            var f = (CFunctionType)fv.VariableType;

            Assert.AreEqual(1, f.Parameters.Count);
            Assert.AreEqual("size", f.Parameters[0].Name);

            var p1 = f.Parameters[0];

            Assert.IsInstanceOf<CBasicType>(p1.ParameterType);
            Assert.AreEqual("double", ((CBasicType)p1.ParameterType).Name);

            Assert.IsInstanceOf<CFunctionType>(f.ReturnType);
            var r = (CFunctionType)f.ReturnType;

            Assert.AreEqual(2, r.Parameters.Count);

            Assert.IsInstanceOf<CPointerType>(r.ReturnType);
        }
    }
}
