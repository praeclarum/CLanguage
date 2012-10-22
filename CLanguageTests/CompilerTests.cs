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
	[TestClass]
	public class CompilerTests
	{
		Executable Compile (string code)
		{
			var c = new Compiler (MachineInfo.Arduino, new Report (new TestPrinter ()));
			c.AddCode (code);
			return c.Compile ();
		}

		Executable CompileWithErrors (string code, params int[] errorCodes)
		{
			var printer = new TestPrinter (errorCodes);
			var c = new Compiler (MachineInfo.Arduino, new Report (printer));
			c.AddCode (code);
			var exe = c.Compile ();
			printer.CheckForErrors ();
			return exe;
		}

		[TestMethod]
		public void ErrorIfDoesntReturn ()
		{
			CompileWithErrors (@"int f () { int a = 42; }", 161);
		}

		[TestMethod]
		public void ErrorIfDoesntReturnValue ()
		{
			CompileWithErrors (@"int f () { int a = 42; return; }", 126);
		}

		[TestMethod]
		public void ReturnConstant ()
		{
			var exe = Compile (@"int f () { return 42; }");
			Assert.That (exe.Functions.Count, Is.EqualTo (exe.MachineInfo.InternalFunctions.Count + 1));
			var f = exe.Functions.OfType<CompiledFunction> ().First (x => x.Name == "f");
			Assert.That (f.Instructions.Count, Is.EqualTo (2));

			Assert.That (f.Instructions[0].Op, Is.EqualTo (OpCode.LoadValue));
			Assert.That (f.Instructions[1].Op, Is.EqualTo (OpCode.Return));
		}

		[TestMethod]
		public void ReturnParamExpr ()
		{
			var exe = Compile (@"int f (int i) { return i + 42; }");
			Assert.That (exe.Functions.Count, Is.EqualTo (exe.MachineInfo.InternalFunctions.Count + 1));
			var f = exe.Functions.OfType<CompiledFunction> ().First (x => x.Name == "f");
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
			Assert.That (exe.Functions.Count, Is.EqualTo (exe.MachineInfo.InternalFunctions.Count + 1));
			var f = exe.Functions.OfType<CompiledFunction> ().First (x => x.Name == "f");
			Assert.That (f.Instructions.Count, Is.EqualTo (7));
			Assert.That (f.Instructions[0].Op, Is.EqualTo (OpCode.LoadArg));
			Assert.That (f.Instructions[1].Op, Is.EqualTo (OpCode.BranchIfFalse));
			Assert.That (f.Instructions[2].Op, Is.EqualTo (OpCode.LoadValue));
			Assert.That (f.Instructions[3].Op, Is.EqualTo (OpCode.Return));
			Assert.That (f.Instructions[4].Op, Is.EqualTo (OpCode.Jump));
			Assert.That (f.Instructions[5].Op, Is.EqualTo (OpCode.LoadValue));
			Assert.That (f.Instructions[6].Op, Is.EqualTo (OpCode.Return));
		}

		[TestMethod]
		public void VoidFunctionsHaveNoValue ()
		{
			CompileWithErrors (@"
void f () {
}
void main () {
	int a = f ();
}", 30);
		}

		[TestMethod]
		public void LocalVariables ()
		{
			var exe = Compile (@"
void f () {
	int a = 4;
	int b = 8;
	int c = a + b;
}");
			var f = exe.Functions.OfType<CompiledFunction> ().First (x => x.Name == "f");
			Assert.That (f.LocalVariables.Count, Is.EqualTo (3));
		}

		[TestMethod]
		public void ArduinoBlink ()
		{
			var exe = Compile (ArduinoTests.BlinkCode);
			Assert.That (exe.Functions.Count, Is.EqualTo (exe.MachineInfo.InternalFunctions.Count + 2));
		}

		[TestMethod]
		public void ArduinoFade ()
		{
			var exe = Compile (ArduinoTests.FadeCode);
			Assert.That (exe.Functions.Count, Is.EqualTo (exe.MachineInfo.InternalFunctions.Count + 2));
		}
	}
}
