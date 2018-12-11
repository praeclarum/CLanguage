using System;
using CoreGraphics;

using static CLanguage.Editor.Extensions;

#if __IOS__
using UIKit;
using NativeColor = UIKit.UIColor;
using NativeFont = UIKit.UIFont;
using NativeGraphics = UIKit.UIGraphics;
using NativeStringAttributes = UIKit.UIStringAttributes;
#elif __MACOS__
using AppKit;
using NativeColor = AppKit.NSColor;
using NativeFont = AppKit.NSFont;
using NativeGraphics = AppKit.NSGraphics;
using NativeStringAttributes = AppKit.NSStringAttributes;
#endif

namespace CLanguage.Editor
{
    class ErrorView : DrawingView
    {
        Report.AbstractMessage message = new Report.AbstractMessage ("Info", "");
        public Report.AbstractMessage Message {
            get => message;
            set {
                message = value;
                SetNeedsDisplayInRect (Bounds);
                AlphaValue = string.IsNullOrEmpty (message.Text) ? 0 : 1;
            }
        }

        public ErrorView ()
        {
            BackgroundColor = NativeColor.Clear;
        }

        protected override void DrawDirtyRect (CGRect dirtyRect)
        {
            var c = NativeGraphicsCGContext;

            NativeColor.Clear.Set ();
            NativeGraphics.RectFill (dirtyRect);

            var bounds = Bounds;
            if (bounds.Width < bounds.Height)
                return;

            var p = CGPath.FromRoundedRect (bounds, bounds.Height / 2, bounds.Height / 2);
            c.AddPath (p);
            Theme.ErrorBubbleBackgroundColor.ColorWithAlphaComponent (0.875f).SetFill ();
            c.FillPath ();

            var mt = message.Text;
            if (string.IsNullOrWhiteSpace (mt))
                return;

            if (!message.Location.IsNull)
                mt = $"Line {message.Location.Line:#,0}: {mt}";

            bounds.Inflate (-18, -6);
            mt.DrawInRect (bounds, Theme.ErrorBubbleTextAttributes);
        }
    }
}
