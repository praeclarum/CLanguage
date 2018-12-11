using System;

using UIKit;

namespace CEditor
{
    public class ViewController : UIViewController
    {
        readonly CLanguage.Editor.CEditor textEditor = new CLanguage.Editor.CEditor (UIScreen.MainScreen.Bounds);

        public ViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.AddSubview (textEditor);

            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Leading, 1, 0));
            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Trailing, 1, 0));
            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Top, 1, 0));
            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Bottom, 1, 0));
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }
    }
}
