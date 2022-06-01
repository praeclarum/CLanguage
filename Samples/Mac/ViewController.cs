using System;

using AppKit;
using CLanguage.Editor;
using Foundation;
using CLanguage.Tests;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using CLanguage;

namespace CEditor
{
    public partial class ViewController : NSViewController
    {
        private Document document;

        readonly MachineInfo machineInfo = new ArduinoTestMachineInfo ();

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

        public ViewController ()
        {
            View = new NSView (new CGRect (0, 0, 320, 480));
        }

        public void MakeEditor ()
        {
            textEditor = new CLanguage.Editor.CEditor (View.Bounds);
            View.AddSubview (textEditor);
            textEditor.AutoresizingMask = NSViewResizingMask.HeightSizable | NSViewResizingMask.WidthSizable;
        }

        void BindDocument ()
        {
            if (document == null)
                return;
            document.CodeChanged += Document_CodeChanged;
            textEditor.SetCLanguage (machineInfo);
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

            var code = "void start() { __cinit(); main(); } " + document.Code;
            Task.Run (() => {
                var interpreter = CLanguage.CLanguageService.CreateInterpreter (code, machineInfo);
                interpreter.Reset ("start");
                interpreter.Step (1_000_000);
            });
        }
    }
}
