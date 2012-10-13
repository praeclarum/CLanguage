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
		public void OverwriteArgs ()
		{
			var i = Compile (@"
int abs (int x) {
	if (x < 0) x = -x;
	return x;
}
void main () {
	assertAreEqual (0, abs(0));
	assertAreEqual (101, abs(-101));
	assertAreEqual (101, abs(101));
}");
			i.Reset ("main");
			i.Step ();
		}
		[Test]
		public void VoidFunctionCalls ()
		{
			var i = Compile (@"
int output = 0;
void print (int v) {
	output += v;
}
void main () {
	print (1);
	print (2);
	print (3);
	assertAreEqual (6, output);
}");
			i.Reset ("main");
			i.Step ();
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

