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
            var controller = (NSWindowController)NSStoryboard.FromName ("Main", null).InstantiateControllerWithIdentifier ("Document Window Controller");
            if (controller.ContentViewController is ViewController c) {
                c.Document = this;
            }
            AddWindowController (controller);
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
