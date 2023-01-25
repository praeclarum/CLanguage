#nullable enable

using Foundation;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UIKit;

namespace CLanguageTestsiOS
{
    public partial class TestViewController : UITableViewController
    {
        class TestResult
        {
            public bool Pass;
            public bool Fail => !Pass;
            public string Name;
            public string? Error;

            public TestResult (string name, bool pass, string? error)
            {
                Pass = pass;
                Name = name;
                Error = error;
            }
        }

        TestResult[] results = Array.Empty<TestResult>();

        Task? testTask = null;

        public TestViewController () : base (UITableViewStyle.Plain)
        {
            Title = "Tests";
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
                testTask = TestRunner.RunTestsAsync ();
            }
        }
    }
}
