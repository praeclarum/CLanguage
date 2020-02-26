#nullable enable

using System;

using AppKit;
using Foundation;
using System.Text;
using System.IO;

namespace CEditor
{
    [Register("Document")]
    public class Document : NSDocument
    {
        Encoding encoding = Encoding.UTF8;
        public string Code { get; set; } = "";

        public event EventHandler? CodeChanged;

        public Document (IntPtr handle) : base (handle)
        {
        }

        [Export("autosavesInPlace")]
        public static bool AutosaveInPlace()
        {
            return true;
        }

        public override void MakeWindowControllers()
        {
            var controller = new NSWindowController ("EditorWindow");
            //Console.WriteLine ("Made window controller: " + controller);
            //var c = new ViewController ();
            //c.MakeEditor ();
            //c.Document = this;
            //controller.LoadWindow ();
            //controller.Window.ContentViewController = c;
            //controller.ContentViewController = c;
            AddWindowController (controller);
        }

        public override NSData? GetAsData(string typeName, out NSError? outError)
        {
            try {
                var bytes = encoding.GetBytes (Code);
                var data = NSData.FromArray (bytes);
                outError = null;
                return data;
            }
            catch (Exception ex) {
                Console.WriteLine (ex);
                outError = NSError.FromDomain (new NSString ("CEditor"), 101);
                return null;
            }
        }

        public override bool ReadFromData(NSData data, string typeName, out NSError? outError)
        {
            try {
                using (var s = data.AsStream ())
                using (var r = new StreamReader (s)) {
                    Code = r.ReadToEnd ();
                    encoding = r.CurrentEncoding;
                }
                outError = null;
                BeginInvokeOnMainThread (() => CodeChanged?.Invoke (this, EventArgs.Empty));
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine (ex);
                outError = NSError.FromDomain (new NSString ("CEditor"), 101);
                return false;
            }
        }
    }
}
