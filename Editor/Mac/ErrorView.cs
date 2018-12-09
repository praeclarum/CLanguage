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
    class ErrorView : NativeView
    {
        Theme theme = new Theme (isDark: false);
        public Theme Theme {
            get => theme;
            set {
                theme = value;
                SetNeedsDisplayInRect (Bounds);
            }
        }

        Report.AbstractMessage message = new Report.AbstractMessage ("Info", "");
        public Report.AbstractMessage Message {
            get => message;
            set {
                message = value;
                SetNeedsDisplayInRect (Bounds);
                AlphaValue = string.IsNullOrEmpty (message.Text) ? 0 : 1;
            }
        }

        public override bool IsFlipped => true;

        public override void DrawRect (CGRect dirtyRect)
        {
            var c = NSGraphicsContext.CurrentContext.CGContext;

            NSColor.Clear.Set ();
            NSGraphics.RectFill (dirtyRect);

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
