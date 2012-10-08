using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

#if VS_UNIT_TESTING
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClassAttribute = NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute = NUnit.Framework.TestAttribute;
#endif

namespace CLanguage.Tests
{
    [TestClass]
    public class ParserTests
    {
    }
}
