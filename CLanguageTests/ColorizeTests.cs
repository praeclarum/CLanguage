using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;
using System.Globalization;
using CLanguage.Syntax;

namespace CLanguage.Tests
{
    [TestClass]
    public class ColorizeTests
    {
        void AssertColorization (string code, params SyntaxColor[] expectedColors)
        {
            var mi = new MachineInfo ();
            mi.AddInternalFunction ("void delay()");
            mi.HeaderCode = "#define FOO 1\n\nvoid delay();\n\n";
            var colors = CLanguageService.Colorize (code, mi);
            Assert.AreEqual (expectedColors.Length, colors.Length, "Number of colored tokens don't match.");
            for (int i = 0; i < colors.Length; i++) {
                var color = colors[i];
                var ecolor = expectedColors[i];
                Assert.IsTrue (color.Length > 0, "Span has length == 0");
                Assert.AreEqual (ecolor, color.Color);
            }
        }

        [TestMethod]
        public void Expression ()
        {
            AssertColorization ("42 / 100.0 + 50",
                                SyntaxColor.Number, 
                                SyntaxColor.Operator, 
                                SyntaxColor.Number, 
                                SyntaxColor.Operator,
                                SyntaxColor.Number);
        }

        [TestMethod]
        public void IfStatement ()
        {
            AssertColorization ("if (true) {}",
                                SyntaxColor.Keyword,
                                SyntaxColor.Operator,
                                SyntaxColor.Number,
                                SyntaxColor.Operator,
                                SyntaxColor.Operator,
                                SyntaxColor.Operator);
        }

        [TestMethod]
        public void IntTypeDecl ()
        {
            AssertColorization ("int x = 42;",
                                SyntaxColor.Type,
                                SyntaxColor.Identifier,
                                SyntaxColor.Operator,
                                SyntaxColor.Number,
                                SyntaxColor.Operator);
        }

        [TestMethod]
        public void StringLiteral ()
        {
            AssertColorization ("*\"Hello\"",
                                SyntaxColor.Operator,
                                SyntaxColor.String);
        }

        [TestMethod]
        public void CharLiteral ()
        {
            AssertColorization ("'h'",
                                SyntaxColor.String);
        }

        [TestMethod]
        public void SimpleStatement ()
        {
            AssertColorization ("f;",
                                SyntaxColor.Identifier,
                                SyntaxColor.Operator);
        }

        [TestMethod]
        public void UnknownFuncall ()
        {
            AssertColorization ("foo();",
                                SyntaxColor.Identifier,
                                SyntaxColor.Operator,
                                SyntaxColor.Operator,
                                SyntaxColor.Operator);
        }

        [TestMethod]
        public void KnownFuncall ()
        {
            AssertColorization ("delay();",
                                SyntaxColor.Function,
                                SyntaxColor.Operator,
                                SyntaxColor.Operator,
                                SyntaxColor.Operator);
        }

        [TestMethod]
        public void VoidFundef ()
        {
            AssertColorization ("void foo();",
                                SyntaxColor.Keyword,
                                SyntaxColor.Identifier,
                                SyntaxColor.Operator,
                                SyntaxColor.Operator,
                                SyntaxColor.Operator);
        }

        [TestMethod]
        public void MultilineComment ()
        {
            AssertColorization ("2 + /* la dee \n\n\n daaa */ foo",
                                SyntaxColor.Number,
                                SyntaxColor.Operator,
                                SyntaxColor.Identifier);
        }


        [TestMethod]
        public void MachineInfoDefine ()
        {
            AssertColorization ("FOO + 2",
                                SyntaxColor.Identifier,
                                SyntaxColor.Operator,
                                SyntaxColor.Number);
        }

        [TestMethod]
        public void UserDefine ()
        {
            AssertColorization ("#define FOOFOO 100\n\nFOOFOO + 3",
                                SyntaxColor.Operator,
                                SyntaxColor.Identifier,
                                SyntaxColor.Identifier,
                                SyntaxColor.Number,
                                SyntaxColor.Identifier,
                                SyntaxColor.Operator,
                                SyntaxColor.Number);
        }

        [TestMethod]
        public void UnsignedSigned ()
        {
            AssertColorization ("signed int x; unsigned int y;",
                                SyntaxColor.Type,
                                SyntaxColor.Type,
                                SyntaxColor.Identifier,
                                SyntaxColor.Operator,
                                SyntaxColor.Type,
                                SyntaxColor.Type,
                                SyntaxColor.Identifier,
                                SyntaxColor.Operator);
        }
    }
}
