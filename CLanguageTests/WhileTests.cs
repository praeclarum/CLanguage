using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class WhileTests : TestsBase
    {
        [TestMethod]
        public void InnerForLoop ()
        {
            Run (@"
void main()
{
	int s = 0;
    int a = 0;
	
	while (s < 3)
	{
		for (int pos = 5; pos < 7; pos++)
		{
            a++;
		}
		s++;
	}
    assertAreEqual(6, a);
}
");
        }
    }
}
