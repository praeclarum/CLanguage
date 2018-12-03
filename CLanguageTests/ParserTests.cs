using System;
using System.Linq;
using CLanguage.Syntax;
using CLanguage.Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using static CLanguage.CLanguageService;
using CLanguage.Interpreter;

namespace CLanguage.Tests
{
	[TestClass]
	public class ParserTests
	{
		[TestMethod]
		public void BadFunction ()
		{
			var failed = false;
			try {
				var tu = ParseTranslationUnit (@"void setup()
{
pinMode (4, OUTPUT);
}

void loop()
{
pinMode
sleep(1000);
}", new TestPrinter ());
			}
            catch {
				failed = true;
			}
			if (!failed) {
				Assert.Fail ("Shouldn't have compiled");
			}
		}

		[TestMethod]
		public void ForLoopWithThreeInits ()
		{
            var exe = Compile (@"
void f () {
	int acc;
	int i;
	int j;
	for (i = -10, acc = 0, j = 42; i <= 10; i += 2) {
		acc = acc + 1;
	}
}");
            var f = (CompiledFunction)exe.Functions.First (x => x.Name == "f");
			var forS = (ForStatement)f.Body.Statements[3];
			var init = forS.InitBlock;
			Assert.AreEqual (init.Statements.Count, 1);
			var expr = (SequenceExpression)((ExpressionStatement)init.Statements[0]).Expression;
			Assert.IsInstanceOfType (expr.First, typeof(SequenceExpression));
			var sexpr = (SequenceExpression)expr.First;
			Assert.AreEqual (((VariableExpression)((AssignExpression)sexpr.First).Left).VariableName, "i");
			Assert.AreEqual (((VariableExpression)((AssignExpression)sexpr.Second).Left).VariableName, "acc");
			Assert.IsInstanceOfType (expr.Second, typeof(AssignExpression));
			Assert.AreEqual (((VariableExpression)((AssignExpression)expr.Second).Left).VariableName, "j");
		}

        [TestMethod]
        public void HexNumbers ()
        {
            var lex = new Lexer ("", "0x201");
            lex.Advance ();
            Assert.AreEqual (TokenKind.CONSTANT, lex.CurrentToken.Kind);
            Assert.AreEqual (513, lex.CurrentToken.Value);
        }

        [TestMethod]
        public void EmojiIds ()
        {
            AssertId ("🎃");
            AssertId ("🎃", "🎃=0;");
        }

        [TestMethod]
        public void NonEnglishIds ()
        {
            AssertId ("ὸ");
            AssertId ("あ");
            AssertId ("あ", "あ/2");
            AssertId ("あ", "あ (2");
        }

        [TestMethod]
        public void BadSymbols ()
        {
            AssertId ("´");
            AssertId ("⁼");
        }

        void AssertId (string expectedId, string code = null)
        {
            var lex = new Lexer ("", code ?? expectedId);
            lex.Advance ();
            Assert.AreEqual (TokenKind.IDENTIFIER, lex.CurrentToken.Kind);
            Assert.AreEqual (expectedId, lex.CurrentToken.Value);
        }
    }
}

