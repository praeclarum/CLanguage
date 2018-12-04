using System;
using System.Linq;
using CLanguage.Interpreter;
using CLanguage.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CLanguage.CLanguageService;
using System.Collections.Generic;

namespace CLanguage.Tests
{
    [TestClass]
    public class InternalObjectTests : TestsBase
    {
        class Calc
        {
            readonly List<int> stack = new List<int> ();
            public void Push (int x)
            {
                stack.Add (x);
            }
            public int Pop ()
            {
                var x = stack[stack.Count - 1];
                stack.RemoveAt (stack.Count - 1);
                return x;
            }
        }

        [TestMethod]
        public void CalcMethods ()
        {
            var mi = new ArduinoTestMachineInfo ();
            mi.AddGlobalReference ("c", new Calc ());
            Run (@"
void main() {
    c.Push(678);
    assertAreEqual(678, c.Pop());
}
", mi);
        }
    }
}

