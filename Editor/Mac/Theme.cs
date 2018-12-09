using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Foundation;

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
using System.Diagnostics;
#endif

namespace CLanguage.Editor
{
    public class Theme
    {
        readonly bool isDark;

        public NativeColor BackgroundColor { get; }
        public NativeColor LineNumberColor { get; }

        static readonly NativeFont codeFont = Font (FindFontFamily (), (int)(NativeFont.SystemFontSize + 0.5));

        public NativeFont CodeFont => codeFont;

        public NSDictionary TypingAttributes => defaultAttrs;
        public NSDictionary CommentAttributes => defaultAttrs;
        public readonly NSDictionary SelectedAttributes;

        public readonly NSDictionary[] ColorAttributes;

        public NSDictionary LineNumberAttributes { get; }

        readonly NSDictionary defaultAttrs;

        public Theme (bool isDark)
        {
            this.isDark = isDark;

            defaultAttrs = new NativeStringAttributes {
                Font = codeFont,
                ForegroundColor = !isDark ? Gray (0xA0) : Gray (0x66),
            }.Dictionary;
            SelectedAttributes = new NativeStringAttributes {
                BackgroundColor = !isDark ? Gray (0xDD) : Gray (0x50),
            }.Dictionary;

            BackgroundColor = NativeColor.TextBackground;
            LineNumberColor = isDark ? Gray (50) : Gray (220);
            LineNumberAttributes = MakeLineNumberAttrs ();
            ColorAttributes = Enumerable.Repeat (defaultAttrs, 16).ToArray ();
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Number] = MakeAttrs (Rgb (197, 0, 11), Rgb (255, 211, 32));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.String] = MakeAttrs (Rgb (197, 0, 11), Rgb (255, 211, 32));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Identifier] = MakeAttrs (Gray (0x33), Gray(0xE0));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Operator] = MakeAttrs (Gray (0x88), Gray (0xAA));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Keyword] = MakeAttrs (Rgb (52, 120, 184), Rgb (72, 144, 204));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Type] = MakeAttrs (Rgb (0, 128, 128), Rgb (0, 164, 164));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Function] = MakeAttrs (Rgb (204, 102, 0), Rgb (204, 102, 0));
        }

        NSDictionary MakeAttrs (NativeColor color, NativeColor darkColor) => new NativeStringAttributes {
            Font = codeFont,
            ForegroundColor = isDark ? darkColor : color,
        }.Dictionary;

        NSDictionary MakeLineNumberAttrs () => new NativeStringAttributes {
            Font = codeFont,
            ForegroundColor = LineNumberColor,
            ParagraphStyle = new NSMutableParagraphStyle {
                Alignment = NSTextAlignment.Right,
            }
        }.Dictionary;

        static string FindFontFamily ()
        {
            var fonts = NSFontManager.SharedFontManager.AvailableFonts;
            if (NSScreen.MainScreen.BackingScaleFactor > 1.1 && fonts.Contains ("FiraCode-Retina")) {
                return "FiraCode-Retina";
            }
            if (fonts.Contains ("FiraCode-Regular")) {
                return "FiraCode-Regular";
            }
            return "Menlo-Regular";
        }

#if __IOS__
        static NativeColor Rgb (int r, int g, int b) => NativeColor.FromRGB (r, g, b);
        static NativeColor Gray (int g) => NativeColor.FromWhite (g / ((nfloat)255), 1);
        static NativeFont Font (string name, int size) => NativeFont.FromName (name, size);
        static readonly NSTextStorageEditActions CharsEdited = NSTextStorageEditActions.Characters;
        static readonly NSTextStorageEditActions AttrsEdited = NSTextStorageEditActions.Attributes;
#else
        static NativeColor Rgb (int r, int g, int b) => NativeColor.FromRgb (r, g, b);
        static NativeColor Gray (int g) => NativeColor.FromWhite (g / ((nfloat)255), 1);
        static NativeFont Font (string name, int size) => NativeFont.FromFontName (name, size);
        static readonly nuint CharsEdited = (nuint)(int)(NSTextStorageEditedFlags.EditedCharacters);
        static readonly nuint AttrsEdited = (nuint)(int)(NSTextStorageEditedFlags.EditedAttributed);
#endif
    }
}
