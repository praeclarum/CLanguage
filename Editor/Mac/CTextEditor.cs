using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Foundation;
using CoreGraphics;
using System.Threading.Tasks;

#if __IOS__
using UIKit;
using NativeColor = UIKit.UIColor;
using NativeFont = UIKit.UIFont;
using NativeStringAttributes = UIKit.UIStringAttributes;
#elif __MACOS__
using AppKit;
using NativeTextView = AppKit.NSTextView;
using NativeView = AppKit.NSView;
using NativeColor = AppKit.NSColor;
using NativeFont = AppKit.NSFont;
using NativeStringAttributes = AppKit.NSStringAttributes;
#endif

namespace CLanguage.Editor
{
    [Register ("CTextEditor")]
    public class CTextEditor : NativeView, INSTextStorageDelegate, INSLayoutManagerDelegate
    {
        readonly NativeTextView textView;

        public string Text {
            get => textView.Value;
            set {
                textView.TextStorage.SetString (new NSAttributedString (value ?? "", defaultAttrs));
                BeginInvokeOnMainThread (() => ColorizeCode (textView.TextStorage));
            }
        }

#if __IOS__
#elif __MACOS__
        readonly NSScrollView scrollView;
#endif

        public CTextEditor (NSCoder coder) : base (coder)
        {
            textView = new NativeTextView (Bounds);
            scrollView = new NSScrollView (Bounds);
            Initialize ();
        }

        public CTextEditor (IntPtr handle) : base (handle)
        {
            textView = new NativeTextView (Bounds);
            scrollView = new NSScrollView (Bounds);
            Initialize ();
        }

        public CTextEditor (CGRect frameRect) : base (frameRect)
        {
            textView = new NativeTextView (Bounds);
            scrollView = new NSScrollView (Bounds);
            Initialize ();
        }

        void Initialize ()
        {
            InitializeColors ();

            var bounds = Bounds;

            //textView.LayoutManager.ReplaceTextStorage (storage);
            textView.LayoutManager.TextStorage.Delegate = this;
            textView.Font = CodeFont;
            textView.TypingAttributes = defaultAttrs;

            textView.MinSize = new CGSize (bounds.Width, bounds.Height);
            textView.MaxSize = new CGSize (nfloat.MaxValue, nfloat.MaxValue);
            textView.VerticallyResizable = true;
            textView.AutoresizingMask = NSViewResizingMask.WidthSizable;
            textView.AutomaticTextReplacementEnabled = false;
            textView.AutomaticDashSubstitutionEnabled = false;
            textView.AutomaticQuoteSubstitutionEnabled = false;
            textView.AutomaticSpellingCorrectionEnabled = false;
            textView.HorizontallyResizable = true;
            textView.TextContainer.ContainerSize = new CGSize (nfloat.MaxValue, nfloat.MaxValue);
            textView.TextContainer.WidthTracksTextView = false;

            scrollView.VerticalScrollElasticity = NSScrollElasticity.Allowed;
            scrollView.HorizontalScrollElasticity = NSScrollElasticity.Allowed;
            scrollView.HasVerticalScroller = true;
            scrollView.HasHorizontalScroller = true;
            scrollView.DocumentView = textView;

            TranslatesAutoresizingMaskIntoConstraints = false;
            scrollView.TranslatesAutoresizingMaskIntoConstraints = false;
            scrollView.Frame = Bounds;
            AddSubview (scrollView);
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, scrollView, NSLayoutAttribute.Leading, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, scrollView, NSLayoutAttribute.Trailing, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Top, NSLayoutRelation.Equal, scrollView, NSLayoutAttribute.Top, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, scrollView, NSLayoutAttribute.Bottom, 1, 0));

            textView.TextDidChange += TextView_TextDidChange;
        }

        void InitializeColors ()
        {
            colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Number] = MakeAttrs (Rgb (197, 0, 11), Rgb (255, 211, 32));
            colorAttrs[(int)CLanguage.Syntax.SyntaxColor.String] = MakeAttrs (Rgb (197, 0, 11), Rgb (255, 211, 32));
            colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Comment] = MakeAttrs (Rgb (255, 0, 11), Rgb (255, 0, 0));
            colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Identifier] = MakeAttrs (NativeColor.Black, NativeColor.White);
            colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Keyword] = MakeAttrs (Rgb (52, 120, 184), Rgb (52, 120, 184));
            colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Type] = MakeAttrs (Rgb (0, 128, 128), Rgb (0, 164, 164));
            colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Function] = MakeAttrs (Rgb (204, 102, 0), Rgb (204, 102, 0));
            colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Operator] = MakeAttrs (Rgb (96, 96, 96), Rgb (164, 164, 192));
        }

        static readonly NativeFont CodeFont = Font ("Menlo-Regular", (int)(NativeFont.SystemFontSize + 0.5));

        static readonly NSDictionary defaultAttrs = new NativeStringAttributes {
            Font = CodeFont,
            ForegroundColor = Rgb (128, 128, 128),
        }.Dictionary;

        readonly NSDictionary[] colorAttrs = Enumerable.Repeat (defaultAttrs, 16).ToArray ();

        readonly bool isDark = true;

        NSDictionary MakeAttrs (NativeColor color, NativeColor darkColor) => new NativeStringAttributes {
            Font = CodeFont,
            ForegroundColor = isDark ? darkColor : color,
        }.Dictionary;


        public override bool AcceptsFirstResponder ()
        {
            return true;
        }

        void TextView_TextDidChange (object sender, EventArgs e)
        {
        }

        [Export ("textStorage:didProcessEditing:range:changeInLength:")]
        async void DidProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editedMask, NSRange editedRange, nint delta)
        {
            if (editedMask.HasFlag (NSTextStorageEditActions.Characters)) {
                //
                // Have to yield here because this is called *before* the layout managers.
                // And we need them to be in sync. So we yield and catch the next run loop.
                //
                await Task.Yield ();

                ColorizeCode (textStorage);
            }
        }

        void ColorizeCode (NSTextStorage textStorage)
        {
            var code = textStorage.Value;
            var managers = textStorage.LayoutManagers;
            var spans = CLanguage.CLanguageService.Colorize (code, new MachineInfo ());
            foreach (var lm in managers) {
                lm.RemoveTemporaryAttribute (NSStringAttributeKey.ForegroundColor, new NSRange (0, code.Length));
            }
            foreach (var s in spans) {
                var attrs = colorAttrs[(int)s.Color];
                var range = new NSRange (s.Index, s.Length);
                foreach (var lm in managers) {
                    lm.SetTemporaryAttributes (attrs, range);
                }
            }
        }

        [Export ("layoutManager:shouldUseTemporaryAttributes:forDrawingToScreen:atCharacterIndex:effectiveRange:")]
        NSDictionary ShouldUseTemporaryAttributes (NSLayoutManager layoutManager, NSDictionary temporaryAttributes, bool drawingToScreen, nint charIndex, IntPtr effectiveCharRange)
        {
            if (drawingToScreen)
                return temporaryAttributes;
            return null;
        }

#if __IOS__
        static NativeColor Rgb (int r, int g, int b) => NativeColor.FromRGB (r, g, b);
        static NativeFont Font (string name, int size) => NativeFont.FromName (name, size);
        static readonly NSTextStorageEditActions CharsEdited = NSTextStorageEditActions.Characters;
        static readonly NSTextStorageEditActions AttrsEdited = NSTextStorageEditActions.Attributes;
#else
        static NativeColor Rgb (int r, int g, int b) => NativeColor.FromRgb (r, g, b);
        static NativeFont Font (string name, int size) => NativeFont.FromFontName (name, size);
        static readonly nuint CharsEdited = (nuint)(int)(NSTextStorageEditedFlags.EditedCharacters);
        static readonly nuint AttrsEdited = (nuint)(int)(NSTextStorageEditedFlags.EditedAttributed);
#endif
    }
}
