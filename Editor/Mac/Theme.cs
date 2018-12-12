using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Foundation;
using static CLanguage.Editor.Extensions;

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
    public class Theme
    {
        public readonly bool IsDark;

        public NativeColor ErrorBubbleBackgroundColor { get; }
        public readonly NSDictionary ErrorBubbleTextAttributes;

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
            this.IsDark = isDark;

            defaultAttrs = new NativeStringAttributes {
                Font = codeFont,
                ForegroundColor = !isDark ? Rgb (101, 121, 140) : Rgb (127, 140, 152),
            }.Dictionary;
            SelectedAttributes = new NativeStringAttributes {
#if __MACOS__
                BackgroundColor = !isDark ? NativeColor.SelectedTextBackground : NativeColor.SelectedTextBackground.ColorWithAlphaComponent (0.5f),
#elif __IOS__
                BackgroundColor = Rgb (0, 0x84, 0xD1).ColorWithAlpha (0.5f),
#endif
            }.Dictionary;

            BackgroundColor = !isDark ? Rgb (0xFF, 0xFF, 0xFF) : Rgb (41, 42, 47);
            ErrorBubbleBackgroundColor = !isDark ? Rgb (0xC5, 0, 0xB) : Rgb (0xC5 * 2 /4, 0, 0xB);
            ErrorBubbleTextAttributes = new NativeStringAttributes {
                ForegroundColor = Gray (255).ColorWithAlphaComponent (0.9375f),
                Font = NativeFont.BoldSystemFontOfSize (NativeFont.SystemFontSize),
            }.Dictionary;
            LineNumberColor = isDark ? Gray (85) : Gray (255).ColorWithAlpha (0.75f);
            LineNumberAttributes = MakeLineNumberAttrs ();
            ColorAttributes = Enumerable.Repeat (defaultAttrs, 16).ToArray ();

            // Xcode Colors
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Number] = MakeAttrs (Rgb (38, 42, 215), Rgb (166, 157, 247));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.String] = MakeAttrs (Rgb (197, 0, 11), Rgb (255, 128, 112));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Identifier] = MakeAttrs (Gray (0x33), Gray(0xE0));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Operator] = MakeAttrs (Gray (0x88), Gray (0xAA));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Keyword] = MakeAttrs (Rgb (173, 61, 164), Rgb (255, 122, 178));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Type] = MakeAttrs (Rgb (0, 128, 128), Rgb (0, 192, 164));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Function] = MakeAttrs (Rgb (204, 102, 0), Rgb (255, 161, 79));

            // Arduino Colors
            //ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Number] = MakeAttrs (Rgb (197, 0, 11), Rgb (255, 211, 32));
            //ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.String] = MakeAttrs (Rgb (197, 0, 11), Rgb (255, 211, 32));
            //ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Identifier] = MakeAttrs (NativeColor.Black, NativeColor.White);
            //ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Keyword] = MakeAttrs (Rgb (52, 120, 184), Rgb (52, 120, 184));
            //ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Type] = MakeAttrs (Rgb (0, 128, 128), Rgb (0, 164, 164));
            //ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Function] = MakeAttrs (Rgb (204, 102, 0), Rgb (204, 102, 0));
            //ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Operator] = MakeAttrs (Rgb (96, 96, 96), Rgb (164, 164, 192));
        }

#if __MACOS__
        public NSDictionary ErrorAttributes (string message, NSDictionary existingAttributes)
        {
            return new NativeStringAttributes {
                //BackgroundColor = !isDark ? Rgb (0xFF, 0xCC, 0xCC) : Rgb (0x55, 0x00, 0x00),
                UnderlineColor = !IsDark ? Rgb (0xFE, 0x00, 0x0B) : Rgb (0xFF, 0x00, 0x0B),
                UnderlineStyle = NSUnderlineStyle.Thick.ToKit (),
                ToolTip = message,
            }.Dictionary;
        }

        public NSDictionary WarningAttributes (string message, NSDictionary existingAttributes)
        {
            return new NativeStringAttributes {
                UnderlineColor = NativeColor.SystemYellowColor,// !isDark ? Rgb (120, 73, 42) : Rgb (0xFF, 0xD3, 0x20),
                UnderlineStyle = NSUnderlineStyle.Thick.ToKit (),
                ToolTip = message,
            }.Dictionary;
        }
#elif __IOS__
        public NSDictionary ErrorAttributes (string message, NSDictionary existingAttributes)
        {
            return new NativeStringAttributes ((NSDictionary)existingAttributes.MutableCopy ()) {
                //BackgroundColor = !isDark ? Rgb (0xFF, 0xCC, 0xCC) : Rgb (0x55, 0x00, 0x00),
                UnderlineColor = !IsDark ? Rgb (0xFE, 0x00, 0x0B) : Rgb (0xFF, 0x00, 0x0B),
                UnderlineStyle = NSUnderlineStyle.Thick.ToKit (),
            }.Dictionary;
        }

        public NSDictionary WarningAttributes (string message, NSDictionary existingAttributes)
        {
            return new NativeStringAttributes ((NSDictionary)existingAttributes.MutableCopy ()) {
                UnderlineColor = !IsDark ? Rgb (0xFE, 0xD3, 0x20) : Rgb (0xFF, 0xD3, 0x20),
                UnderlineStyle = NSUnderlineStyle.Thick.ToKit (),
            }.Dictionary;
        }
#endif

        NSDictionary MakeAttrs (NativeColor color, NativeColor darkColor) => new NativeStringAttributes {
            Font = codeFont,
            ForegroundColor = IsDark ? darkColor : color,
        }.Dictionary;

        NSDictionary MakeLineNumberAttrs () => new NativeStringAttributes {
            Font = Font (codeFont.GetFontName (), (int)(NativeFont.SystemFontSize * 0.8 + 0.5)),
            ForegroundColor = LineNumberColor,
            ParagraphStyle = new NSMutableParagraphStyle {
                Alignment = TextAlignmentRight,
            }
        }.Dictionary;

        static string FindFontFamily ()
        {
#if __MACOS__
            var fonts = NSFontManager.SharedFontManager.AvailableFonts;
            if (NSScreen.MainScreen.BackingScaleFactor > 1.1 && fonts.Contains ("FiraCode-Retina")) {
                return "FiraCode-Retina";
            }
            if (fonts.Contains ("FiraCode-Regular")) {
                return "FiraCode-Regular";
            }
#endif
            return "Menlo-Regular";
        }
    }
}
