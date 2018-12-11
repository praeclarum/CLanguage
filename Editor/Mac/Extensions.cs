using System;

#if __IOS__
using UIKit;
using NativeColor = UIKit.UIColor;
using NativeFont = UIKit.UIFont;
using NativeStringAttributes = UIKit.UIStringAttributes;
using NativeTextAlignment = UIKit.UITextAlignment;
#elif __MACOS__
using AppKit;
using NativeColor = AppKit.NSColor;
using NativeFont = AppKit.NSFont;
using NativeStringAttributes = AppKit.NSStringAttributes;
using NativeTextAlignment = AppKit.NSTextAlignment;
#endif

using Foundation;

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
        public static NativeColor Gray (int g) => NativeColor.FromWhiteAlpha (g / ((nfloat)255), 1);
        public static NativeFont Font (string name, int size) => NativeFont.FromName (name, size);
        public static string GetFontName (this NativeFont f) => f.Name;
        public static NSUnderlineStyle ToKit (this NSUnderlineStyle s) => s;
        public static NativeColor ColorWithAlphaComponent (this NativeColor color, nfloat alpha) => color.ColorWithAlpha (alpha);
        public static NativeTextAlignment TextAlignmentRight = NativeTextAlignment.Right;
#else
        public static NativeColor Rgb (int r, int g, int b) => NativeColor.FromRgb (r, g, b);
        public static NativeColor Gray (int g) => NativeColor.FromWhite (g / ((nfloat)255), 1);
        public static NativeFont Font (string name, int size) => NativeFont.FromFontName (name, size);
        public static string GetFontName (this NativeFont f) => f.FontName;
        public static NSUnderlineStyle ToKit (this NSUnderlineStyle s) => (nint)s;
        public static NativeTextAlignment TextAlignmentRight = NativeTextAlignment.Right;
#endif
    }
}
