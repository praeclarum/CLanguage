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
				new ArduinoTestMachineInfo (),
				new Report (new TestPrinter ()));
			c.AddCode (code);
			var exe = c.Compile ();
			return new Interpreter (exe);
		}

		[Test]
		public void FunctionCallsWithValues ()
		{
			var i = Compile (@"
int mulMulDiv (long m1, long m2, long d) {
	return (m1 * m2) / d;
}

void main () {
	assertAreEqual (66, mulMulDiv (2, 100, 3));
}");
			i.Reset ("main");
			i.Step ();
		}


		[Test]
		public void ForLoop ()
		{
			var i = Compile (@"
void main () {
	int acc;
	int i;
	for (acc = 0, i = -10; i <= 10; i += 2) {
		acc = acc + 1;
	}
	assertAreEqual (11, acc);
}");
			i.Reset ("main");
			i.Step ();
		}

		[Test]
		public void LocalVariableInitialization ()
		{
			var i = Compile (@"
void main () {
	int a = 4;
	int b = 8;
	int c = a + b;
	assertAreEqual (12, c);
}");
			i.Reset ("main");
			i.Step ();
		}
	}
}

