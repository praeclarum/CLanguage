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
using System.IO;
using System.Diagnostics;
using System.Linq;
#endif

namespace CLanguage.Editor
{
    class EditorTextView : NativeTextView
    {
        string userIndent = "  ";

        Dictionary<int, char> autoChars = new Dictionary<int, char> ();

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
                var insert = true;
                if (autoChars.Count > 0 && s.Length == 1 && sr.Length == 0 && autoChars.TryGetValue ((int)sr.Location, out var ch) && s.Length == 1 && s[0] == ch) {
                    autoChars.Remove ((int)sr.Location);
                    insert = false;
                    nsr = new NSRange (sr.Location + 1, 0);
                }

                var ac = -1;
                if (nsr == null) {
                    var lines = GetLinesInRange (sr);
                    switch (s) {
                        case "\t" when sr.Length == 0:
                            r = userIndent;
                            break;
                        case "\n" when sr.Location > 0 && sr.Length == 0 && lines.AllText[(int)sr.Location - 1] == '{':
                            r = "\n" + lines.Indent + userIndent;
                            nsr = new NSRange (sr.Location + 1 + lines.Indent.Length + userIndent.Length, 0);
                            break;
                        case "\n" when sr.Length == 0 && lines.Indent.Length > 0:
                            r = "\n" + lines.Indent;
                            nsr = new NSRange (sr.Location + 1 + lines.Indent.Length, 0);
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
                        case "{" when sr.Length == 0 && lines.AtEndOfLines (sr.Location): {
                                r = "{\n" + lines.Indent + userIndent + "\n" + lines.Indent + "}";
                                nsr = new NSRange (sr.Location + 2 + lines.Indent.Length + userIndent.Length, 0);
                            }
                            break;
                        case "\"" when sr.Length == 0:
                            r = "\"\"";
                            nsr = new NSRange (sr.Location + 1, 0);
                            ac = (int)sr.Location + 1;
                            autoChars[ac] = '\"';
                            break;
                        case "\'" when sr.Length == 0:
                            r = "\'\'";
                            nsr = new NSRange (sr.Location + 1, 0);
                            ac = (int)sr.Location + 1;
                            autoChars[ac] = '\'';
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

        static string GetLineIndent (string line)
        {
            var e = 0;
            while (e < line.Length && char.IsWhiteSpace (line[e]))
                e++;
            return line.Substring (0, e);
        }

        class LinesInRange
        {
            public NSRange Range;
            public string Indent = "";
            public string AllText = "";
            public readonly List<string> Lines = new List<string> ();
            public override string ToString () => $"{Lines.Count} lines";
            public bool AtEndOfLines (nint location) => location == (Range.Location + Range.Length);
        }

        public void MutateLinesInRange (NSRange range, Action<List<string>> mutator)
        {
            var lines = GetLinesInRange (range);
            mutator (lines.Lines);
            var newLinesText = string.Join ("\n", lines.Lines);
            TextStorage.Replace (lines.Range, newLinesText);
        }

        LinesInRange GetLinesInRange (NSRange range)
        {
            var lines = new LinesInRange ();

            var text = Value;
            var lineStartIndex = (int)range.Location;
            var lineEndIndex = (int)(range.Location + range.Length);

            while (lineStartIndex > 0 && text[lineStartIndex - 1] != '\n') {
                lineStartIndex--;
            }
            while (lineEndIndex < text.Length && text[lineEndIndex] != '\n') {
                lineEndIndex++;
            }

            lines.Range = new NSRange (lineStartIndex, lineEndIndex - lineStartIndex);

            var s = lineStartIndex;
            var i = s;
            while (i < lineEndIndex) {
                if (text[i] == '\n') {
                    lines.Lines.Add (text.Substring (s, i - s));
                    s = i + 1;
                }
                else {
                    i++;
                }
            }
            if (i - s >= 0) {
                lines.Lines.Add (text.Substring (s, i - s));
            }

            var inCharCount = lineEndIndex - lineStartIndex;
            var outCharCount = lines.Lines.Sum (x => x.Length) + lines.Lines.Count - 1;
            Debug.Assert (outCharCount == inCharCount, $"Line output char count doesn't match: {outCharCount} != {inCharCount}");

            lines.Indent = GetLineIndent (lines.Lines[0]);
            lines.AllText = text;
            return lines;
        }
    }
}
