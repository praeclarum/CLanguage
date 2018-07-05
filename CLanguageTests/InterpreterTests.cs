using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;

namespace CLanguage.Tests
{
	[TestClass]
	public class InterpreterTests
	{
		CInterpreter Compile (string code)
		{
            return CLanguageService.CreateInterpreter (code, new ArduinoTestMachineInfo (), printer: new TestPrinter ());
		}

		[TestMethod]
		public void InfiniteRecursionThrows ()
		{
			try {
				var i = Compile (@"
int f (int n) {
	return f (n);
}
void main () {
	f (1);
}");
				i.Reset ("main");
				i.Step ();

				Assert.Fail ("Expected ExecutionException but got nothing");
			}
			catch (ExecutionException) {
			}
			catch (Exception ex) {
				Assert.Fail ("Expected ExecutionException but got " + ex);
			}
		}

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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

		[TestMethod]
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


		[TestMethod]
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

		[TestMethod]
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

        [TestMethod, Ignore]
        public void AddressOfLocal ()
        {
            var i = Compile (@"
void main () {
    int a = 4;
    int *pa = &a;
    assertAreEqual (4, *pa);
    a = *pa + 1;
    assertAreEqual (5, *pa);
}");
            i.Reset ("main");
            i.Step ();
        }

        [TestMethod]
        public void AddressOfGlobal ()
        {
            var i = Compile (@"
int a = 0;
void main () {
    int *pa = &a;
    assertAreEqual (0, *pa);
    a = *pa + 1;
    assertAreEqual (1, *pa);
}");
            i.Reset ("main");
            i.Step ();
        }

        [TestMethod]
        public void PreDecrement ()
        {
            var i = Compile (@"
void main () {
    int a = 100;
    assertAreEqual (99, --a);
}");
            i.Reset ("main");
            i.Step ();
        }

        [TestMethod]
        public void PreIncrement ()
        {
            var i = Compile (@"
void main () {
    int a = 100;
    assertAreEqual (101, ++a);
}");
            i.Reset ("main");
            i.Step ();
        }

        [TestMethod]
        public void PostDecrement ()
        {
            var i = Compile (@"
void main () {
    int a = 100;
    assertAreEqual (100, a--);
    assertAreEqual (99, a);
}");
            i.Reset ("main");
            i.Step ();
        }

        [TestMethod]
        public void PostIncrement ()
        {
            var i = Compile (@"
void main () {
    int a = 100;
    assertAreEqual (100, a++);
    assertAreEqual (101, a);
}");
            i.Reset ("main");
            i.Step ();
        }

        [TestMethod]
        public void BoolAssignment ()
        {
            var i = Compile (@"
void main () {
    bool a = false;
    assertAreEqual (false, a);
    assertAreEqual ((bool)0, a);
    a = true;
    assertAreEqual (true, a);
    assertAreEqual ((bool)1, a);
}");
            i.Reset ("main");
            i.Step ();
        }

        [TestMethod]
        public void BoolLoopEnd ()
        {
            var i = Compile (@"
void main () {
    int i = 0;
    bool b = true;
    while (b) {
        i++;
        b = i < 10;
    }    
    assertAreEqual (false, b);
    assertAreEqual (10, i);
}");
            i.Reset ("main");
            i.Step ();
        }
	}
}

