using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;

namespace CLanguage.Tests
{
	[TestClass]
	public class CompilerTests
	{
		Executable Compile (string code)
		{
            return CLanguageService.Compile (code, new ArduinoTestMachineInfo (), new TestPrinter ());
		}

		Executable CompileWithErrors (string code, params int[] errorCodes)
		{
			var printer = new TestPrinter (errorCodes);
            return CLanguageService.Compile (code, new ArduinoTestMachineInfo (), printer);
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
			var f = exe.Functions.OfType<CompiledFunction> ().First (x => x.Name == "f");
			Assert.AreEqual (f.Instructions.Count, 2);

			Assert.AreEqual (f.Instructions[0].Op, OpCode.LoadConstant);
			Assert.AreEqual (f.Instructions[1].Op, OpCode.Return);
		}

		[TestMethod]
		public void ReturnParamExpr ()
		{
			var exe = Compile (@"int f (int i) { return i + 42; }");
			var f = exe.Functions.OfType<CompiledFunction> ().First (x => x.Name == "f");
			Assert.AreEqual (f.Instructions.Count, 4);

			Assert.AreEqual (f.Instructions[0].Op, OpCode.LoadArg);
			Assert.AreEqual (f.Instructions[1].Op, OpCode.LoadConstant);
			Assert.AreEqual (f.Instructions[2].Op, OpCode.AddInt16);
			Assert.AreEqual (f.Instructions[3].Op, OpCode.Return);
		}

		[TestMethod]
		public void ConditionalReturn ()
		{
			var exe = Compile (@"int f (int i) { if (i) return 0; else return 42; }");
			var f = exe.Functions.OfType<CompiledFunction> ().First (x => x.Name == "f");
			Assert.AreEqual (f.Instructions.Count, 7);
			Assert.AreEqual (f.Instructions[0].Op, OpCode.LoadArg);
			Assert.AreEqual (f.Instructions[1].Op, OpCode.BranchIfFalse);
			Assert.AreEqual (f.Instructions[2].Op, OpCode.LoadConstant);
			Assert.AreEqual (f.Instructions[3].Op, OpCode.Return);
			Assert.AreEqual (f.Instructions[4].Op, OpCode.Jump);
			Assert.AreEqual (f.Instructions[5].Op, OpCode.LoadConstant);
			Assert.AreEqual (f.Instructions[6].Op, OpCode.Return);
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
			Assert.AreEqual (f.LocalVariables.Count, 3);
		}

		[TestMethod]
		public void ArduinoBlink ()
		{
			var exe = Compile (ArduinoInterpreterTests.BlinkCode);
            Assert.IsTrue (exe.Functions.Exists (x => x.Name == "loop"));
		}

		[TestMethod]
		public void ArduinoFade ()
		{
			var exe = Compile (ArduinoInterpreterTests.FadeCode);
            Assert.IsTrue (exe.Functions.Exists (x => x.Name == "loop"));
		}

        [TestMethod]
        public void CannotAssignToFuncalls ()
        {
            CompileWithErrors ("int foo() {return 1;} int main() { foo() = 42; return 0; }", 131);
        }

        [TestMethod]
        public void CannotAssignToFunctions ()
        {
            CompileWithErrors ("int foo() {return 1;} int main() { foo = 42; return 0; }", 30, 1656);
        }
	}
}
