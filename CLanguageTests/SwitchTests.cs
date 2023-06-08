using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class SwitchTests : TestsBase
    {
        [TestMethod]
        public void EmptySwitch ()
        {
            Run (@"
void main()
{
    int x = 6;
	switch (x)
	{
	}
    assertAreEqual(6, x);
}
");
        }
    }
}
