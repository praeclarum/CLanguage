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
	public class CompilerTests
	{
		Compiler CreateCompiler ()
		{
			return new Compiler (MachineInfo.Arduino, new Report (new TestPrinter ()));
		}

		Executable Compile (string code)
		{
			var c = CreateCompiler ();
			c.AddCode (code);
			return c.Compile ();
		}

		[TestMethod]
		public void ReturnConstant ()
		{
			var exe = Compile (@"int f () { return 42; }");
			Assert.That (exe.Functions.Count, Is.EqualTo (1));
			var f = exe.Functions.First (x => x.Name == "f");
			Assert.That (f.Instructions.Count, Is.EqualTo (2));

			Assert.That (f.Instructions[0].Op, Is.EqualTo (OpCode.LoadValue));
			Assert.That (f.Instructions[1].Op, Is.EqualTo (OpCode.Return));
		}

		[TestMethod]
		public void ReturnParamExpr ()
		{
			var exe = Compile (@"int f (int i) { return i + 42; }");
			Assert.That (exe.Functions.Count, Is.EqualTo (1));
			var f = exe.Functions.First (x => x.Name == "f");
			Assert.That (f.Instructions.Count, Is.EqualTo (4));

			Assert.That (f.Instructions[0].Op, Is.EqualTo (OpCode.LoadArg));
			Assert.That (f.Instructions[1].Op, Is.EqualTo (OpCode.LoadValue));
			Assert.That (f.Instructions[2].Op, Is.EqualTo (OpCode.AddInt16));
			Assert.That (f.Instructions[3].Op, Is.EqualTo (OpCode.Return));
		}

		[TestMethod]
		public void ConditionalReturn ()
		{
			var exe = Compile (@"int f (int i) { if (i) return 0; else return 42; }");
			Assert.That (exe.Functions.Count, Is.EqualTo (1));
			var f = exe.Functions.First (x => x.Name == "f");
			Assert.That (f.Instructions.Count, Is.EqualTo (7));
			Assert.That (f.Instructions[0].Op, Is.EqualTo (OpCode.LoadArg));
			Assert.That (f.Instructions[1].Op, Is.EqualTo (OpCode.BranchIfFalse));
			Assert.That (f.Instructions[2].Op, Is.EqualTo (OpCode.LoadValue));
			Assert.That (f.Instructions[3].Op, Is.EqualTo (OpCode.Return));
			Assert.That (f.Instructions[4].Op, Is.EqualTo (OpCode.Jump));
			Assert.That (f.Instructions[5].Op, Is.EqualTo (OpCode.LoadValue));
			Assert.That (f.Instructions[6].Op, Is.EqualTo (OpCode.Return));
		}

		[Test]
		public void LocalVariables ()
		{
			var exe = Compile (@"
void f () {
	int a = 4;
	int b = 8;
	int c = a + b;
}");
			var f = exe.Functions.First (x => x.Name == "f");
			Assert.That (f.LocalVariables.Count, Is.EqualTo (3));
		}

		[TestMethod]
		public void ArduinoBlink ()
		{
			var exe = Compile (ArduinoTests.BlinkCode);
			Assert.That (exe.Functions.Count, Is.EqualTo (2));
		}

		[TestMethod]
		public void ArduinoFade ()
		{
			var exe = Compile (ArduinoTests.FadeCode);
			Assert.That (exe.Functions.Count, Is.EqualTo (2));
		}
	}
}
