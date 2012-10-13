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

		[Test, ExpectedException (typeof (ExecutionException))]
		public void InfiniteRecursionThrows ()
		{
			var i = Compile (@"
int f (int n) {
	return f (n);
}
void main () {
	f (1);
}");
			i.Reset ("main");
			i.Step ();
		}

		[Test]
		public void InfiniteLoopStopsEventually ()
		{
			var i = Compile (@"
void main () {
	while (true) {
	}
}");
			i.Reset ("main");
			i.Step ();
		}

		[Test]
		public void MutualRecursive ()
		{
			var i = Compile (@"
int m (int);

int f (int n) {
	return (n == 0) ? 1 : n - m (f (n - 1));
}
int m (int n) {
	return (n == 0) ? 0 : n - f (m (n - 1));
}
void main () {
	assertAreEqual (1, f (0));
	assertAreEqual (1, f (1));
	assertAreEqual (2, f (2));
	assertAreEqual (2, f (3));
	assertAreEqual (3, f (4));
	assertAreEqual (3, f (5));
	assertAreEqual (4, f (6));
	assertAreEqual (5, f (7));

	assertAreEqual (0, m (0));
	assertAreEqual (0, m (1));
	assertAreEqual (1, m (2));
	assertAreEqual (2, m (3));
	assertAreEqual (2, m (4));
	assertAreEqual (3, m (5));
	assertAreEqual (4, m (6));
	assertAreEqual (4, m (7));
}");
			i.Reset ("main");
			i.Step ();
		}

		[Test]
		public void Recursive ()
		{
			var i = Compile (@"
unsigned long fib (unsigned long n) {
	if (n == 0 || n == 1) {
		return n;
	}
	else {
		return fib (n - 1) + fib (n - 2);
	}
}
void main () {
	assertAreEqual (0, fib (0));
	assertAreEqual (1, fib (1));
	assertAreEqual (1, fib (2));
	assertAreEqual (2, fib (3));
	assertAreEqual (3, fib (4));
	assertAreEqual (5, fib (5));
	assertAreEqual (8, fib (6));
	assertAreEqual (13, fib (7));
}");
			i.Reset ("main");
			i.Step ();
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

