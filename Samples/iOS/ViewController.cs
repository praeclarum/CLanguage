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

        private readonly Task<NSUrl> initialDocumentUrlTask;

        Document document;

        public ViewController (Task<NSUrl> initialDocumentUrlTask)
        {
            textEditor.Options = new CLanguage.Compiler.CompilerOptions (
                new ArduinoTestMachineInfo (),
                new CLanguage.Report (),
                Enumerable.Empty<CLanguage.Syntax.Document> ());
            this.initialDocumentUrlTask = initialDocumentUrlTask;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.AddSubview (textEditor);

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
