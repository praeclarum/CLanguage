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
using System.Collections.Generic;
#endif

namespace CLanguage.Editor
{
    class EditorTextView : NativeTextView
    {
        Dictionary<int, char> autoChars = new Dictionary<int, char> ();

        public EditorTextView (CGRect frameRect) : base (frameRect)
        {
        }

        string tabIndent = "  ";

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
                var insert = true;
                if (autoChars.Count > 0 && s.Length == 1 && sr.Length == 0 && autoChars.TryGetValue ((int)sr.Location, out var ch) && s.Length == 1 && s[0] == ch) {
                    autoChars.Remove ((int)sr.Location);
                    insert = false;
                    nsr = new NSRange (sr.Location + 1, 0);
                }

                var ac = -1;
                if (nsr == null) {
                    switch (s) {
                        case "\t" when sr.Length == 0:
                            r = tabIndent;
                            break;
                        case "(" when sr.Length == 0:
                            r = "()";
                            nsr = new NSRange (sr.Location + 1, 0);
                            ac = (int)sr.Location + 1;
                            autoChars[ac] = ')';
                            break;
                        case "[" when sr.Length == 0:
                            r = "[]";
                            nsr = new NSRange (sr.Location + 1, 0);
                            ac = (int)sr.Location + 1;
                            autoChars[ac] = ']';
                            break;
                        case "{" when sr.Length == 0:
                            r = "{\n" + tabIndent + "\n}";
                            nsr = new NSRange (sr.Location + 2 + tabIndent.Length, 0);
                            break;
                    }
                }
                if (insert) {
                    OffsetAutoChars (r != null ? r.Length : s.Length, rr, ac);
                    base.InsertText (r != null ? new NSString (r) : text, rr);
                }
                if (nsr.HasValue) {
                    BeginInvokeOnMainThread (() => SelectedRange = nsr.Value);
                }
            }
        }

        void OffsetAutoChars (int textLength, NSRange replacementRange, int ignoreIndex)
        {
            RemoveEarlierAutoChars (replacementRange.Location + replacementRange.Length);

            var o = textLength - (int)replacementRange.Length;
            var n = new Dictionary<int, char> ();

            foreach (var kv in autoChars) {
                var nk = kv.Key == ignoreIndex ? kv.Key : kv.Key + o;
                n[nk] = kv.Value;
            }
            autoChars = n;
        }

        void RemoveEarlierAutoChars (nint location)
        {
            var l = (int)location;
            var r = new List<int> ();
            foreach (var k in autoChars.Keys) {
                if (k < location) {
                    r.Add (k);
                }
            }
            foreach (var k in r) {
                autoChars.Remove (k);
            }
        }
    }
}
