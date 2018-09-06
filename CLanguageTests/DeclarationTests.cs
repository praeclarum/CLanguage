using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Syntax;
using CLanguage.Types;
using static CLanguage.CLanguageService;
using CLanguage.Interpreter;

namespace CLanguage.Tests
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/C_variable_types_and_declarations
    /// </summary>
	[TestClass]
    public class DeclarationTests
    {
        List<CompiledVariable> ParseVariables (string code)
        {
            var exe = CLanguageService.Compile (code, MachineInfo.Windows32, new TestPrinter ());
            return exe.Globals.Skip (1).ToList (); // Skip __zero__
        }

        List<BaseFunction> ParseFunctions (string code)
        {
            var exe = CLanguageService.Compile (code, MachineInfo.Windows32, new TestPrinter ());
            return exe.Functions.Where (x => x.Name != "__cinit").ToList ();
        }

		[TestMethod]
        public void Basic()
        {
            var vs = ParseVariables("int cat;");
            Assert.AreEqual(1, vs.Count);
            var v = vs[0];
            Assert.AreEqual("cat", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CBasicType));
        }

		[TestMethod]
        public void SignednessBasic()
        {
            var vs = ParseVariables ("unsigned int x; signed int y; int z; unsigned char grey;signed char white;");
            Assert.AreEqual(5, vs.Count);
            var x = vs[0];
            var y = vs[1];
            var z = vs[2];
            var grey = vs[3];
            var white = vs[4];
            Assert.AreEqual(Signedness.Unsigned, ((CBasicType)x.VariableType).Signedness);
            Assert.AreEqual(Signedness.Signed, ((CBasicType)y.VariableType).Signedness);
            Assert.AreEqual(Signedness.Signed, ((CBasicType)z.VariableType).Signedness);
            Assert.AreEqual(Signedness.Unsigned, ((CBasicType)grey.VariableType).Signedness);
            Assert.AreEqual(Signedness.Signed, ((CBasicType)white.VariableType).Signedness);
        }

		[TestMethod]
        public void SignednessNoBasic()
        {
            var vs = ParseVariables ("unsigned x; signed y;");
            var x = (CBasicType)vs[0].VariableType;
            var y = (CBasicType)vs[1].VariableType;
            Assert.AreEqual(Signedness.Unsigned, x.Signedness);
            Assert.AreEqual("int", x.Name);
            Assert.AreEqual(Signedness.Signed, y.Signedness);
            Assert.AreEqual("int", y.Name);
        }

		[TestMethod]
        public void SizeBasic()
        {
            var vs = ParseVariables ("short int yellow;long int orange;long long int red;long brown;long double black;");
            var yellow = (CBasicType)vs[0].VariableType;
            var orange = (CBasicType)vs[1].VariableType;
            var red = (CBasicType)vs[2].VariableType;
            var brown = (CBasicType)vs[3].VariableType;
            var black = (CBasicType)vs[4].VariableType;
            Assert.AreEqual("short", yellow.Size);
            Assert.AreEqual("int", yellow.Name);
            Assert.AreEqual("long", orange.Size);
            Assert.AreEqual("int", orange.Name);
            Assert.AreEqual("long long", red.Size);
            Assert.AreEqual("int", red.Name);
            Assert.AreEqual("long", brown.Size);
            Assert.AreEqual("int", brown.Name);
            Assert.AreEqual("double", black.Name);
        }

		[TestMethod]
        public void QualifiedBasic()
        {
            var vs = ParseVariables ("const int cat;");
            var v = vs[0];
            Assert.IsInstanceOfType(v.VariableType, typeof(CBasicType));
            Assert.AreEqual(TypeQualifiers.Const, v.VariableType.TypeQualifiers);
        }

		[TestMethod]
        public void Pointer()
        {
            var vs = ParseVariables ("char *square;");
            var v = vs[0];
            Assert.AreEqual("square", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            Assert.IsInstanceOfType(((CPointerType)v.VariableType).InnerType, typeof(CBasicType));
            Assert.AreEqual("char", ((CBasicType)((CPointerType)v.VariableType).InnerType).Name);
        }

		[TestMethod]
        public void VoidPointer()
        {
            var vs = ParseVariables ("void *triangle;");
            var v = vs[0];
            Assert.AreEqual("triangle", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            Assert.IsInstanceOfType(((CPointerType)v.VariableType).InnerType, typeof(CVoidType));
            Assert.IsTrue(((CPointerType)v.VariableType).InnerType.IsVoid);
        }

		[TestMethod]
        public void PointerSeparation()
        {
            var vs = ParseVariables ("long* first, second;");
            var v = vs[0];
            var v1 = vs[1];
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
            var vs = ParseVariables ("const char *kite;");
            var v = vs[0];
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
            var vs = ParseVariables ("char * const pentagon;");
            var v = vs[0];
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
            var vs = ParseVariables ("char const * const hexagon;");
            var v = vs[0];
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
            var vs = ParseVariables ("const char * const hexagon;");
            var v = vs[0];
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
            var vs = ParseVariables ("char **septagon;");
            var v = vs[0];
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
            var vs = ParseVariables ("unsigned long const int * const *octagon;");
            var v = vs[0];
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
            var vs = ParseVariables ("int cat[10];");
            var t = (CArrayType)vs[0].VariableType;
            Assert.AreEqual(10, t.Length.Value);
            Assert.IsInstanceOfType(t.ElementType, typeof(CBasicType));
        }

		[TestMethod]
        public void ArrayOfArrays()
        {
            var vs = ParseVariables ("double dog[5][12];");
            var a = (CArrayType)vs[0].VariableType;
            Assert.AreEqual(5, a.Length.Value);

            var a1 = (CArrayType)a.ElementType;
            Assert.AreEqual(12, a1.Length.Value);

            Assert.IsInstanceOfType(a1.ElementType, typeof(CBasicType));
            Assert.AreEqual("double", ((CBasicType)a1.ElementType).Name);
        }

		[TestMethod]
        public void ArrayOfPointers()
        {
            var vs = ParseVariables ("char *mice[10];");
            Assert.IsInstanceOfType(vs[0].VariableType, typeof(CArrayType));
            var a = (CArrayType)vs[0].VariableType;
            Assert.AreEqual(10, a.Length.Value);

            Assert.IsInstanceOfType(a.ElementType, typeof(CPointerType));
            var p = (CPointerType)a.ElementType;

            Assert.IsInstanceOfType(p.InnerType, typeof(CBasicType));
            Assert.AreEqual("char", ((CBasicType)p.InnerType).Name);
        }

		[TestMethod]
        public void PointerToArray()
        {
            var vs = ParseVariables ("double (*elephant)[20];");
            Assert.IsInstanceOfType(vs[0].VariableType, typeof(CPointerType));
            var p = (CPointerType)vs[0].VariableType;

            Assert.IsInstanceOfType(p.InnerType, typeof(CArrayType));
            var a = (CArrayType)p.InnerType;
            Assert.AreEqual(20, a.Length.Value);

            Assert.IsInstanceOfType(a.ElementType, typeof(CBasicType));
            Assert.AreEqual("double", ((CBasicType)a.ElementType).Name);
        }

		[TestMethod]
        public void ArrayOfPointersToArray()
        {
            var vs = ParseVariables ("int (*a[5])[42];");
            Assert.IsInstanceOfType(vs[0].VariableType, typeof(CArrayType));
            var a = (CArrayType)vs[0].VariableType;
            Assert.AreEqual(5, a.Length.Value);

            Assert.IsInstanceOfType(a.ElementType, typeof(CPointerType));
            var p = (CPointerType)a.ElementType;

            Assert.IsInstanceOfType(p.InnerType, typeof(CArrayType));
        }

		[TestMethod]
        public void PointerToArrayOfPointers()
        {
            var vs = ParseVariables ("int *(*crocodile)[15];");
            var v = vs[0];
            Assert.AreEqual("crocodile", v.Name);
            Assert.IsInstanceOfType(v.VariableType, typeof(CPointerType));
            var p = (CPointerType)v.VariableType;

            Assert.IsInstanceOfType(p.InnerType, typeof(CArrayType));
            var a = (CArrayType)p.InnerType;
            Assert.AreEqual(15, a.Length.Value);

            Assert.IsInstanceOfType(a.ElementType, typeof(CPointerType));
            var p2 = (CPointerType)a.ElementType;

            Assert.IsInstanceOfType(p2.InnerType, typeof(CBasicType));
            Assert.AreEqual("int", ((CBasicType)p2.InnerType).Name);
        }

		[TestMethod]
        public void FunctionNoArgName()
        {
            var codes = new string[] {
                "long int bat(int) { return 0; }",
                "long int bat(char) { return 0; }"
            };
            foreach (var code in codes)
            {
                var fs = ParseFunctions (code);
                Assert.AreEqual(1, fs.Count);
                var f = fs[0];
                Assert.AreEqual("bat", f.Name);

                Assert.IsInstanceOfType(f.FunctionType.ReturnType, typeof(CBasicType));
                Assert.AreEqual("int", ((CBasicType)f.FunctionType.ReturnType).Name);

                Assert.AreEqual(1, f.FunctionType.Parameters.Count);
                Assert.AreEqual("", f.FunctionType.Parameters[0].Name);
                Assert.IsInstanceOfType(f.FunctionType.Parameters[0].ParameterType, typeof(CBasicType));
            }
        }

		[TestMethod]
        public void FunctionPointerReturnVoidArg()
        {
            var codes = new string[] {
                "char *wicket(void) {return 0;}",
            };
            foreach (var code in codes)
            {
                var fs = ParseFunctions (code);
                Assert.AreEqual(1, fs.Count);
                var f = fs[0];
                Assert.AreEqual("wicket", f.Name);

                Assert.IsInstanceOfType(f.FunctionType.ReturnType, typeof(CPointerType));
                Assert.AreEqual("char", ((CBasicType)((CPointerType)f.FunctionType.ReturnType).InnerType).Name);

                Assert.AreEqual(0, f.FunctionType.Parameters.Count);
            }
        }

		[TestMethod]
        public void FunctionWithFunctionArg()
        {
            var codes = new string[] {
                "int crowd(char p1, int (*p2)(void)) {return 0;}",
                "int crowd(char p1, int p2(void)) {return 0;}"
            };
            foreach (var code in codes)
            {
                var fs = ParseFunctions (code);
                Assert.AreEqual(1, fs.Count);
                var f = fs[0];
                Assert.AreEqual("crowd", f.Name);

                Assert.IsInstanceOfType(f.FunctionType.ReturnType, typeof(CBasicType));
                Assert.AreEqual("int", ((CBasicType)f.FunctionType.ReturnType).Name);

                Assert.AreEqual(2, f.FunctionType.Parameters.Count);
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
                "int crowd(char p1, int *(*p2)(void)) {return 0;}",
                "int crowd(char p1, int *p2(void)) {return 0;}"
            };
            foreach (var code in codes)
            {
                var fs = ParseFunctions (code);
                Assert.AreEqual(1, fs.Count);
                var f = fs[0];
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

            var vs = ParseVariables (code);
            Assert.AreEqual(1, vs.Count);
            var fv = vs[0];
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

            var vs = ParseVariables (code);
            Assert.AreEqual(1, vs.Count);
            var fv = vs[0];
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

        [TestMethod]
        public void AutoConstants ()
        {
            AssertAutoType ("1", CBasicType.SignedInt);
            AssertAutoType ("1l", CBasicType.SignedLongInt);
            AssertAutoType ("true", CBasicType.Bool);
            AssertAutoType ("false", CBasicType.Bool);
            AssertAutoType ("1 + 2", CBasicType.SignedInt);
            AssertAutoType ("1 + 2.0", CBasicType.Double);
        }

        void AssertAutoType (string code, CType expectedType)
        {
            var fullCode = "auto x = " + code + ";";
            var exe = CLanguageService.Compile (fullCode, machineInfo: new ArduinoTestMachineInfo ());
            var g = exe.Globals.FirstOrDefault (x => x.Name == "x");
            Assert.AreEqual (expectedType, g.VariableType);
        }
    }
}
