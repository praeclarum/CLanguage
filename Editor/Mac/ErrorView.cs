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

            var la = Theme.ErrorBubbleTextAttributes;

            var bounds = Bounds;
            var p = CGPath.FromRoundedRect (bounds, 16, 16);
            c.AddPath (p);
            Theme.ErrorBubbleBackgroundColor.SetFill ();
            c.FillPath ();

            var mt = message.Text;
            if (string.IsNullOrWhiteSpace (mt))
                return;

            bounds.Inflate (-18, -6);
            mt.DrawInRect (bounds, la);
        }
    }
}
