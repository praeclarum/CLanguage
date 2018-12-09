using System;
using CoreGraphics;

#if __IOS__
using UIKit;
using NativeColor = UIKit.UIColor;
using NativeFont = UIKit.UIFont;
using NativeStringAttributes = UIKit.UIStringAttributes;
#elif __MACOS__
using AppKit;
using NativeView = AppKit.NSView;
#endif

namespace CLanguage.Editor
{
    class MarginView : NativeView
    {
        Theme theme = new Theme (isDark: false);
        public Theme Theme {
            get => theme;
            set {
                theme = value;
                SetNeedsDisplayInRect (Bounds);
            }
        }

        nfloat lineHeight = 15;
        nfloat baseline = 12;
        CGRect textBounds = new CGRect (0, 0, 100, 1000);
        int lineCount = 1;

        public override bool IsFlipped => true;

        public override void DrawRect (CGRect dirtyRect)
        {
            Theme.BackgroundColor.Set ();
            NSGraphics.RectFill (dirtyRect);

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

        public void SetLinePositions (nfloat lineHeight, nfloat baseline, CGRect bounds, int lineCount)
        {
            this.lineHeight = lineHeight;
            this.baseline = baseline;
            this.textBounds = bounds;
            this.lineCount = lineCount;
            SetNeedsDisplayInRect (Bounds);
        }
    }
}
