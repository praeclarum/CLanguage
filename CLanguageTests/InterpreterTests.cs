using System;
using NUnit.Framework;

namespace CLanguage.Tests
{
	[TestFixture]
	public class InterpreterTests
	{
		Interpreter Compile (string code)
		{
			var c = new Compiler (
				new Report (new TestPrinter ()),
				new ArduinoTestMachineInfo ());
			c.AddCode (code);
			var exe = c.Compile ();
			return new Interpreter (exe);
		}


		[Test]
		public void LocalVariableInitialization ()
		{
			var i = Compile (@"
void math () {
	int a = 4;
	int b = 8;
	int c = a + b;
	assertAreEqual (12, c);
}");
			i.Start ("math");
		}
	}
}

