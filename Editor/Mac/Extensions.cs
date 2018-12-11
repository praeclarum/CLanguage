using System;

#if __IOS__
using UIKit;
using NativeColor = UIKit.UIColor;
using NativeFont = UIKit.UIFont;
using NativeStringAttributes = UIKit.UIStringAttributes;
#elif __MACOS__
using AppKit;
using NativeColor = AppKit.NSColor;
using NativeFont = AppKit.NSFont;
using NativeStringAttributes = AppKit.NSStringAttributes;
#endif

namespace CLanguage.Editor
{
    static class Extensions
    {
        public static string GetIndent (this string line)
        {
            var e = 0;
            while (e < line.Length && char.IsWhiteSpace (line[e]))
                e++;
            if (e == 0)
                return string.Empty;
            return line.Substring (0, e);
        }

#if __IOS__
        public static NativeColor Rgb (int r, int g, int b) => NativeColor.FromRGB (r, g, b);
        public static NativeColor Gray (int g) => NativeColor.FromWhite (g / ((nfloat)255), 1);
        public static NativeFont Font (string name, int size) => NativeFont.FromName (name, size);
#else
        public static NativeColor Rgb (int r, int g, int b) => NativeColor.FromRgb (r, g, b);
        public static NativeColor Gray (int g) => NativeColor.FromWhite (g / ((nfloat)255), 1);
        public static NativeFont Font (string name, int size) => NativeFont.FromFontName (name, size);
#endif
    }
}
