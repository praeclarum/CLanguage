using System;

using AppKit;
using CLanguage.Editor;
using Foundation;
using CLanguage.Tests;
using System.Linq;
using System.Threading.Tasks;

namespace CEditor
{
    public partial class ViewController : NSViewController
    {
        private Document document;

        public Document Document {
            get => document;
            set {
                if (ReferenceEquals (document, value))
                    return;
                UnbindDocument ();
                document = value;
                BindDocument ();
            }
        }

        public override NSObject RepresentedObject {
            get => Document;
            set => Document = value as Document;
        }

        public ViewController (IntPtr handle) : base (handle)
        {
        }

        void BindDocument ()
        {
            if (document == null)
                return;
            document.CodeChanged += Document_CodeChanged;
            textEditor.Options = new CLanguage.Compiler.CompilerOptions (new ArduinoTestMachineInfo ());
            //await Task.Delay (1000);
            textEditor.Text = document.Code;
            textEditor.TextChanged += TextEditor_TextChanged;
        }

        void UnbindDocument ()
        {
            if (document == null)
                return;
            textEditor.TextChanged -= TextEditor_TextChanged;
            document.CodeChanged -= Document_CodeChanged;
        }

        void Document_CodeChanged (object sender, EventArgs e)
        {
            textEditor.Text = document.Code;
        }

        void TextEditor_TextChanged (object sender, EventArgs e)
        {
            document.Code = textEditor.Text;
        }
    }
}
