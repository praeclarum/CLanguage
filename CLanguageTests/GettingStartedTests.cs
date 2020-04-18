using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
    [TestClass]
    public class GettingStartedTests
    {
        [TestMethod]
        public void EasyRun ()
        {
            var code = @"
void main() {
}";
            CLanguageService.Run (code);
        }

        [TestMethod]
        public void EasyEval ()
        {
            var result = CLanguageService.Eval ("2 + 3");
            Assert.AreEqual (5, result);
        }

        [TestMethod]
        public void EasyEvalMore ()
        {
            var result = CLanguageService.Eval ("x * 100", "int x = 42;");
            Assert.AreEqual (4200, result);
        }

    }
}
