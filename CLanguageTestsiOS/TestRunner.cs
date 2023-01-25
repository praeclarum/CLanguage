#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public class TestClassAttribute : Attribute
    {
    }

    public class TestMethodAttribute : Attribute
    {
    }

    public class IgnoreAttribute : Attribute
    {
    }

    public static class Assert
    {
        public static void AreEqual(int x, int y, string? message = null)
        {
            if (x != y)
                throw new AssertFailedException($"{x} != {y}");
        }

        public static void AreEqual(double x, double y, double eps)
        {
            if (Math.Abs(x - y) > eps)
                throw new AssertFailedException($"{x} != {y}");
        }

        public static void AreEqual(float x, float y, float eps)
        {
            if (MathF.Abs(x - y) > eps)
                throw new AssertFailedException($"{x} != {y}");
        }

        public static void AreEqual(object x, object y, string? message = null)
        {
            if (!x.Equals(y))
                throw new AssertFailedException($"{x} != {y}");
        }

        public static void AreEqual (string x, string y, string? message = null)
        {
            if (x != y)
                throw new AssertFailedException ($"{x} != {y}");
        }

        public static void Fail (string message)
        {
            throw new AssertFailedException ($"Fail: {message}");
        }

        public static void IsInstanceOfType (object o, Type t, string? message = null)
        {
            if (o is not object)
                throw new AssertFailedException ($"Object is null");
            if (!t.IsAssignableFrom(o.GetType()))
                throw new AssertFailedException ($"Object is not {t}");
        }

        public static void IsFalse (bool x, string? message = null)
        {
            if (x)
                throw new AssertFailedException ($"Value is true");
        }

        public static void IsNotNull (object o, string? message = null)
        {
            if (o is not object)
                throw new AssertFailedException ($"Object is null");
        }

        public static void IsTrue (bool x, string? message = null)
        {
            if (!x)
                throw new AssertFailedException ($"Value is false");
        }

        public static void ThrowsException<T> (Action action)
        {
            var threwObj = default(Exception);
            try
            {
                action();
            }
            catch (Exception ex)
            {
                threwObj = ex;
            }
            if (threwObj is T)
            {
            }
            else if (threwObj is object)
            {
                throw new AssertFailedException($"Threw wrong type: {threwObj.GetType()} (expected {typeof(T)})");
            }
            else
            {
                throw new AssertFailedException($"Didn't throw");
            }
        }
    }

    public class AssertFailedException : Exception
    {
        public AssertFailedException(string message) : base(message)
        {
        }
    }
}

namespace CLanguageTestsiOS
{
    static class TestRunner
    {
        public static async Task RunTestsAsync(Action<TestResult> handleResult)
        {
            var asm = Assembly.GetExecutingAssembly ();
            var testClasses = asm.DefinedTypes.Where (x => x.GetCustomAttribute<TestClassAttribute> () is object).ToArray();
            foreach (var testClass in testClasses)
            {
                var testObj = Activator.CreateInstance(testClass);
                var testMethods = testClass.DeclaredMethods.Where(x => x.GetCustomAttribute<TestMethodAttribute>() is object && x.GetCustomAttribute<IgnoreAttribute>() is not object).ToArray();
                foreach (var testMethod in testMethods)
                {
                    var r = new TestResult ($"{testClass.Name}.{testMethod.Name}", true);
                    try
                    {
                        var taskO = testMethod.Invoke(testObj, Array.Empty<object>());
                        if (taskO is Task task)
                        {
                            await task;
                        }
                        r.Error = null;
                        r.Pass = true;
                    }
                    catch (Exception ex)
                    {
                        r.Pass = false;
                        var iex = ex;
                        while (iex.InnerException is object)
                            iex = iex.InnerException;
                        r.Error = (iex is AssertFailedException) ? iex.Message : iex.ToString();
                    }
                    handleResult(r);
                    await Task.Delay(1);
                }
            }
        }
    }

    class TestResult
    {
        public bool Pass;
        public bool Fail => !Pass;
        public string Name;
        public string? Error;

        public TestResult (string name, bool pass = false, string? error = null)
        {
            Pass = false;
            Name = name;
            Error = "Not Run";
        }
    }
}

