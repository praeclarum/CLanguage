using System;
using System.Linq;
using Foundation;
using System.Threading.Tasks;
using System.Diagnostics;

#if __IOS__
using UIKit;

namespace CLanguage.Editor
{
    public class CEditorController : UIViewController
    {
        readonly CLanguage.Editor.CEditor textEditor = new CLanguage.Editor.CEditor (UIScreen.MainScreen.Bounds);

        public CLanguage.Editor.CEditor Editor => textEditor;

        public CEditorController ()
        {
            AddKeyCommand (UIKeyCommand.Create (new NSString ("]"), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("indent:"), new NSString ("Indent".Localize ())));
            AddKeyCommand (UIKeyCommand.Create (new NSString ("["), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("outdent:"), new NSString ("Outdent".Localize ())));
            AddKeyCommand (UIKeyCommand.Create (new NSString ("/"), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("toggleComment:"), new NSString ("Toggle Comment".Localize ())));
            AddKeyCommand (UIKeyCommand.Create (new NSString ("="), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("increaseFontSize:"), new NSString ("Increase Font Size".Localize ())));
            AddKeyCommand (UIKeyCommand.Create (new NSString ("-"), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("decreaseFontSize:"), new NSString ("Decrease Font Size".Localize ())));
            AddKeyCommand (UIKeyCommand.Create (new NSString ("0"), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("restoreFontSize:"), new NSString ("Restore Font Size".Localize ())));
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

        [Export ("increaseFontSize:")]
        public void IncreaseFontSize (NSObject sender) => textEditor.IncreaseFontSize (sender);

        [Export ("decreaseFontSize:")]
        public void DecreaseFontSize (NSObject sender) => textEditor.DecreaseFontSize (sender);

        [Export ("restoreFontSize:")]
        public void RestoreFontSize (NSObject sender) => textEditor.RestoreFontSize (sender);
    }
}
#endif
