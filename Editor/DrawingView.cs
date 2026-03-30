using System;

using static CLanguage.Editor.Extensions;

#if __IOS__ || __MACCATALYST__ || __MACOS__

namespace CLanguage.Editor
{
    abstract class DrawingView : NativeView
    {
        protected DrawingView ()
        {
#if __IOS__ || __MACCATALYST__
            Opaque = false;
            ContentMode = UIViewContentMode.Redraw;
            UserInteractionEnabled = false;
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
            var context = NSGraphicsContext.CurrentContext?.CGContext;
            context?.SaveState ();
            context?.ClipToRect (Bounds);
            DrawDirtyRect (dirtyRect);
            context?.RestoreState ();
        }
#elif __IOS__ || __MACCATALYST__
        public nfloat AlphaValue { get => Alpha; set => Alpha = value; }
        public override void Draw (CGRect rect)
        {
            DrawDirtyRect (rect);
        }
#endif

        protected abstract void DrawDirtyRect (CGRect dirtyRect);
    }
}

#endif
