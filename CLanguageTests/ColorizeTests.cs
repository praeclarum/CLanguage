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
            Assert.AreEqual (expectedColors.Length, colors.Length);
            for (int i = 0; i < colors.Length; i++) {
                var color = colors[i];
                var ecolor = expectedColors[i];
                Assert.AreEqual (ecolor, color.Color);
            }
        }

        [TestMethod]
        public void Expression ()
        {
            AssertColorization ("42 / 100.0 + 50",
                                SyntaxColor.Constant, 
                                SyntaxColor.Operator, 
                                SyntaxColor.Constant, 
                                SyntaxColor.Operator,
                                SyntaxColor.Constant);
        }

        [TestMethod]
        public void IfStatement ()
        {
            AssertColorization ("if (true) {}",
                                SyntaxColor.Keyword,
                                SyntaxColor.Operator,
                                SyntaxColor.Constant,
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
                                SyntaxColor.Constant,
                                SyntaxColor.Operator);
        }

        [TestMethod]
        public void StringLiteral ()
        {
            AssertColorization ("*\"Hello\"",
                                SyntaxColor.Operator,
                                SyntaxColor.Constant);
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
                                SyntaxColor.Constant,
                                SyntaxColor.Operator,
                                SyntaxColor.Identifier);
        }


        [TestMethod]
        public void MachineInfoDefine ()
        {
            AssertColorization ("FOO + 2",
                                SyntaxColor.Identifier,
                                SyntaxColor.Operator,
                                SyntaxColor.Constant);
        }

        [TestMethod]
        public void UserDefine ()
        {
            AssertColorization ("#define FOOFOO 100\n\nFOOFOO + 3",
                                SyntaxColor.Identifier,
                                SyntaxColor.Operator,
                                SyntaxColor.Constant);
        }
    }
}
