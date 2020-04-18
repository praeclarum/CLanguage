using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;

namespace CLanguage.Tests
{
	[TestClass]
	public class InterpreterTests : TestsBase
	{
        [TestMethod]
        public void YieldingDelay ()
        {
            var mi = new TestMachineInfo ();
            bool[] hit = new bool[3];
            mi.AddInternalFunction ("int yieldingDelay(int ms)", i => {
                var ms = i.ReadArg (0).Int16Value;
                Console.WriteLine ($"RUN Y={i.YieldedValue}");
                hit[i.YieldedValue] = true;
                if (i.YieldedValue == 0) {
                    i.Yield (1);
                }
                else if (i.YieldedValue == 1) {
                    i.Yield (2);
                }
                else {
                    i.Yield (0);
                    i.Push (ms * 1000);
                }
            });
            var it = Run (@"
void main () {
    auto x = yieldingDelay(3);
    assertAreEqual (3000, x);
}", mi);
            Assert.IsTrue (hit[0]);
            Assert.IsFalse (hit[1]);
            Assert.IsFalse (hit[2]);
            it.Step ();
            Assert.IsTrue (hit[0]);
            Assert.IsTrue (hit[1]);
            Assert.IsFalse (hit[2]);
            it.Step ();
            Assert.IsTrue (hit[0]);
            Assert.IsTrue (hit[1]);
            Assert.IsTrue (hit[2]);
        }

        [TestMethod]
		public void InfiniteRecursionThrows ()
		{
			try {
				var i = Run (@"
int f (int n) {
	return f (n);
}
void main () {
	f (1);
}");

				Assert.Fail ("Expected ExecutionException but got nothing");
			}
			catch (ExecutionException) {
			}
			catch (Exception) {
				throw;
			}
		}

		[TestMethod]
		public void InfiniteLoopStopsEventually ()
		{
			var i = Run (@"
void main () {
	while (true) {
	}
}");
		}

		[TestMethod]
		public void MutualRecursive ()
		{
			var i = Run (@"
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
		}

		[TestMethod]
		public void Recursive ()
		{
			var i = Run (@"
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
		}

		[TestMethod]
		public void OverwriteArgs ()
		{
			var i = Run (@"
int abs (int x) {
	if (x < 0) x = -x;
	return x;
}
void main () {
	assertAreEqual (0, abs(0));
	assertAreEqual (101, abs(-101));
	assertAreEqual (101, abs(101));
}");
		}

		[TestMethod]
		public void VoidFunctionCalls ()
		{
			var i = Run (@"
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
		}

		[TestMethod]
		public void FunctionCallsWithValues ()
		{
			var i = Run (@"
int mulMulDiv (long m1, long m2, long d) {
	return (m1 * m2) / d;
}

void main () {
	assertAreEqual (66, mulMulDiv (2, 100, 3));
}");
		}


		[TestMethod]
		public void ForLoop ()
		{
			var i = Run (@"
void main () {
	int acc;
	int i;
	for (acc = 0, i = -10; i <= 10; i += 2) {
		acc = acc + 1;
	}
	assertAreEqual (11, acc);
}");
		}

		[TestMethod]
		public void ForLoopWithBreak ()
		{
			var i = Run (@"
void main () {
	int i;
	for (i = 0; i <= 10; i++) {
		if (i >= 5)
            break;
	}
	assertAreEqual (5, i);
}");
		}

		[TestMethod]
		public void AintNoLoopForBreak ()
		{
			Assert.ThrowsException<AssertFailedException> (() => {
				Run (@"
void main () {
    break;
}");
			});
		}

		[TestMethod]
		public void AintNoLoopForContinue ()
		{
			Assert.ThrowsException<AssertFailedException> (() => {
				Run (@"
void main () {
    continue;
}");
			});
		}

		[TestMethod]
		public void ForLoopWithContinue ()
		{
			var i = Run (@"
void main () {
    int otherI = 0;
	int i;
	for (i = 0; i <= 10; i++) {
		if (i >= 5) {
            continue;
        }
        otherI++;
	}
	assertAreEqual (5, otherI);
}");
		}

		[TestMethod]
		public void SizeofInt ()
		{
			var i = Run (@"
void main () {
	int s = sizeof(int);
	assertAreEqual (2, s);
}");
		}

		[TestMethod]
		public void SizeofSizeofInt ()
		{
			var i = Run (@"
void main () {
	int s = sizeof(sizeof(int));
	assertAreEqual (4, s);
}");
		}

		[TestMethod]
		public void SizeofIntPtr ()
		{
			var i = Run (@"
void main () {
	int s = sizeof(int*);
	assertAreEqual (2, s);
}");
		}

		[TestMethod]
		public void SizeofLongInt ()
		{
			var i = Run (@"
void main () {
	int s = sizeof(long int);
	assertAreEqual (4, s);
}");
		}

		[TestMethod]
		public void SizeofArray ()
		{
			var i = Run (@"
int array[] = { 0, 1, 2, 3, 4 };
void main () {
	int num = sizeof(array) / sizeof(array[0]);
    int num2 = sizeof(array) / sizeof(int);
	assertAreEqual (5, num);
    assertAreEqual (5, num2);
}");
		}

		[TestMethod]
		public void LocalVariableInitialization ()
		{
			var i = Run (@"
void main () {
	int a = 4;
	int b = 8;
	int c = a + b;
	assertAreEqual (12, c);
}");
		}

        [TestMethod]
        public void GlobalVariableInitialization ()
        {
            var i = Run (@"
int a = 4;
int b = 8;
int c = a + b;
void main () {
    assertAreEqual (12, c);
}");
        }

        [TestMethod]
        public void AddressOfLocal ()
        {
            var i = Run (@"
void main () {
    int a = 4;
    int *pa = &a;
    assertAreEqual (4, *pa);
    a = *pa + 1;
    assertAreEqual (5, *pa);
}");
        }

        [TestMethod]
        public void AddressOfGlobal ()
        {
            var i = Run (@"
int a = 0;
void main () {
    int *pa = &a;
    assertAreEqual (0, *pa);
    a = *pa + 1;
    assertAreEqual (1, *pa);
}");
        }

        [TestMethod]
        public void PreDecrement ()
        {
            var i = Run (@"
void main () {
    int a = 100;
    assertAreEqual (99, --a);
}");
        }

        [TestMethod]
        public void PreIncrement ()
        {
            var i = Run (@"
void main () {
    int a = 100;
    assertAreEqual (101, ++a);
}");
        }

        [TestMethod]
        public void PostDecrement ()
        {
            var i = Run (@"
void main () {
    int a = 100;
    assertAreEqual (100, a--);
    assertAreEqual (99, a);
}");
        }

        [TestMethod]
        public void PostIncrement ()
        {
            var i = Run (@"
void main () {
    int a = 100;
    assertAreEqual (100, a++);
    assertAreEqual (101, a);
}");
        }

        [TestMethod]
        public void BoolAssignment ()
        {
            var i = Run (@"
void main () {
    bool a = false;
    assertAreEqual (false, a);
    assertAreEqual ((bool)0, a);
    a = true;
    assertAreEqual (true, a);
    assertAreEqual ((bool)1, a);
}");
        }

        [TestMethod]
        public void BoolLoopEnd ()
        {
            var i = Run (@"
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
        }
	}
}

