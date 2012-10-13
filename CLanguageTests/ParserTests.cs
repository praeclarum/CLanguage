using System;
using System.Linq;
using NUnit.Framework;

namespace CLanguage.Tests
{
	[TestFixture]
	public class ParserTests
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

		[Test]
		public void ForLoopWithThreeInits ()
		{
			var tu = Parse (@"
void f () {
	int acc;
	int i;
	int j;
	for (i = -10, acc = 0, j = 42; i <= 10; i += 2) {
		acc = acc + 1;
	}
	assertAreEqual (11, acc);
}");
			var f = tu.Functions.First (x => x.Name == "f");
			var forS = (ForStatement)f.Body.Statements[0];
			var init = forS.InitBlock;
			Assert.That (init.Statements.Count, Is.EqualTo (1));
			var expr = (SequenceExpression)((ExpressionStatement)init.Statements[0]).Expression;
			Assert.That (expr.First, Is.InstanceOf<SequenceExpression> ());
			var sexpr = (SequenceExpression)expr.First;
			Assert.That (((VariableExpression)((AssignExpression)sexpr.First).Left).VariableName, Is.EqualTo ("i"));
			Assert.That (((VariableExpression)((AssignExpression)sexpr.Second).Left).VariableName, Is.EqualTo ("acc"));
			Assert.That (expr.Second, Is.InstanceOf<AssignExpression> ());
			Assert.That (((VariableExpression)((AssignExpression)expr.Second).Left).VariableName, Is.EqualTo ("j"));
		}
	}
}

