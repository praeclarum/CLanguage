using System;
using System.Linq;
using UIKit;
using Foundation;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CLanguage.Editor
{
    public class CEditorController : UIViewController
    {
        static readonly bool ios11 = UIDevice.CurrentDevice.CheckSystemVersion (11, 0);

        readonly CLanguage.Editor.CEditor textEditor = new CLanguage.Editor.CEditor (UIScreen.MainScreen.Bounds);

        public CLanguage.Editor.CEditor Editor => textEditor;

        IDisposable keyboardObserver;

        public CEditorController ()
        {
            AddKeyCommand (UIKeyCommand.Create (new NSString ("]"), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("indent:"), new NSString ("Indent".Localize ())));
            AddKeyCommand (UIKeyCommand.Create (new NSString ("["), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("outdent:"), new NSString ("Outdent".Localize ())));
            AddKeyCommand (UIKeyCommand.Create (new NSString ("/"), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("toggleComment:"), new NSString ("Toggle Comment".Localize ())));
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            textEditor.AutoresizingMask = UIViewAutoresizing.None;
            textEditor.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddSubview (textEditor);
            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Leading, 1, 0));
            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Trailing, 1, 0));
            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Top, 1, 0));
            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Bottom, 1, 0));
        }

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
            View.BackgroundColor = Editor.Theme.BackgroundColor;
        }

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
            if (keyboardObserver == null) {
                keyboardObserver = UIKeyboard.Notifications.ObserveWillChangeFrame ((sender, e) => {
                    //Console.WriteLine ($"KEYBOARD MOVE TO " + e.FrameEnd);
                    var kframe = Editor.ConvertRectFromView (e.FrameEnd, null);
                    var intersection = Editor.Bounds;
                    intersection.Intersect (kframe);
                    //Console.WriteLine ($"INTERSECTION {intersection}");

                    var safeBottom = intersection.Height + 32; // Add about 2 lines to the bottom

                    if (ios11) {
                        AdditionalSafeAreaInsets = new UIEdgeInsets (0, 0, safeBottom, 0);
                    }
                    else {
                        safeBottom += 44; // Account for possible bottom bar
                        var ci = Editor.TextView.ContentInset;
                        Editor.TextView.ContentInset = new UIEdgeInsets (ci.Top, ci.Left, safeBottom, ci.Right);
                    }
                });
            }
        }

        public override void ViewDidDisappear (bool animated)
        {
            base.ViewDidDisappear (animated);
            keyboardObserver?.Dispose ();
            keyboardObserver = null;
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
        }

        [Export ("indent:")]
        public void Indent (NSObject sender) => textEditor.Indent (sender);

        [Export ("outdent:")]
        public void Outdent (NSObject sender) => textEditor.Outdent (sender);

        [Export ("toggleComment:")]
        public void ToggleComment (NSObject sender) => textEditor.ToggleComment (sender);
    }
}
