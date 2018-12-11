using System;
using CoreGraphics;

using static CLanguage.Editor.Extensions;

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
        nfloat lineHeight = 15;
        CGRect textBounds = new CGRect (0, 0, 100, 1000);
        int lineCount = 1;

        protected override void DrawDirtyRect (CGRect dirtyRect)
        {
            Theme.BackgroundColor.Set ();
            NativeGraphics.RectFill (dirtyRect);

            var la = Theme.LineNumberAttributes;
            var fontHeight = "123".StringSize (la).Height;

            var y = -textBounds.Y + (lineHeight - fontHeight);
            var bottom = Bounds.Bottom;
            var width = Bounds.Width;
            var hpad = (nfloat)4;
            var frame = new CGRect (0, y, width - hpad, lineHeight);

            for (var line = 1; line <= lineCount; line++) {
                if (frame.Bottom > 0) {
                    line.ToString ().DrawInRect (frame, la);
                }
                frame.Y += lineHeight;
                if (frame.Y > bottom)
                    break;
            }
        }

        public void SetLinePositions (nfloat lineHeight, CGRect bounds, int lineCount)
        {
            this.lineHeight = lineHeight;
            this.textBounds = bounds;
            this.lineCount = lineCount;
            SetNeedsDisplayInRect (Bounds);
        }
    }
}
