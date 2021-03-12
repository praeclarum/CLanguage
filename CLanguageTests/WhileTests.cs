using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class WhileTests : TestsBase
    {
        [TestMethod, Ignore ("Issue #19")]
        public void InnerForLoop ()
        {
            var i = Run (@"
void main()
{
	int s = 0;
	
	while (s < 3)
	{
		for (int pos = 5; pos < 7; pos++)
		{
		}
		stack++;
	}
}
");
        }
    }
}
