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
    }
}
