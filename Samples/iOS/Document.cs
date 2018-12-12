using System;
using UIKit;
using Foundation;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace CEditor
{
    public class Document : UIDocument
    {
        public string Code { get; set; } = "";
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public Document (NSUrl initialUrl)
            : base (initialUrl)
        {
        }

        public override bool LoadFromContents (NSObject contents, string typeName, out NSError outError)
        {
            try {
                var data = (NSData)contents;
                if (data.Length > 0) {
                    using (var s = data.AsStream ())
                    using (var r = new StreamReader (s)) {
                        Code = r.ReadToEnd ();
                        Encoding = r.CurrentEncoding;
                    }
                }
                else {
                    Code = "";
                }
                outError = null;
                return true;
            }
            catch (Exception ex) {
                Debug.WriteLine (ex);
                outError = NSError.FromDomain (new NSString ("CEditor"), 1001);
                return false;
            }
        }

        public override NSObject ContentsForType (string typeName, out NSError outError)
        {
            try {
                var bytes = Encoding.GetBytes (Code);
                var data = NSData.FromArray (bytes);
                outError = null;
                return data;
            }
            catch (Exception ex) {
                Debug.WriteLine (ex);
                outError = NSError.FromDomain (new NSString ("CEditor"), 1002);
                return null;
            }
        }
    }
}
