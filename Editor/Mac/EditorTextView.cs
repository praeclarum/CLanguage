using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;

using Foundation;
using CoreGraphics;

#if __IOS__
using UIKit;
using NativeTextView = UIKit.UITextView;
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
        string userIndent = "  ";

        Dictionary<int, char> autoChars = new Dictionary<int, char> ();

        public EditorTextView (CGRect frameRect)
#if __MACOS__
            : base (frameRect)
#elif __IOS__
            : base (frameRect, CreateContainer (frameRect.Size))
#endif
        {
        }

#if __MACOS__
        public override string[] ReadablePasteboardTypes ()
        {
            var types = NSString.ReadableTypeIdentifiers;
            return types;
        }

        public override void InsertText (NSObject text, NSRange replacementRange)
        {
            var s = (text is NSString ns)
                ? ns.ToString ()
                : ((text is NSAttributedString nas)
                    ? nas.ToString ()
                    : default (string));
            if (s == null)
                return;
            var insert = InsertTextIntoSelection (s, SelectedRange);
            if (insert.Insert.HasValue) {
                var r = insert.Insert.Value.Text;
                base.InsertText (r != null ? new NSString (r) : text, insert.Insert.Value.Range);
            }
            if (insert.SelectRange.HasValue) {
                BeginInvokeOnMainThread (() => SelectedRange = insert.SelectRange.Value);
            }
        }
#elif __IOS__
        static NSTextContainer CreateContainer (CGSize size)
        {
            var storage = new EditorTextStorage ();
            var container = new NSTextContainer (new CGSize (size.Width, nfloat.MaxValue)) {
                WidthTracksTextView = true,
                LineBreakMode = UILineBreakMode.WordWrap,
            };
            var layout = new NSLayoutManager ();
            storage.AddLayoutManager (layout);
            layout.AddTextContainer (container);
            return container;
        }

        public override void InsertText (string text)
        {
            var r = InsertTextIntoSelection (text, SelectedRange);
            if (r.Insert.HasValue) {
                base.InsertText (r.Insert.Value.Text ?? text);
            }
            if (r.SelectRange.HasValue) {
                BeginInvokeOnMainThread (() => SelectedRange = r.SelectRange.Value);
            }
        }
#endif

        ((string Text, NSRange Range)? Insert, NSRange? SelectRange) InsertTextIntoSelection (string s, NSRange sr)
        {
            //Console.WriteLine ($"{sr} => \"{text}\"");

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
                    case "\n" when sr.Location > 0 && sr.Length == 0 
                        && (lines.AllText[(int)sr.Location - 1] == '{' || lines.AllText[(int)sr.Location - 1] == ':'):
                        r = "\n" + lines.Indent + userIndent;
                        nsr = new NSRange (sr.Location + 1 + lines.Indent.Length + userIndent.Length, 0);
                        break;
                    case "\n" when sr.Length == 0 && lines.Indent.Length > 0:
                        r = "\n" + lines.Indent;
                        nsr = new NSRange (sr.Location + 1 + lines.Indent.Length, 0);
                        break;
                    case ";\n" when sr.Length == 0:
                        r = ";\n" + lines.Indent;
                        nsr = new NSRange (sr.Location + 2 + lines.Indent.Length, 0);
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
            var insertValue = default ((string, NSRange)?);
            if (insert) {
                OffsetAutoChars (r != null ? r.Length : s.Length, rr, ac);
                insertValue = (r, rr);
            }
            return (insertValue, nsr);
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

        public class LinesInRange
        {
            public NSRange Range;
            public string Indent = "";
            public string AllText = "";
            public readonly List<string> Lines = new List<string> ();
            public override string ToString () => $"{Lines.Count} lines";
            public bool AtEndOfLines (nint location) => location == (Range.Location + Range.Length);
        }

        void ChangeLinesInRange (NSRange range, Func<string, NSRange, List<string>, IEnumerable<string>> map)
        {
            try {
                var lines = GetLinesInRange (range);
                var newLines = map (lines.AllText, lines.Range, lines.Lines).ToArray ();
                if (!newLines.SequenceEqual (lines.Lines)) {
                    var newLinesText = string.Join ("\n", newLines);
#if __MACOS__
                    TextStorage.Replace (lines.Range, newLinesText);
#elif __IOS__
                    var rangeStart = GetPosition (BeginningOfDocument, lines.Range.Location);
                    var rangeEnd = GetPosition (rangeStart, lines.Range.Length);
                    ReplaceText (GetTextRange (rangeStart, rangeEnd), newLinesText);
#endif
                    if (lines.Lines.Count > 0) {
                        if (lines.Lines.Count == 1) {
                            SelectedRange = new NSRange (lines.Range.Location + newLinesText.Length, 0);
                        }
                        else {
                            SelectedRange = new NSRange (lines.Range.Location, newLinesText.Length);
                        }
                    }
                }
            }
            catch (Exception ex) {
                Debug.WriteLine (ex);
            }
        }

        public void ChangeSelectedLines (Func<string, NSRange, List<string>, IEnumerable<string>> map)
        {
            ChangeLinesInRange (SelectedRange, map);
        }

        public LinesInRange GetLinesInRange (NSRange range)
        {
            var lines = new LinesInRange ();

            var text = TextStorage.Value;
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
                    i++;
                    s = i;
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

            lines.Indent = lines.Lines[0].GetIndent ();
            lines.AllText = text;
            return lines;
        }
    }
}
