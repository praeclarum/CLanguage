using System;
using CoreGraphics;

using static CLanguage.Editor.Extensions;

#if __IOS__
using UIKit;
using NativeColor = UIKit.UIColor;
using NativeFont = UIKit.UIFont;
using NativeStringAttributes = UIKit.UIStringAttributes;
using NativeView = UIKit.UIView;
#elif __MACOS__
using AppKit;
using NativeView = AppKit.NSView;
using NativeColor = AppKit.NSColor;
#endif

namespace CLanguage.Editor
{
    abstract class DrawingView : NativeView
    {
        public DrawingView ()
        {
#if __IOS__
            Opaque = false;
#elif __MACOS__
            WantsLayer = true;
#endif
        }

        Theme theme = new Theme (isDark: false, fontScale: 1);
        public Theme Theme {
            get => theme;
            set {
                if (!ReferenceEquals (theme, value)) {
                    theme = value;
                    SetNeedsDisplayInRect (Bounds);
                }
            }
        }

#if __MACOS__
        NativeColor backgroundColor = NativeColor.Black;
        public NativeColor BackgroundColor {
            get => backgroundColor;
            set {
                backgroundColor = value;
                SetNeedsDisplayInRect (Bounds);
            }
        }
        public override bool IsFlipped => true;
        public override void DrawRect (CGRect dirtyRect)
        {
            DrawDirtyRect (dirtyRect);
        }
#elif __IOS__
        public nfloat AlphaValue { get => Alpha; set => Alpha = value; }
        protected DrawingView ()
        {
            Opaque = false;
            ContentMode = UIViewContentMode.Redraw;
            UserInteractionEnabled = false;
        }
        public override void Draw (CGRect rect)
        {
            DrawDirtyRect (rect);
        }
#endif

        protected abstract void DrawDirtyRect (CGRect dirtyRect);
    }
}
