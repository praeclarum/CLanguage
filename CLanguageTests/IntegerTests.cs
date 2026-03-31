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
		public void ShiftLeftIssue41PressureSensor ()
		{
			// Exact reproduction of issue #41:
			// On Arduino (16-bit int), combining byte and unsigned int
			// via shifts to build a 19-bit pressure value.
			// The left operand must be cast to long to avoid overflow.
			Run (@"
void main () {
    byte pressure_data_high = 5;
    pressure_data_high &= 0x07;
    unsigned int pressure_data_low = 0x1234;

    // With long cast on left operand, shift happens at 32-bit precision
    long pressure = (((long)pressure_data_high << 16) | pressure_data_low) / 4;
    assert32AreEqual (83085L, pressure);
}");
			// Verify the full range: high=7 (max 3 bits), low=0xFFFF
			Run (@"
void main () {
    byte pressure_data_high = 7;
    unsigned int pressure_data_low = 0xFFFF;
    long pressure = (((long)pressure_data_high << 16) | pressure_data_low) / 4;
    assert32AreEqual (131071L, pressure);
}");
		}

		[TestMethod]
		public void ShiftLeftByteOverflowsOn16BitInt ()
		{
			// On Arduino (16-bit int), byte promotes to int (16-bit).
			// Shifting a 16-bit value left by >= 16 overflows.
			// Casting to long first preserves the value.
			Run (@"
void main () {
    byte b = 7;
    // Without cast: byte promotes to 16-bit int, shift by 16 overflows
    long result_no_cast = b << 16;
    assert32AreEqual (0L, result_no_cast);

    // With cast to long: shift happens at 32-bit, no overflow
    long result_cast = (long)b << 16;
    assert32AreEqual (458752L, result_cast);
}");
		}

		[TestMethod]
		public void ShiftLeftBitBoundary16BitInt ()
		{
			// Edge cases at the 16-bit int boundary on Arduino
			Run (@"
void main () {
    byte b = 1;

    // Shift by 15: sets the sign bit of a 16-bit signed int
    assertAreEqual (-32768, b << 15);

    // Shift by 14: highest positive power of 2 in signed 16-bit
    assertAreEqual (16384, b << 14);

    // Unsigned int shift by 15: sets MSB, stays positive
    unsigned int u = 1;
    assertU16AreEqual (32768, u << 15);

    // Building a 16-bit value from two bytes via shift
    byte high = 0xAB;
    byte low = 0xCD;
    assertU16AreEqual (0xABCD, ((unsigned int)high << 8) | low);
}");
		}

		[TestMethod]
		public void ShiftResultTypeDependsOnlyOnLeftOperand ()
		{
			// C11 §6.5.7: The type of the result is the promoted left operand.
			// The right operand's type must NOT widen the result.
			Run (@"
void main () {
    // Left=byte, right=long: result is int (promoted byte), NOT long.
    // The long right operand must not pull the shift type up.
    byte b = 1;
    long shift = 8;
    assertAreEqual (256, b << shift);

    // Left=long, right=byte: result is long (promoted long).
    long l = 1;
    byte s = 20;
    assert32AreEqual (1048576L, l << s);

    // Left=unsigned long, right=char: result is unsigned long.
    unsigned long ul = 0xFF;
    char sc = 16;
    assertU32AreEqual (16711680, ul << sc);

    // Left=int, right=long: result is int, not long.
    // Shift by 12 fits in 16-bit int.
    int i = 1;
    long sl = 12;
    assertAreEqual (4096, i << sl);
}");
		}

		[TestMethod]
		public void ShiftRightSignedUnsignedBehavior ()
		{
			// Signed right shift is arithmetic (preserves sign bit),
			// unsigned right shift is logical (fills with zeros).
			Run (@"
void main () {
    // Signed: right shift preserves sign
    int neg = -1024;
    assertAreEqual (-128, neg >> 3);

    // Unsigned: right shift fills with zeros
    unsigned int uneg = 0xFC00;
    assertU16AreEqual (0x1F80, uneg >> 3);

    // Long signed right shift
    long lneg = -262144L;
    assert32AreEqual (-32768L, lneg >> 3);

    // Decomposing a 32-bit long back into bytes via right shift
    unsigned long val = 0x00051234;
    byte high = (byte)(val >> 16);
    unsigned int low = (unsigned int)(val & 0xFFFF);
    assertAreEqual (5, high);
    assertU16AreEqual (0x1234, low);
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

