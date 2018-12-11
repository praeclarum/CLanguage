using System;
using System.Linq;
using CLanguage.Interpreter;
using CLanguage.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CLanguage.CLanguageService;

namespace CLanguage.Tests
{
    [TestClass]
    public class PreprocessorTests : TestsBase
    {
        [TestMethod]
        public void AssignToDefines ()
        {
            var exe = Compile (@"
#define INPUT 1
#define OUTPUT 0
#define HIGH 255
#define LOW 0

int input = INPUT;
int output = OUTPUT;
int high = HIGH;
int low = LOW;

");
            Assert.AreEqual (5, exe.Globals.Count);
        }

        [TestMethod]
        public void DefineWithSimpleArg ()
        {
            Run (@"
#define ID(x) x
void main() {
    assertAreEqual(42, ID(42));
}
");
        }

        [TestMethod]
        public void DefineParamIncompleteArgs ()
        {
            Run (@"
#define ID(x x
void main() {
    assertAreEqual(42, ID(42));
}
", 1001, 103, 2064);
        }

        [TestMethod]
        public void DefineMultiline ()
        {
            Run (@"
#define DO i++; \
i++;
void main() {
    auto i = 0;
    DO
    DO
    assertAreEqual(4, i);
}
");
        }

        [TestMethod]
        public void DefineMultilineParams ()
        {
            Run (@"
#define DO(x, n) x++; \
x += n;
void main() {
    auto i = 0;
    DO(i, 1)
    DO(i, 2)
    DO(i, 3)
    assertAreEqual(9, i);
}
");
        }

        [TestMethod]
        public void IfndefTrue ()
        {
            Run (@"
#ifndef DO
#define DO(x) x++;
#endif
void main() {
    auto i = 0;
    DO(i)
    assertAreEqual(1, i);
}
");
        }

        [TestMethod]
        public void IfndefFalse ()
        {
            Run (@"
#define FOO
#ifndef FOO
error
#endif
void main() {
}
");
        }

        [TestMethod]
        public void IfdefTrue ()
        {
            Run (@"
#define FOO
#ifdef FOO
#define DO(x) x++;
#endif
void main() {
    auto i = 10;
    DO(i)
    assertAreEqual(11, i);
}
");
        }

        [TestMethod]
        public void IfdefFalse ()
        {
            Run (@"
#ifdef FOO
error
#endif
void main() {
}
");
        }

        [TestMethod]
        public void IfdefTrueElse ()
        {
            Run (@"
#define FOO
#ifdef FOO
#define DO(x) x++;
#else
#define DO(x) x = x * 20;
#endif
void main() {
    auto i = 10;
    DO(i)
    assertAreEqual(11, i);
}
");
        }

        [TestMethod]
        public void IfdefFalseElse ()
        {
            Run (@"
#ifdef FOO
#define DO(x) x++;
#else
#define DO(x) x = x * 20;
#endif
void main() {
    auto i = 10;
    DO(i)
    assertAreEqual(200, i);
}
");
        }

        [TestMethod]
        public void If1 ()
        {
            Run (@"
#if 1
#define DO(x) x++;
#endif
void main() {
    auto i = 10;
    DO(i)
    assertAreEqual(11, i);
}
");
        }

        [TestMethod]
        public void IfTrue ()
        {
            Run (@"
#if true
#define DO(x) x++;
#endif
void main() {
    auto i = 10;
    DO(i)
    assertAreEqual(11, i);
}
");
        }

        [TestMethod]
        public void IfFalseMath ()
        {
            Run (@"
#if 1-1
error
#endif
void main() {
}
");
        }

        [TestMethod]
        public void IfFalseElse ()
        {
            Run (@"
#if false
#else
#define DO(x) x++;
#endif
void main() {
    auto i = 10;
    DO(i)
    assertAreEqual(11, i);
}
");
        }

        [TestMethod]
        public void IfTrueVariable ()
        {
            Run (@"
#define FOO 1
#if FOO
#define DO(x) x++;
#endif
void main() {
    auto i = 10;
    DO(i)
    assertAreEqual(11, i);
}
");
        }

        [TestMethod]
        public void IfTrueVariableWithBadExpressions ()
        {
            Run (@"
#define FOO 1
#define BAR ](++
#if FOO
#define DO(x) x++;
#endif
void main() {
    auto i = 10;
    DO(i)
    assertAreEqual(11, i);
}
");
        }

        [TestMethod]
        public void VaArgs ()
        {
            Run (@"
#define DEBUG_PRINT(...) __VA_ARGS__
void main() {
    //TODO: __VA_ARGS__
    //auto i = DEBUG_PRINT(100);
    //assertAreEqual(100, i);
}
");
        }

        [TestMethod]
        public void BadVaArgs ()
        {
            Run (@"
#define DEBUG_PRINT(..) __VA_ARGS__
void main() {
    //TODO: __VA_ARGS__
    //auto i = DEBUG_PRINT(100);
    //assertAreEqual(100, i);
}
");
        }

        [TestMethod]
        public void IncompleteDefine ()
        {
            Run (@"
#define
void main() {
}
", 1025);
        }

        [TestMethod]
        public void IncludeRelativeFileUnknown ()
        {
            Run (@"
#include ""doesntexist.h""
void main() {
}
", 1027);
        }

        [TestMethod]
        public void IncludeRelativeStdint ()
        {
            Run (@"
#include ""stdint.h""
void main() {
    int16_t x = 2000;
    assertAreEqual(2000, x);
}
");
        }

        [TestMethod]
        public void IncludeAbsoluteStdint ()
        {
            Run (@"
#include <stdint.h>
void main() {
    int16_t x = 2000;
    assertAreEqual(2000, x);
}
");
        }


    }
}

