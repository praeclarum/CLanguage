using System;

using AppKit;
using Foundation;

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

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
        }

        void BindDocument ()
        {
            if (document == null)
                return;
            document.CodeChanged += Document_CodeChanged;
            textEditor.Text = document.Code;
        }

        void UnbindDocument ()
        {
            if (document == null)
                return;
            document.CodeChanged -= Document_CodeChanged;
        }

        void Document_CodeChanged (object sender, EventArgs e)
        {
            textEditor.Text = document.Code;
        }
    }
}
