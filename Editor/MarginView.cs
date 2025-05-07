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
            var drawRect = Bounds;
            drawRect.Intersect (dirtyRect);
            NativeGraphics.RectFill (drawRect);

            var la = Theme.LineNumberAttributes;

            var bottom = Bounds.Bottom;
            var hpad = 4 * Theme.FontScale;
            var width = Bounds.Width - hpad;
            var x = 0;
            var yoff = -textBounds.Y;

            var codeLineHeight = "0".StringSize (Theme.CommentAttributes).Height;
            var numLineHeight = "0".StringSize (la).Height;

#if __MACOS__
            var tyoff = Theme.LineHeightMultiple * Theme.CodeFont.PointSize - numLineHeight * 0.8f;
#elif __IOS__
            var tyoff = codeLineHeight - numLineHeight;
#endif

            var rline = 0;
            //NativeColor.White.Set ();
            for (var line = 0; line < lineStarts.Count; line++) {
                var lineStartIndex = lineStarts[line];

                if (lineStartIndex >= startIndex) {
                    if (rline < lineBounds.Count) {
                        var frame = new CGRect (x, lineBounds[rline].Y + yoff, width, lineBounds[rline].Height);
                        //c.StrokeRect (frame);
                        frame.Y = (nfloat)Math.Floor (frame.Y + tyoff);
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
