using System;
using CoreGraphics;

using static CLanguage.Editor.Extensions;
using System.Collections.Generic;

#if __IOS__
using UIKit;
using NativeColor = UIKit.UIColor;
using NativeFont = UIKit.UIFont;
using NativeGraphics = UIKit.UIGraphics;
using NativeStringAttributes = UIKit.UIStringAttributes;
using NativeTextAlignment = UIKit.UITextAlignment;
#elif __MACOS__
using AppKit;
using NativeColor = AppKit.NSColor;
using NativeFont = AppKit.NSFont;
using NativeGraphics = AppKit.NSGraphics;
using NativeStringAttributes = AppKit.NSStringAttributes;
using NativeTextAlignment = AppKit.NSTextAlignment;
#endif

namespace CLanguage.Editor
{
    class MarginView : DrawingView
    {
        int startIndex = 0;
        List<CGRect> lineBounds = new List<CGRect> (1) { new CGRect (0, 0, 40, 16) };
        List<int> lineStarts = new List<int> (1) { 0 };
        CGRect textBounds = new CGRect (0, 0, 100, 1000);

        protected override void DrawDirtyRect (CGRect dirtyRect)
        {
            Theme.BackgroundColor.Set ();
            NativeGraphics.RectFill (dirtyRect);

            var la = Theme.LineNumberAttributes;
            var fontHeight = "123".StringSize (la).Height;

            var bottom = Bounds.Bottom;
            var width = Bounds.Width;
            var hpad = (nfloat)4;

            var rline = 0;
            for (var line = 0; line < lineStarts.Count; line++) {
                var lineStartIndex = lineStarts[line];

                if (lineStartIndex >= startIndex) {
                    if (rline < lineBounds.Count) {
                        var frame = new CGRect (0, lineBounds[rline].Y - textBounds.Y + 11, width, 16);
                        (line + 1).ToString ().DrawInRect (frame, la);
                        rline++;
                    }
                    else {
                        break;
                    }
                }
            }
        }

        public void SetLinePositions (int startIndex, List<CGRect> lineBounds, CGRect bounds, List<int> lineStarts)
        {
            this.startIndex = startIndex;
            this.lineBounds = lineBounds;
            this.lineStarts = lineStarts;
            this.textBounds = bounds;
            SetNeedsDisplayInRect (Bounds);
        }
    }
}
