using System;
using System.Linq;
using CLanguage.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
	[TestClass]
	public class PreprocessorTests
	{
		TranslationUnit Parse (string code)
		{
			var report = new Report (new TestPrinter ());
			var pp = new Preprocessor (report);
			pp.AddCode ("stdin", code);
			var lexer = new Lexer (pp);
			var parser = new CParser ();
			return parser.ParseTranslationUnit (lexer, report);
		}

		[TestMethod]
		public void AssignToDefines ()
		{
			var tu = Parse (@"
#define INPUT 1
#define OUTPUT 0
#define HIGH 255
#define LOW 0

int input = INPUT;
int output = OUTPUT;
int high = HIGH;
int low = LOW;

");
			Assert.IsInstanceOfType (((AssignExpression)((ExpressionStatement)tu.Statements[0]).Expression).Right, typeof(ConstantExpression));
			Assert.IsInstanceOfType (((AssignExpression)((ExpressionStatement)tu.Statements[1]).Expression).Right, typeof(ConstantExpression));
			Assert.IsInstanceOfType (((AssignExpression)((ExpressionStatement)tu.Statements[2]).Expression).Right, typeof(ConstantExpression));
			Assert.IsInstanceOfType (((AssignExpression)((ExpressionStatement)tu.Statements[3]).Expression).Right, typeof(ConstantExpression));

			Assert.AreEqual (((ConstantExpression)((AssignExpression)((ExpressionStatement)tu.Statements[0]).Expression).Right).EmitValue, 1);
			Assert.AreEqual (((ConstantExpression)((AssignExpression)((ExpressionStatement)tu.Statements[1]).Expression).Right).EmitValue, 0);
			Assert.AreEqual (((ConstantExpression)((AssignExpression)((ExpressionStatement)tu.Statements[2]).Expression).Right).EmitValue, 255);
			Assert.AreEqual (((ConstantExpression)((AssignExpression)((ExpressionStatement)tu.Statements[3]).Expression).Right).EmitValue, 0);
		}
	}
}

