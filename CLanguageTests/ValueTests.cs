using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CLanguage.Interpreter;

namespace CLanguage.Tests
{
    [TestClass]
    public class ValueTests
    {
        [TestMethod]
        public void ByteFromInt ()
        {
            Assert.AreEqual (1, ((Value)0x10001).UInt8Value);
        }
    }
}
