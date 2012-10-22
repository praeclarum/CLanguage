using System;
using System.Linq;

#if NETFX_CORE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#endif

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
			Assert.That (((AssignExpression)((ExpressionStatement)tu.Statements[0]).Expression).Right, Is.InstanceOf<ConstantExpression> ());
			Assert.That (((AssignExpression)((ExpressionStatement)tu.Statements[1]).Expression).Right, Is.InstanceOf<ConstantExpression> ());
			Assert.That (((AssignExpression)((ExpressionStatement)tu.Statements[2]).Expression).Right, Is.InstanceOf<ConstantExpression> ());
			Assert.That (((AssignExpression)((ExpressionStatement)tu.Statements[3]).Expression).Right, Is.InstanceOf<ConstantExpression> ());

			Assert.That (((ConstantExpression)((AssignExpression)((ExpressionStatement)tu.Statements[0]).Expression).Right).EmitValue, Is.EqualTo (1));
			Assert.That (((ConstantExpression)((AssignExpression)((ExpressionStatement)tu.Statements[1]).Expression).Right).EmitValue, Is.EqualTo (0));
			Assert.That (((ConstantExpression)((AssignExpression)((ExpressionStatement)tu.Statements[2]).Expression).Right).EmitValue, Is.EqualTo (255));
			Assert.That (((ConstantExpression)((AssignExpression)((ExpressionStatement)tu.Statements[3]).Expression).Right).EmitValue, Is.EqualTo (0));
		}
	}
}

