#nullable enable

using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UIKit;

namespace CLanguageTestsiOS
{
    public partial class TestViewController : UITableViewController
    {
        List<TestResult> passResults = new();
        List<TestResult> failResults = new ();

        Task? testTask = null;

        public TestViewController () : base (UITableViewStyle.Grouped)
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
                testTask = TestRunner.RunTestsAsync (r =>
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
