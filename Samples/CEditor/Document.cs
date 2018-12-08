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
        string code = "";

        public Document(IntPtr handle) : base(handle)
        {
            // Add your subclass-specific initialization here.
        }

        public override void WindowControllerDidLoadNib(NSWindowController windowController)
        {
            base.WindowControllerDidLoadNib(windowController);
            // Add any code here that needs to be executed once the windowController has loaded the document's window.
        }

        [Export("autosavesInPlace")]
        public static bool AutosaveInPlace()
        {
            return true;
        }

        public override void MakeWindowControllers()
        {
            // Override to return the Storyboard file name of the document.
            AddWindowController((NSWindowController)NSStoryboard.FromName("Main", null).InstantiateControllerWithIdentifier("Document Window Controller"));
        }

        public override NSData GetAsData(string typeName, out NSError outError)
        {
            try {
                var bytes = encoding.GetBytes (code);
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

        public override bool ReadFromData(NSData data, string typeName, out NSError outError)
        {
            try {
                using (var s = data.AsStream ())
                using (var r = new StreamReader (s)) {
                    code = r.ReadToEnd ();
                    encoding = r.CurrentEncoding;
                }
                outError = null;
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
