using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if VS_UNIT_TESTING
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#endif

namespace CLanguage.Tests
{
    [TestClass]
    public class SizeTests
    {
        CType ParseType(string code)
        {
			var report = new Report (new TestPrinter ());
            var pp = new Preprocessor(report);
            pp.AddCode("stdin", code);
            var lexer = new Lexer(pp);
            var parser = new CParser();
            var tu = parser.ParseTranslationUnit(lexer, report);
            return tu.Variables[0].VariableType;
        }

        EmitContext _c = new EmitContext (MachineInfo.WindowsX86, new Report (new TextWriterReportPrinter (Console.Out)));

        [TestMethod]
        public void BasicSizes()
        {
            var tests = new Dictionary<string, int> {
{"char a;", 1},
{"signed char a;", 1},
{"unsigned char a;", 1},
{"short a;", 2},
{"signed short a;", 2},
{"unsigned short a;", 2},
{"int a;", 4},
{"signed int a;", 4},
{"unsigned int a;", 4},
{"long a;", 4},
{"long long a;", 8},
{"long long int a;", 8},
{"unsigned long a;", 4},
{"unsigned long long a;", 8},
{"double a;", 8},
{"float a;", 4},
{"long double a;", 8},
            };
            foreach (var t in tests)
            {
                var code = t.Key;
                var size = t.Value;

                var type = ParseType(code);

                Assert.AreEqual(size, type.GetSize(_c));
            }
        }

        [TestMethod]
        public void PointerSizes()
        {
            var tests = new Dictionary<string, int> {
{"char* a;", 4},
{"int* a;", 4},
{"double* a;", 4},
{"char** a;", 4},
{"const char* a;", 4},
{"const char** a;", 4},
{"char *const a = 0;", 4},
{"const char *const a = 0;", 4},
{"char *const *a;", 4},
{"char *const *const a = 0;", 4},
{"char *const **a;", 4},
{"char *const ****a;", 4},
{"int (*a)(int);", 4},
{"int *(*a)(int);", 4},
{"int *(**a)(int);", 4},
            };
            foreach (var t in tests)
            {
                var code = t.Key;
                var size = t.Value;

                var type = ParseType(code);

                Assert.AreEqual(size, type.GetSize(_c));
            }
        }

        [TestMethod]
        public void ArraySizes()
        {
            var tests = new Dictionary<string, int> {
{"char a[42];", 42},
{"char a[42][12];", 504},
{"char *a[42];", 168},
{"char *a[42][12];", 2016},
{"int a[42];", 168},
{"int a[42][12];", 2016},
{"int *a[42];", 168},
{"int *a[42][12];", 2016},
{"int (*a)[42];", 4},
{"int (*a)[42][12];", 4},
{"int (*a[5])[42];", 20},
{"int (*a[5])[42][12];", 20},
{"int (*a[5])[2][3][5][7][11];", 20},
{"int (*a)[2][3][5][7][11];", 4},
{"int *a[2][3][5][7][11];", 9240},
{"short *a[2][3][5][7][11];", 9240},
{"short a[2][3][5][7][11];", 4620},
{"short (a[2][3])[5][7][11];", 4620},
            };
            foreach (var t in tests)
            {
                var code = t.Key;
                var size = t.Value;

                var type = ParseType(code);

                Assert.AreEqual(size, type.GetSize(_c));
            }
        }


    }
}
