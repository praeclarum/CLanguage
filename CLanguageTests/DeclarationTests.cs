using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            Assert.IsInstanceOfType(v.VariableType, typeof(CBasicType));
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
            Assert.IsInstanceOfType(v.VariableType, typeof(CBasicType));
            Assert.AreEqual(TypeQualifiers.Const, v.VariableType.TypeQualifiers);
        }

		[TestMethod]
        public void Pointer()
        {
            var tu = Parse("char *square;");
            var v = tu.Variables[0];
            Assert.AreEqual("square", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            Assert.IsInstanceOfType(((CPointerType)v.VariableType).InnerType, typeof(CBasicType));
            Assert.AreEqual("char", ((CBasicType)((CPointerType)v.VariableType).InnerType).Name);
        }

		[TestMethod]
        public void VoidPointer()
        {
            var tu = Parse("void *triangle;");
            var v = tu.Variables[0];
            Assert.AreEqual("triangle", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            Assert.IsInstanceOfType(((CPointerType)v.VariableType).InnerType, typeof(CVoidType));
            Assert.IsTrue(((CPointerType)v.VariableType).InnerType.IsVoid);
        }

		[TestMethod]
        public void PointerSeparation()
        {
            var tu = Parse("long* first, second;");
            var v = tu.Variables[0];
            var v1 = tu.Variables[1];
            Assert.AreEqual("first", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            Assert.IsInstanceOfType(((CPointerType)v.VariableType).InnerType, typeof(CBasicType));
            Assert.AreEqual("int", ((CBasicType)((CPointerType)v.VariableType).InnerType).Name);
            Assert.AreEqual("second", v1.Name);
            Assert.IsInstanceOfType(v1.VariableType, typeof(CBasicType));
            Assert.AreEqual("int", ((CBasicType)v1.VariableType).Name);
        }

		[TestMethod]
        public void NonConstPointerToConstChar()
        {
            var tu = Parse("const char *kite;");
            var v = tu.Variables[0];
            Assert.AreEqual("kite", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            var pt = (CPointerType)v.VariableType;
            Assert.AreEqual(TypeQualifiers.None, pt.TypeQualifiers);
            Assert.IsInstanceOfType(pt.InnerType, typeof(CBasicType));
            Assert.AreEqual("char", ((CBasicType)pt.InnerType).Name);
            Assert.AreEqual(TypeQualifiers.Const, ((CBasicType)pt.InnerType).TypeQualifiers);
        }

		[TestMethod]
        public void ConstPointerToChar()
        {
            var tu = Parse("char * const pentagon;");
            var v = tu.Variables[0];
            Assert.AreEqual("pentagon", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            var pt = (CPointerType)v.VariableType;
            Assert.AreEqual(TypeQualifiers.Const, pt.TypeQualifiers);
            Assert.IsInstanceOfType(pt.InnerType, typeof(CBasicType));
            Assert.AreEqual("char", ((CBasicType)pt.InnerType).Name);
            Assert.AreEqual(TypeQualifiers.None, ((CBasicType)pt.InnerType).TypeQualifiers);
        }

		[TestMethod]
        public void ConstPointerToConstChar1()
        {
            var tu = Parse("char const * const hexagon;");
            var v = tu.Variables[0];
            Assert.AreEqual("hexagon", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            var pt = (CPointerType)v.VariableType;
            Assert.AreEqual(TypeQualifiers.Const, pt.TypeQualifiers);
            Assert.IsInstanceOfType(pt.InnerType, typeof(CBasicType));
            Assert.AreEqual("char", ((CBasicType)pt.InnerType).Name);
            Assert.AreEqual(TypeQualifiers.Const, ((CBasicType)pt.InnerType).TypeQualifiers);
        }

		[TestMethod]
        public void ConstPointerToConstChar2()
        {
            var tu = Parse("const char * const hexagon;");
            var v = tu.Variables[0];
            Assert.AreEqual("hexagon", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            var pt = (CPointerType)v.VariableType;
            Assert.AreEqual(TypeQualifiers.Const, pt.TypeQualifiers);
            Assert.IsInstanceOfType(pt.InnerType, typeof(CBasicType));
            Assert.AreEqual("char", ((CBasicType)pt.InnerType).Name);
            Assert.AreEqual(TypeQualifiers.Const, ((CBasicType)pt.InnerType).TypeQualifiers);
        }

		[TestMethod]
        public void PointerToPointer()
        {
            var tu = Parse("char **septagon;");
            var v = tu.Variables[0];
            Assert.AreEqual("septagon", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            var pt = (CPointerType)v.VariableType;
            Assert.IsInstanceOfType(pt.InnerType, typeof(CPointerType));
            var pt1 = (CPointerType)pt.InnerType;
            Assert.IsInstanceOfType(pt1.InnerType, typeof(CBasicType));
            Assert.AreEqual("char", ((CBasicType)pt1.InnerType).Name);
        }

		[TestMethod]
        public void PointerToConstPointerToConstBasic()
        {
            var tu = Parse("unsigned long const int * const *octagon;");
            var v = tu.Variables[0];
            Assert.AreEqual("octagon", v.Name);

            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            var pt = (CPointerType)v.VariableType;
            Assert.AreEqual(TypeQualifiers.None, pt.TypeQualifiers);

            Assert.IsInstanceOfType(pt.InnerType, typeof(CPointerType));
            var pt1 = (CPointerType)pt.InnerType;
            Assert.AreEqual(TypeQualifiers.Const, pt1.TypeQualifiers);

            Assert.IsInstanceOfType(pt1.InnerType, typeof(CBasicType));
            var b = (CBasicType)pt1.InnerType;
            Assert.AreEqual(TypeQualifiers.Const, b.TypeQualifiers);
            Assert.AreEqual("int", b.Name);
        }

		[TestMethod]
        public void Array()
        {
            var tu = Parse("int cat[10];");
            var t = (CArrayType)tu.Variables[0].VariableType;
            Assert.IsInstanceOfType(t.LengthExpression, typeof(ConstantExpression));
            Assert.AreEqual(10, ((ConstantExpression)t.LengthExpression).Value);
            Assert.IsInstanceOfType(t.ElementType, typeof(CBasicType));
        }

		[TestMethod]
        public void ArrayOfArrays()
        {
            var tu = Parse("double dog[5][12];");
            var a = (CArrayType)tu.Variables[0].VariableType;
            Assert.IsInstanceOfType(a.LengthExpression, typeof(ConstantExpression));
            Assert.AreEqual(5, ((ConstantExpression)a.LengthExpression).Value);

            var a1 = (CArrayType)a.ElementType;
            Assert.IsInstanceOfType(a1.LengthExpression, typeof(ConstantExpression));
            Assert.AreEqual(12, ((ConstantExpression)a1.LengthExpression).Value);

            Assert.IsInstanceOfType(a1.ElementType, typeof(CBasicType));
            Assert.AreEqual("double", ((CBasicType)a1.ElementType).Name);
        }

		[TestMethod]
        public void ArrayOfPointers()
        {
            var tu = Parse("char *mice[10];");
            Assert.IsInstanceOfType(tu.Variables[0].VariableType, typeof(CArrayType));
            var a = (CArrayType)tu.Variables[0].VariableType;
            Assert.IsInstanceOfType(a.LengthExpression, typeof(ConstantExpression));
            Assert.AreEqual(10, ((ConstantExpression)a.LengthExpression).Value);

            Assert.IsInstanceOfType(a.ElementType, typeof(CPointerType));
            var p = (CPointerType)a.ElementType;

            Assert.IsInstanceOfType(p.InnerType, typeof(CBasicType));
            Assert.AreEqual("char", ((CBasicType)p.InnerType).Name);
        }

		[TestMethod]
        public void PointerToArray()
        {
            var tu = Parse("double (*elephant)[20];");
            Assert.IsInstanceOfType(tu.Variables[0].VariableType, typeof(CPointerType));
            var p = (CPointerType)tu.Variables[0].VariableType;

            Assert.IsInstanceOfType(p.InnerType, typeof(CArrayType));
            var a = (CArrayType)p.InnerType;
            Assert.IsInstanceOfType(a.LengthExpression, typeof(ConstantExpression));
            Assert.AreEqual(20, ((ConstantExpression)a.LengthExpression).Value);

            Assert.IsInstanceOfType(a.ElementType, typeof(CBasicType));
            Assert.AreEqual("double", ((CBasicType)a.ElementType).Name);
        }

		[TestMethod]
        public void ArrayOfPointersToArray()
        {
            var tu = Parse("int (*a[5])[42];");
            Assert.IsInstanceOfType(tu.Variables[0].VariableType, typeof(CArrayType));
            var a = (CArrayType)tu.Variables[0].VariableType;
            Assert.IsInstanceOfType(a.LengthExpression, typeof(ConstantExpression));
            Assert.AreEqual(5, ((ConstantExpression)a.LengthExpression).Value);

            Assert.IsInstanceOfType(a.ElementType, typeof(CPointerType));
            var p = (CPointerType)a.ElementType;

            Assert.IsInstanceOfType(p.InnerType, typeof(CArrayType));
        }

		[TestMethod]
        public void PointerToArrayOfPointers()
        {
            var tu = Parse("int *(*crocodile)[15];");
            var v = tu.Variables[0];
            Assert.AreEqual("crocodile", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            var p = (CPointerType)v.VariableType;

            Assert.IsInstanceOfType(p.InnerType, typeof(CArrayType));
            var a = (CArrayType)p.InnerType;
            Assert.IsInstanceOfType(a.LengthExpression, typeof(ConstantExpression));
            Assert.AreEqual(15, ((ConstantExpression)a.LengthExpression).Value);

            Assert.IsInstanceOfType(a.ElementType, typeof(CPointerType));
            var p2 = (CPointerType)a.ElementType;

            Assert.IsInstanceOfType(p2.InnerType, typeof(CBasicType));
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

                Assert.IsInstanceOfType(f.FunctionType.ReturnType, typeof(CBasicType));
                Assert.AreEqual("int", ((CBasicType)f.FunctionType.ReturnType).Name);

                Assert.AreEqual(1, f.ParameterInfos.Count);
                Assert.AreEqual("", f.ParameterInfos[0].Name);
                Assert.IsInstanceOfType(f.FunctionType.Parameters[0].ParameterType, typeof(CBasicType));
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

                Assert.IsInstanceOfType(f.FunctionType.ReturnType, typeof(CPointerType));
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

                Assert.IsInstanceOfType(f.FunctionType.ReturnType, typeof(CBasicType));
                Assert.AreEqual("int", ((CBasicType)f.FunctionType.ReturnType).Name);

                Assert.AreEqual(2, f.ParameterInfos.Count);
                var p1 = f.FunctionType.Parameters[0];
                var p2 = f.FunctionType.Parameters[1];

                Assert.IsInstanceOfType(p1.ParameterType, typeof(CBasicType));
                Assert.AreEqual("char", ((CBasicType)p1.ParameterType).Name);

                Assert.IsInstanceOfType(p2.ParameterType, typeof(CFunctionType));
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

                Assert.IsInstanceOfType(p1.ParameterType, typeof(CBasicType));
                Assert.AreEqual("char", ((CBasicType)p1.ParameterType).Name);

                Assert.IsInstanceOfType(p2.ParameterType, typeof(CFunctionType));
                Assert.IsInstanceOfType(((CFunctionType)p2.ParameterType).ReturnType, typeof(CPointerType));
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

            Assert.IsInstanceOfType(fv.VariableType, typeof(CPointerType));
            var fp = (CPointerType)fv.VariableType;

            Assert.IsInstanceOfType(fp.InnerType, typeof(CFunctionType));
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

            Assert.IsInstanceOfType(p1.ParameterType, typeof(CBasicType));
            Assert.AreEqual("double", ((CBasicType)p1.ParameterType).Name);

            Assert.IsInstanceOfType(f.ReturnType, typeof(CFunctionType));
            var r = (CFunctionType)f.ReturnType;

            Assert.AreEqual(2, r.Parameters.Count);

            Assert.IsInstanceOfType(r.ReturnType, typeof(CPointerType));
        }
    }
}
