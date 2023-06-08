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

        [TestMethod]
        public void HitOnly ()
        {
            Run (@"
void main()
{
    int x = 6;
	switch (x)
	{
        case 1:
            x = 100;
            break;
        case 6:
            x = 600;
            break;
        default:
            x = 1000;
	}
    assertAreEqual(600, x);
}
");
        }

        [TestMethod]
        public void HitDefault ()
        {
            Run (@"
void main()
{
    int x = 5;
	switch (x)
	{
        default:
            x = 1000;
            break;
        case 1:
            x = 100;
            break;
        case 6:
            x = 600;
            break;
	}
    assertAreEqual(1000, x);
}
");
        }

        [TestMethod]
        public void NoDefault ()
        {
            Run (@"
void main()
{
    int x = 6;
	switch (x)
	{
        case 1:
            x = 100;
            break;
        case 5:
            x = 500;
            break;
	}
    assertAreEqual(6, x);
}
");
        }

        [TestMethod]
        public void Fallthrough ()
        {
            Run (@"
void main()
{
    int x = 5;
    switch (x)
    {
        case 1:
            x = 100;
            break;
        case 5:
            x = 500;
        case 6:
            x = 600;
            break;
    }
    assertAreEqual(600, x);
}");
        }
    }
}


