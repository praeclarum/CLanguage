using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class VirtualTests : TestsBase
    {
        [TestMethod, Ignore]
        public void BaseWithVirtualFunction ()
        {
            Run (@"
class B {
    virtual int f() { return 42; }
}

void main()
{
    B b;
    assertAreEqual(43, b.f());
}
");
        }
    }
}
