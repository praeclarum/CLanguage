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
using NativeLineBreakMode = UIKit.UILineBreakMode;
#elif __MACOS__
using AppKit;
using NativeColor = AppKit.NSColor;
using NativeFont = AppKit.NSFont;
using NativeStringAttributes = AppKit.NSStringAttributes;
using NativeLineBreakMode = AppKit.NSLineBreakMode;
#endif

namespace CLanguage.Editor
{
    public class Theme
    {
        public readonly bool IsDark;
        public readonly nfloat LineHeightMultiple;

        public NativeColor ErrorBubbleBackgroundColor { get; }
        public readonly NSDictionary ErrorBubbleTextAttributes;

        public NativeColor BackgroundColor { get; }
        public NativeColor LineNumberColor { get; }

        readonly NativeFont codeFont;

        public NativeFont CodeFont => codeFont;
        public NativeFont LineNumberFont { get; }

        public NSDictionary TypingAttributes => defaultAttrs;
        public NSDictionary CommentAttributes => defaultAttrs;
        public readonly NSDictionary SelectedAttributes;

        public readonly NSDictionary[] ColorAttributes;

        public NSDictionary LineNumberAttributes { get; }
        public double FontScale { get; }

        readonly NSDictionary defaultAttrs;

        readonly NSParagraphStyle defaultParagraph;

        public Theme (bool isDark, double fontScale)
        {
            this.IsDark = isDark;
            FontScale = fontScale;
            this.LineHeightMultiple = 1.2f;
            codeFont = Font (FindFontFamily (), (int)(NativeFont.SystemFontSize * fontScale + 0.5));

            defaultParagraph = new NSMutableParagraphStyle {
                LineHeightMultiple = LineHeightMultiple,
            };
            defaultAttrs = new NativeStringAttributes {
                Font = codeFont,
                ForegroundColor = !isDark ? Rgb (101, 121, 140) : Rgb (127, 140, 152),
                ParagraphStyle = defaultParagraph,
            }.Dictionary;
            SelectedAttributes = new NativeStringAttributes {
#if __MACOS__
                BackgroundColor = !isDark ? NativeColor.SelectedTextBackground : NativeColor.SelectedTextBackground.ColorWithAlphaComponent (0.5f),
#elif __IOS__
                BackgroundColor = Rgb (0, 0x84, 0xD1).ColorWithAlpha (0.5f),
#endif
            }.Dictionary;

            BackgroundColor = !isDark ? Rgb (0xFF, 0xFF, 0xFF) : Rgb (41, 42, 47);
            ErrorBubbleBackgroundColor = !isDark ? Rgb (0xC5, 0, 0xB) : Rgb (0xC5 * 5 / 8, 0, 0xB);
            ErrorBubbleTextAttributes = new NativeStringAttributes {
                ForegroundColor = Gray (255).ColorWithAlphaComponent (0.9375f),
                Font = NativeFont.BoldSystemFontOfSize ((nfloat)(NativeFont.SystemFontSize)),
                ParagraphStyle = new NSMutableParagraphStyle {
#if __MACOS__
                    LineBreakMode = NativeLineBreakMode.ByWordWrapping,
#elif __IOS__
                    LineBreakMode = NativeLineBreakMode.WordWrap,
#endif
                },
            }.Dictionary;
            LineNumberFont = Font (codeFont.GetFontName (), (int)(NativeFont.SystemFontSize * 0.8 * fontScale + 0.5));
            LineNumberColor = !isDark ? Gray (0).ColorWithAlphaComponent (0.25f) : Gray (255).ColorWithAlphaComponent (0.125f);
            LineNumberAttributes = MakeLineNumberAttrs ();
            ColorAttributes = Enumerable.Repeat (defaultAttrs, 16).ToArray ();

            // Xcode Colors
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Number] = MakeAttrs (Rgb (38, 42, 215), Rgb (166, 157, 247));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.String] = MakeAttrs (Rgb (197, 0, 11), Rgb (255, 128, 112));
            ColorAttributes[(int)CLanguage.Syntax.SyntaxColor.Identifier] = MakeAttrs (Gray (0x33), Gray (0xE0));
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

        public Theme WithFontScale (double newFontScale)
        {
            var baseSize = NativeFont.SystemFontSize;
            var oldSize = Math.Round (baseSize * FontScale, 1);
            var newSize = Math.Round (baseSize * newFontScale, 1);
            if (newSize < 5)
                newSize = 5;
            else if (newSize > 50)
                newSize = 50;
            if (newSize == oldSize)
                return this;
            return new Theme (IsDark, newSize / baseSize);
        }

#if __MACOS__
        public NSDictionary<NSString, NSObject> ErrorAttributes (string message, NSDictionary existingAttributes)
        {
            var d = new NativeStringAttributes {
                //BackgroundColor = !isDark ? Rgb (0xFF, 0xCC, 0xCC) : Rgb (0x55, 0x00, 0x00),
                UnderlineColor = !IsDark ? Rgb (0xFE, 0x00, 0x0B) : Rgb (0xFF, 0x00, 0x0B),
                UnderlineStyle = NSUnderlineStyle.Thick.ToKit (),
                ToolTip = message,
            }.Dictionary;
            return new NSDictionary<NSString, NSObject> (d.Keys.Cast<NSString> ().ToArray (), d.Values.ToArray ());
        }

        public NSDictionary<NSString, NSObject> WarningAttributes (string message, NSDictionary existingAttributes)
        {
            var d = new NativeStringAttributes {
                UnderlineColor = NativeColor.SystemYellow,
                UnderlineStyle = NSUnderlineStyle.Thick.ToKit (),
                ToolTip = message,
            }.Dictionary;
            return new NSDictionary<NSString, NSObject> (d.Keys.Cast<NSString> ().ToArray (), d.Values.ToArray ());
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
            ParagraphStyle = defaultParagraph,
        }.Dictionary;

        NSDictionary MakeLineNumberAttrs () => new NativeStringAttributes {
            Font = LineNumberFont,
            ForegroundColor = LineNumberColor,
            ParagraphStyle = new NSMutableParagraphStyle {
                Alignment = TextAlignmentRight,
                LineBreakMode = NativeLineBreakModeClipping,
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
