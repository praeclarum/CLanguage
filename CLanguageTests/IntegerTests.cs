using System;
using CLanguage.Interpreter;
using CLanguage.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Globalization;
using CLanguage.Compiler;

namespace CLanguage.Tests
{
	[TestClass]
	public class IntegerTests
	{
		void TestPromote (MachineInfo mi, string type, int resultBytes, Signedness signedness)
		{
			var report = new Report (new TestPrinter ());
			var context = new ExecutableContext (new Executable (mi), report);

			var compiler = new Compiler.CCompiler (mi, report);
			compiler.AddCode ("test.c", type + " v;");
			var exe = compiler.Compile ();

            var ty = exe.Globals.First (x => x.Name == "v").VariableType;
			Assert.IsInstanceOfType (ty, typeof(CBasicType));
			var bty = (CBasicType)ty;
			Assert.IsTrue (bty.IsIntegral);
			var pty = bty.IntegerPromote (context);

			Assert.AreEqual (pty.Signedness, signedness);
			Assert.AreEqual (pty.GetByteSize (context), resultBytes);
		}

		void TestArithmetic (MachineInfo mi, string type1, string type2, CBasicType result)
		{
			var report = new Report (new TestPrinter ());

            var compiler = new Compiler.CCompiler (mi, report);
            compiler.AddCode ("test.c", type1 + " v1; " + type2 + " v2;");
            var exe = compiler.Compile ();
            var context = new ExecutableContext (new Executable (mi), report);

            var ty1 = exe.Globals.First (x => x.Name == "v1").VariableType;
			Assert.IsInstanceOfType (ty1, typeof(CBasicType));
            var ty2 = exe.Globals.First (x => x.Name == "v2").VariableType;
			Assert.IsInstanceOfType (ty2, typeof(CBasicType));

			var bty1 = (CBasicType)ty1;
			var bty2 = (CBasicType)ty2;

			Assert.IsTrue (bty1.IsIntegral);
			Assert.IsTrue (bty2.IsIntegral);

			var aty1 = bty1.ArithmeticConvert (bty2, context);
			var aty2 = bty2.ArithmeticConvert (bty1, context);

			Assert.AreEqual (aty1.Signedness, result.Signedness);
			Assert.AreEqual (aty1.GetByteSize (context), result.GetByteSize (context));
			Assert.AreEqual (aty2.Signedness, result.Signedness);
			Assert.AreEqual (aty2.GetByteSize (context), result.GetByteSize (context));
		}

		[TestMethod]
		public void ArduinoPromote ()
		{
            var mi = new ArduinoTestMachineInfo ();

			TestPromote (mi, "unsigned char", 2, Signedness.Signed);
			TestPromote (mi, "char", 2, Signedness.Signed);
			TestPromote (mi, "short", 2, Signedness.Signed);
			TestPromote (mi, "unsigned short", 2, Signedness.Unsigned);
			TestPromote (mi, "int", 2, Signedness.Signed);
			TestPromote (mi, "unsigned int", 2, Signedness.Unsigned);
			TestPromote (mi, "long", 4, Signedness.Signed);
			TestPromote (mi, "unsigned long", 4, Signedness.Unsigned);
		}

		[TestMethod]
		public void ArduinoArithmatic ()
		{
            var mi = new ArduinoTestMachineInfo ();

			TestArithmetic (mi, "char", "char", CBasicType.SignedInt);
			TestArithmetic (mi, "char", "unsigned char", CBasicType.SignedInt);
			TestArithmetic (mi, "char", "short", CBasicType.SignedInt);
			TestArithmetic (mi, "char", "unsigned short", CBasicType.UnsignedInt);
			TestArithmetic (mi, "char", "int", CBasicType.SignedInt);
			TestArithmetic (mi, "char", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "char", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "char", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "unsigned char", "char", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned char", "unsigned char", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned char", "short", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned char", "unsigned short", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned char", "int", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned char", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned char", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "unsigned char", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "short", "char", CBasicType.SignedInt);
			TestArithmetic (mi, "short", "unsigned char", CBasicType.SignedInt);
			TestArithmetic (mi, "short", "short", CBasicType.SignedInt);
			TestArithmetic (mi, "short", "unsigned short", CBasicType.UnsignedInt);
			TestArithmetic (mi, "short", "int", CBasicType.SignedInt);
			TestArithmetic (mi, "short", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "short", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "short", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "unsigned short", "char", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned short", "unsigned char", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned short", "short", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned short", "unsigned short", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned short", "int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned short", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned short", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "unsigned short", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "int", "char", CBasicType.SignedInt);
			TestArithmetic (mi, "int", "unsigned char", CBasicType.SignedInt);
			TestArithmetic (mi, "int", "short", CBasicType.SignedInt);
			TestArithmetic (mi, "int", "unsigned short", CBasicType.UnsignedInt);
			TestArithmetic (mi, "int", "int", CBasicType.SignedInt);
			TestArithmetic (mi, "int", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "int", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "int", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "unsigned int", "char", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "unsigned char", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "short", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "unsigned short", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "unsigned int", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "long", "char", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "unsigned char", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "short", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "unsigned short", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "int", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "unsigned int", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "unsigned long", "char", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "unsigned char", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "short", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "unsigned short", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "int", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "unsigned int", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "long", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "unsigned long", CBasicType.UnsignedLongInt);
		}

		[TestMethod]
		public void WindowsX86Promote ()
		{
			var mi = MachineInfo.Windows32;

			TestPromote (mi, "unsigned char", 4, Signedness.Signed);
			TestPromote (mi, "char", 4, Signedness.Signed);
			TestPromote (mi, "short", 4, Signedness.Signed);
			TestPromote (mi, "unsigned short", 4, Signedness.Signed);
			TestPromote (mi, "int", 4, Signedness.Signed);
			TestPromote (mi, "unsigned int", 4, Signedness.Unsigned);
			TestPromote (mi, "long", 4, Signedness.Signed);
			TestPromote (mi, "unsigned long", 4, Signedness.Unsigned);
		}

		[TestMethod]
		public void Mac64Arithmatic ()
		{
			var mi = MachineInfo.Mac64;

			TestArithmetic (mi, "char", "char", CBasicType.SignedInt);
			TestArithmetic (mi, "char", "unsigned char", CBasicType.SignedInt);
			TestArithmetic (mi, "char", "short", CBasicType.SignedInt);
			TestArithmetic (mi, "char", "unsigned short", CBasicType.SignedInt);
			TestArithmetic (mi, "char", "int", CBasicType.SignedInt);
			TestArithmetic (mi, "char", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "char", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "char", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "unsigned char", "char", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned char", "unsigned char", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned char", "short", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned char", "unsigned short", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned char", "int", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned char", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned char", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "unsigned char", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "short", "char", CBasicType.SignedInt);
			TestArithmetic (mi, "short", "unsigned char", CBasicType.SignedInt);
			TestArithmetic (mi, "short", "short", CBasicType.SignedInt);
			TestArithmetic (mi, "short", "unsigned short", CBasicType.SignedInt);
			TestArithmetic (mi, "short", "int", CBasicType.SignedInt);
			TestArithmetic (mi, "short", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "short", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "short", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "unsigned short", "char", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned short", "unsigned char", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned short", "short", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned short", "unsigned short", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned short", "int", CBasicType.SignedInt);
			TestArithmetic (mi, "unsigned short", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned short", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "unsigned short", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "int", "char", CBasicType.SignedInt);
			TestArithmetic (mi, "int", "unsigned char", CBasicType.SignedInt);
			TestArithmetic (mi, "int", "short", CBasicType.SignedInt);
			TestArithmetic (mi, "int", "unsigned short", CBasicType.SignedInt);
			TestArithmetic (mi, "int", "int", CBasicType.SignedInt);
			TestArithmetic (mi, "int", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "int", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "int", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "unsigned int", "char", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "unsigned char", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "short", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "unsigned short", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "unsigned int", CBasicType.UnsignedInt);
			TestArithmetic (mi, "unsigned int", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "unsigned int", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "long", "char", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "unsigned char", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "short", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "unsigned short", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "int", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "unsigned int", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "long", CBasicType.SignedLongInt);
			TestArithmetic (mi, "long", "unsigned long", CBasicType.UnsignedLongInt);

			TestArithmetic (mi, "unsigned long", "char", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "unsigned char", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "short", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "unsigned short", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "int", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "unsigned int", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "long", CBasicType.UnsignedLongInt);
			TestArithmetic (mi, "unsigned long", "unsigned long", CBasicType.UnsignedLongInt);
		}

        CInterpreter Run (string code)
        {
            var fullCode = "void start() { __cinit(); main(); } " + code;
            var i = CLanguageService.CreateInterpreter (fullCode, new ArduinoTestMachineInfo (), printer: new TestPrinter ());
            i.Reset ("start");
            i.Run ();
            return i;
        }

        public void AssertEqual (int f, string code)
        {
            var i = Run (@"
void main () {
    assertAreEqual (" + f + @", " + code + @");
}");
        }

        [TestMethod]
        public void BitwiseNot ()
        {
            AssertEqual (~0, "~0");
            AssertEqual (~1, "~1");
            AssertEqual (~2, "~2");
            AssertEqual (~(-0), "~(-0)");
            AssertEqual (~(-1), "~(-1)");
            AssertEqual (~(-2), "~(-2)");
        }

        [TestMethod]
        public void Not ()
        {
            AssertEqual (1, "!0");
            AssertEqual (0, "!1");
            AssertEqual (0, "!2");
            AssertEqual (1, "!(-0)");
            AssertEqual (0, "!(-1)");
            AssertEqual (0, "!(-2)");
            AssertEqual (0, "!true");
            AssertEqual (1, "!false");
            AssertEqual (1, "!!true");
            AssertEqual (0, "!!false");
        }

        [TestMethod]
        public void BitwiseAnd ()
        {
            AssertEqual (0, "0 & 0");
            AssertEqual (0, "0 & 1");
            AssertEqual (0, "1 & 0");
            AssertEqual (1, "1 & 1");
            AssertEqual (3947 & 143, "3947 & 143");
        }

        [TestMethod]
        public void BitwiseOr ()
        {
            AssertEqual (0, "0 | 0");
            AssertEqual (1, "0 | 1");
            AssertEqual (1, "1 | 0");
            AssertEqual (1, "1 | 1");
            AssertEqual (3947 | 143, "3947 | 143");
        }

        [TestMethod]
        public void BitwiseXor ()
        {
            AssertEqual (0, "0 ^ 0");
            AssertEqual (1, "0 ^ 1");
            AssertEqual (1, "1 ^ 0");
            AssertEqual (0, "1 ^ 1");
            AssertEqual (3947 ^ 143, "3947 ^ 143");
        }

        [TestMethod]
        public void ConstantTooBig ()
        {
            AssertEqual (8972313 & 0xFFFF, "8972313");
        }

        [TestMethod]
        public void ShiftLeft ()
        {
            AssertEqual (0 << 0, "0 << 0");
            AssertEqual (0 << 1, "0 << 1");
            AssertEqual (0 << 2, "0 << 2");
            AssertEqual (0 << -1, "0 << -1");
            AssertEqual (1 << 0, "1 << 0");
            AssertEqual (1 << 1, "1 << 1");
            AssertEqual (1 << 2, "1 << 2");
            //AssertEqual (1 << -1, "1 << -1");
            AssertEqual (2 << 0, "2 << 0");
            AssertEqual (2 << 1, "2 << 1");
            AssertEqual (2 << 2, "2 << 2");
            AssertEqual (2 << -1, "2 << -1");
            AssertEqual (-1 << 0, "-1 << 0");
            AssertEqual (-1 << 1, "-1 << 1");
            AssertEqual (-1 << 2, "-1 << 2");
            //AssertEqual (-1 << -1, "-1 << -1");
            AssertEqual (4 << 5, "4 << 5");
        }

        [TestMethod]
        public void ShiftRight ()
        {
            AssertEqual (10 >> 0, "10 >> 0");
            AssertEqual (10 >> 1, "10 >> 1");
            AssertEqual (10 >> 2, "10 >> 2");
            AssertEqual (10 >> -1, "10 >> -1");
            AssertEqual (11 >> 0, "11 >> 0");
            AssertEqual (11 >> 1, "11 >> 1");
            AssertEqual (11 >> 2, "11 >> 2");
            AssertEqual (11 >> -1, "11 >> -1");
            AssertEqual (12 >> 0, "12 >> 0");
            AssertEqual (12 >> 1, "12 >> 1");
            AssertEqual (12 >> 2, "12 >> 2");
            AssertEqual (12 >> -1, "12 >> -1");
            AssertEqual (-11 >> 0, "-11 >> 0");
            AssertEqual (-11 >> 1, "-11 >> 1");
            AssertEqual (-11 >> 2, "-11 >> 2");
            AssertEqual (-11 >> -1, "-11 >> -1");
            AssertEqual (34 >> 5, "34 >> 5");
        }

		[TestMethod]
		public void ShiftLeftIntegerPromotion ()
		{
			// char promotes to int for shift
			Run (@"
void main () {
    char c = 3;
    assertAreEqual (48, c << 4);
}");
			// unsigned char promotes to signed int
			Run (@"
void main () {
    unsigned char uc = 5;
    assertAreEqual (40, uc << 3);
}");
			// int stays int
			Run (@"
void main () {
    int i = 7;
    assertAreEqual (112, i << 4);
}");
			// unsigned int stays unsigned int
			Run (@"
void main () {
    unsigned int ui = 7;
    assertU16AreEqual (112, ui << 4);
}");
			// long stays long for shift (can hold larger values than 16-bit int)
			Run (@"
void main () {
    long l = 3;
    assert32AreEqual (196608L, l << 16);
}");
			// unsigned long stays unsigned long
			Run (@"
void main () {
    unsigned long ul = 5;
    assertU32AreEqual (327680, ul << 16);
}");
		}

		[TestMethod]
		public void ShiftRightIntegerPromotion ()
		{
			// char promotes to int for right shift
			Run (@"
void main () {
    char c = 96;
    assertAreEqual (12, c >> 3);
}");
			// unsigned char promotes to int for right shift
			Run (@"
void main () {
    unsigned char uc = 200;
    assertAreEqual (25, uc >> 3);
}");
			// long stays long for right shift
			Run (@"
void main () {
    long l = 196608L;
    assert32AreEqual (3L, l >> 16);
}");
		}

		[TestMethod]
		public void ShiftResultTypeIsPromotedLeftOperand ()
		{
			// C11 6.5.7: result type is the promoted LEFT operand type,
			// regardless of the right operand type.

			// long left operand with char shift: result is long
			Run (@"
void main () {
    long l = 7;
    char shift = 16;
    assert32AreEqual (458752L, l << shift);
}");
			// unsigned long left operand with char shift: result is unsigned long
			Run (@"
void main () {
    unsigned long ul = 7;
    char shift = 16;
    assertU32AreEqual (458752, ul << shift);
}");
			// int left operand with long shift: result is int (promoted left), not long
			Run (@"
void main () {
    int i = 7;
    long shift = 4;
    assertAreEqual (112, i << shift);
}");
		}

		[TestMethod]
		public void ShiftLeftByteToLongCast ()
		{
			// Original issue scenario: to get large shifts on small types,
			// cast the left operand to long explicitly
			Run (@"
void main () {
    byte b = 7;
    assert32AreEqual (458752L, (long)b << 16);
}");
		}

		[TestMethod]
		public void StdInts ()
		{
			Run (@"
#include <stdint.h>
int8_t byteValue = 42;
void main() {
    assertAreEqual(42, byteValue);
}
");
		}
	}
}

