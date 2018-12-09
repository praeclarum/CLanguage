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
                ForegroundColor = !isDark ? Rgb (101, 121, 140) : Rgb (127, 140, 152),
            }.Dictionary;
            SelectedAttributes = new NativeStringAttributes {
                BackgroundColor = !isDark ? NSColor.SelectedTextBackground : NSColor.SelectedTextBackground.ColorWithAlphaComponent (0.5f),
            }.Dictionary;

            BackgroundColor = NativeColor.TextBackground;
            LineNumberColor = isDark ? Gray (75) : Gray (200);
            LineNumberAttributes = MakeLineNumberAttrs ();
            ColorAttributes = Enumerable.Repeat (defaultAttrs, 16).ToArray ();
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Number] = MakeAttrs (Rgb (38, 42, 215), Rgb (166, 157, 247));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.String] = MakeAttrs (Rgb (197, 0, 11), Rgb (255, 128, 112));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Identifier] = MakeAttrs (Gray (0x33), Gray(0xE0));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Operator] = MakeAttrs (Gray (0x88), Gray (0xAA));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Keyword] = MakeAttrs (Rgb (173, 61, 164), Rgb (255, 122, 178));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Type] = MakeAttrs (Rgb (0, 128, 128), Rgb (0, 192, 164));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Function] = MakeAttrs (Rgb (204, 102, 0), Rgb (204, 102, 0));
        }

        public NSDictionary ErrorAttributes (string message)
        {
            return new NativeStringAttributes {
                BackgroundColor = !isDark ? Rgb (0xFF, 0xCC, 0xCC) : Rgb (0x55, 0x00, 0x00),
                UnderlineColor = !isDark ? Rgb (0xFE, 0x00, 0x0B) : Rgb (0xFF, 0x00, 0x0B),
                UnderlineStyle = (int)NSUnderlineStyle.Thick,
                ToolTip = message,
            }.Dictionary;
        }

        NSDictionary MakeAttrs (NativeColor color, NativeColor darkColor) => new NativeStringAttributes {
            Font = codeFont,
            ForegroundColor = isDark ? darkColor : color,
        }.Dictionary;

        NSDictionary MakeLineNumberAttrs () => new NativeStringAttributes {
            Font = Font (codeFont.FontName, (int)(NativeFont.SystemFontSize * 0.8 + 0.5)),
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
