using System;
using System.Linq;
using CLanguage.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static CLanguage.CLanguageService;

namespace CLanguage.Tests
{
	[TestClass]
	public class PreprocessorTests
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
	}
}

