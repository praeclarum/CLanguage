using System;

using Foundation;
using CoreGraphics;

#if __IOS__
using UIKit;
using NativeColor = UIKit.UIColor;
using NativeFont = UIKit.UIFont;
using NativeStringAttributes = UIKit.UIStringAttributes;
#elif __MACOS__
using NativeTextView = AppKit.NSTextView;
#endif

namespace CLanguage.Editor
{
    class EditorTextView : NativeTextView
    {
        public EditorTextView (CGRect frameRect) : base (frameRect)
        {
        }

        public override void InsertText (NSObject text, NSRange replacementRange)
        {
            var sr = SelectedRange;
            //Console.WriteLine ($"{sr} => \"{text}\"");

            var s = (text is NSString ns)
                ? ns.ToString ()
                : ((text is NSAttributedString nas)
                    ? nas.ToString ()
                    : default (string));

            if (s != null) {
                var r = default (string);
                var rr = sr;
                var nsr = default (NSRange?);
                switch (s) {
                    case "\t" when sr.Length == 0:
                        r = "  ";
                        break;
                    case "(" when sr.Length == 0:
                        r = "()";
                        nsr = new NSRange (sr.Location + 1, 0);
                        break;
                    case "{" when sr.Length == 0:
                        r = "{\n  \n}";
                        nsr = new NSRange (sr.Location + 4, 0);
                        break;
                    //case "}" when sr.Length == 0 && sr.Location > 1:
                        //r = "}";
                        //rr = new NSRange (sr.Location + sr.Length - 2, 2);
                        //break;
                }
                base.InsertText (r != null ? new NSString (r) : text, rr);
                if (nsr.HasValue) {
                    BeginInvokeOnMainThread (() => SelectedRange = nsr.Value);
                }
            }
        }
    }
}
