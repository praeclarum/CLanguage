using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Foo;

namespace Foo
{
    public class TestClassAttribute : Attribute
    {
    }
}

namespace CLanguageTestsiOS
{
    public static class TestRunner
    {
        public static async Task RunTestsAsync()
        {
            var asm = Assembly.GetExecutingAssembly ();
            var testClasses = asm.DefinedTypes.Where (x => x.GetCustomAttribute<TestClassAttribute> () is object).ToArray();
        }
    }
}

