using System;
using System.Linq;
using CLanguage.Tests;
using UIKit;
using Foundation;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CEditor
{
    public class ViewController : CLanguage.Editor.CEditorController
    {
        readonly Task<NSUrl> initialDocumentUrlTask;

        Document document;

        public ViewController (Task<NSUrl> initialDocumentUrlTask)
        {
            this.initialDocumentUrlTask = initialDocumentUrlTask;

            Editor.SetCLanguage (new ArduinoTestMachineInfo ());
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            LoadDocumentAsync ().ContinueWith (_ => { });
        }

        async Task LoadDocumentAsync ()
        {
            try {
                var url = await initialDocumentUrlTask;
                var doc = new Document (url);
                var opened = await doc.OpenAsync ();
                document = doc;
                Title = document.LocalizedName;
                Editor.Text = doc.Code;
                Editor.TextChanged += TextEditor_TextChanged;
            }
            catch (Exception ex) {
                Debug.WriteLine (ex);
            }
        }

        void TextEditor_TextChanged (object sender, EventArgs e)
        {
            var d = document;
            if (d != null) {
                d.Code = Editor.Text;
                d.UpdateChangeCount (UIDocumentChangeKind.Done);

                //var interpreter = CLanguage.CLanguageService.CreateInterpreter (d.Code, Editor.Options.MachineInfo);
                //interpreter.Reset ("main");
                //interpreter.Step (1_000_000);
            }
        }
    }
}
