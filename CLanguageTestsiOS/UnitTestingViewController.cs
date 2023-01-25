#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Foundation;
using UIKit;

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
        public static void AreEqual (int x, int y, string? message = null)
        {
            if (x != y)
                throw new AssertFailedException ($"{x} != {y}");
        }

        public static void AreEqual (double x, double y, double eps)
        {
            if (Math.Abs (x - y) > eps)
                throw new AssertFailedException ($"{x} != {y}");
        }

        public static void AreEqual (float x, float y, float eps)
        {
            if (MathF.Abs (x - y) > eps)
                throw new AssertFailedException ($"{x} != {y}");
        }

        public static void AreEqual (object x, object y, string? message = null)
        {
            if (!x.Equals (y))
                throw new AssertFailedException ($"{x} != {y}");
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
            if (!t.IsAssignableFrom (o.GetType ()))
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
            var threwObj = default (Exception);
            try {
                action ();
            }
            catch (Exception ex) {
                threwObj = ex;
            }
            if (threwObj is T) {
            }
            else if (threwObj is object) {
                throw new AssertFailedException ($"Threw wrong type: {threwObj.GetType ()} (expected {typeof (T)})");
            }
            else {
                throw new AssertFailedException ($"Didn't throw");
            }
        }
    }

    public class AssertFailedException : Exception
    {
        public AssertFailedException (string message) : base (message)
        {
        }
    }

    static class TestRunner
    {
        public static async Task RunAssemblyTestsAsync (Assembly testAssembly, Action<TestResult> handleResult)
        {
            var testClasses =
                testAssembly.DefinedTypes
                .Where (x => x.GetCustomAttribute<TestClassAttribute> () is object).ToArray ();
            foreach (var testClass in testClasses) {
                var testObj = Activator.CreateInstance (testClass);
                var testMethods =
                    testClass.DeclaredMethods
                    .Where (x => x.GetCustomAttribute<TestMethodAttribute> () is object && x.GetCustomAttribute<IgnoreAttribute> () is not object)
                    .ToArray ();
                foreach (var testMethod in testMethods) {
                    var r = new TestResult ($"{testClass.Name}.{testMethod.Name}", true);
                    try {
                        var taskO = testMethod.Invoke (testObj, Array.Empty<object> ());
                        if (taskO is Task task) {
                            await task;
                        }
                        r.Error = null;
                        r.Pass = true;
                    }
                    catch (Exception ex) {
                        r.Pass = false;
                        var iex = ex;
                        while (iex.InnerException is object)
                            iex = iex.InnerException;
                        r.Error = (iex is AssertFailedException) ? iex.Message : iex.ToString ();
                    }
                    handleResult (r);
                    await Task.Delay (1);
                }
            }
        }
    }

    public class TestResult
    {
        public readonly string Name;
        public bool Pass;
        public string? Error;

        public bool Fail => !Pass;

        public TestResult (string name, bool pass = false, string? error = null)
        {
            Pass = false;
            Name = name;
            Error = "Not Run";
        }
    }

    public partial class UnitTestingViewController : UITableViewController
    {
        readonly Assembly testAssembly;

        List<TestResult> passResults = new();
        List<TestResult> failResults = new ();

        Task? testTask = null;

        public UnitTestingViewController (Assembly? assembly = null) : base (UITableViewStyle.Grouped)
        {
            testAssembly = Assembly.GetExecutingAssembly();
            Title = $"{testAssembly.GetName().Name} Tests";
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
        }

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
            if (testTask is not object) {
                testTask = TestRunner.RunAssemblyTestsAsync (testAssembly, r =>
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        (r.Pass ? passResults : failResults).Insert(0, r);
                        TableView.InsertRows(new[] { NSIndexPath.FromRowSection(0, r.Pass ? 1 : 0) }, UITableViewRowAnimation.Automatic);
                    });
                });
            }
        }

        public override string TitleForHeader (UITableView tableView, nint section)
        {
            if (section == 0)
                return "Failing Tests";
            return "Passing Tests";
        }

        public override nint NumberOfSections (UITableView tableView)
        {
            return 2;
        }

        public override nint RowsInSection (UITableView tableView, nint section)
        {
            if (section == 0)
                return failResults.Count;
            return passResults.Count;
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            if (tableView.DequeueReusableCell("T") is not UITableViewCell cell)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, "T");
            }
            var result = indexPath.Section == 0 ? failResults[indexPath.Row] : passResults[indexPath.Row];
            cell.TextLabel.Text = result.Name;
            cell.TextLabel.TextColor = result.Pass ? UIColor.SystemGreen : UIColor.SystemRed;
            cell.DetailTextLabel.Text = result.Error;
            return cell;
        }
    }
}
