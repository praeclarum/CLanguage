using System;
using System.Linq;
using CLanguage.Tests;
using UIKit;
using Foundation;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CEditor
{
    public class ViewController : UIViewController
    {
        readonly CLanguage.Editor.CEditor textEditor = new CLanguage.Editor.CEditor (UIScreen.MainScreen.Bounds);

        readonly Task<NSUrl> initialDocumentUrlTask;

        Document document;

        public ViewController (Task<NSUrl> initialDocumentUrlTask)
        {
            this.initialDocumentUrlTask = initialDocumentUrlTask;

            AddKeyCommand (UIKeyCommand.Create (new NSString ("]"), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("indent:")));
            AddKeyCommand (UIKeyCommand.Create (new NSString ("["), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("outdent:")));
            AddKeyCommand (UIKeyCommand.Create (new NSString ("/"), UIKeyModifierFlags.Command, new ObjCRuntime.Selector ("toggleComment:")));

            textEditor.Options = new CLanguage.Compiler.CompilerOptions (
                new ArduinoTestMachineInfo (),
                new CLanguage.Report (),
                Enumerable.Empty<CLanguage.Syntax.Document> ());
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.AddSubview (textEditor);
            textEditor.AutoresizingMask = UIViewAutoresizing.None;
            textEditor.TranslatesAutoresizingMaskIntoConstraints = false;
            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Leading, 1, 0));
            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Trailing, 1, 0));
            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Top, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Top, 1, 0));
            View.AddConstraint (NSLayoutConstraint.Create (View, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, textEditor, NSLayoutAttribute.Bottom, 1, 0));

            LoadDocumentAsync ().ContinueWith (_ => { });
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
        }

        [Export ("indent:")]
        void Indent (NSObject sender) => textEditor.Indent (sender);

        [Export ("outdent:")]
        void Outdent (NSObject sender) => textEditor.Outdent (sender);

        [Export ("toggleComment:")]
        void ToggleComment (NSObject sender) => textEditor.ToggleComment (sender);

        async Task LoadDocumentAsync ()
        {
            try {
                var url = await initialDocumentUrlTask;
                var doc = new Document (url);
                var opened = await doc.OpenAsync ();
                document = doc;
                Title = document.LocalizedName;
                textEditor.Text = doc.Code;
                textEditor.TextChanged += TextEditor_TextChanged;
            }
            catch (Exception ex) {
                Debug.WriteLine (ex);
            }
        }

        void TextEditor_TextChanged (object sender, EventArgs e)
        {
            var d = document;
            if (d != null) {
                d.Code = textEditor.Text;
                d.UpdateChangeCount (UIDocumentChangeKind.Done);
            }
        }
    }
}
