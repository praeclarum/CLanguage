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
		class TestPrinter : ReportPrinter
		{
			public override void Print (AbstractMessage msg)
			{
				if (msg.MessageType == "Error") {
					Assert.Fail (msg.ToString ());
				}
				else {
					Console.WriteLine (msg);
				}
			}
		}
		Compiler CreateCompiler ()
		{
			return new Compiler (new Report (new TestPrinter ()), MachineInfo.Arduino);
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
			Assert.That (f.Instructions[0], Is.AssignableTo<PushInstruction> ());
			Assert.That (f.Instructions[1], Is.AssignableTo<ReturnInstruction> ());
		}

		[TestMethod]
		public void ReturnParamExpr ()
		{
			var exe = Compile (@"int f (int i) { return i + 42; }");
			Assert.That (exe.Functions.Count, Is.EqualTo (1));
			var f = exe.Functions.First (x => x.Name == "f");
			Assert.That (f.Instructions.Count, Is.EqualTo (4));
			Assert.That (f.Instructions[0], Is.AssignableTo<LoadArgInstruction> ());
			Assert.That (f.Instructions[1], Is.AssignableTo<PushInstruction> ());
			Assert.That (f.Instructions[2], Is.AssignableTo<BinopInstruction> ());
			Assert.That (f.Instructions[3], Is.AssignableTo<ReturnInstruction> ());
		}

		[TestMethod]
		public void ConditionalReturn ()
		{
			var exe = Compile (@"int f (int i) { if (i) return 0; else return 42; }");
			Assert.That (exe.Functions.Count, Is.EqualTo (1));
			var f = exe.Functions.First (x => x.Name == "f");
			Assert.That (f.Instructions.Count, Is.EqualTo (7));
			Assert.That (f.Instructions[0], Is.AssignableTo<LoadArgInstruction> ());
			Assert.That (f.Instructions[1], Is.AssignableTo<BranchIfFalseInstruction> ());
			Assert.That (f.Instructions[2], Is.AssignableTo<PushInstruction> ());
			Assert.That (f.Instructions[3], Is.AssignableTo<ReturnInstruction> ());
			Assert.That (f.Instructions[4], Is.AssignableTo<JumpInstruction> ());
			Assert.That (f.Instructions[5], Is.AssignableTo<PushInstruction> ());
			Assert.That (f.Instructions[6], Is.AssignableTo<ReturnInstruction> ());
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
