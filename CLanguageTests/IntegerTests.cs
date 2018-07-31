using System;
using CLanguage.Interpreter;
using CLanguage.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace CLanguage.Tests
{
	[TestClass]
	public class IntegerTests
	{
		void TestPromote (MachineInfo mi, string type, int resultBytes, Signedness signedness)
		{
			var report = new Report (new TestPrinter ());
			var context = new EmitContext (mi, report);

			var compiler = new Compiler (mi, report);
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
			var context = new EmitContext (mi, report);

			var compiler = new Compiler (mi, report);
            compiler.AddCode ("test.c", type1 + " v1; " + type2 + " v2;");
			var exe = compiler.Compile ();

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
	}
}

