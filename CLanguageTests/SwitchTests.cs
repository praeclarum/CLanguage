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

        [TestMethod]
        public void OnlyDefault ()
        {
            Run (@"
void main()
{
    int x = 6;
	switch (x)
	{
        default:
            x = 1000;
	}
    assertAreEqual(1000, x);
}
");
        }

        [TestMethod]
        public void OnlyDefaultBreak ()
        {
            Run (@"
void main()
{
    int x = 6;
	switch (x)
	{
        default:
            break;
            x = 1000;
	}
    assertAreEqual(6, x);
}
");
        }
    }
}
