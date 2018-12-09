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
                textView.TextStorage.SetString (new NSAttributedString (value ?? "", theme.CommentAttributes));
                BeginInvokeOnMainThread (() => ColorizeCode (textView.TextStorage));
            }
        }

        public event EventHandler TextChanged;

        CTheme theme = new CTheme ();

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
            var bounds = Bounds;

            //textView.LayoutManager.ReplaceTextStorage (storage);
            textView.LayoutManager.TextStorage.Delegate = this;
            textView.Font = theme.CodeFont;
            textView.TypingAttributes = theme.TypingAttributes;

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
            textView.AllowsUndo = true;

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

        void TextView_TextDidChange (object sender, EventArgs e)
        {
            TextChanged?.Invoke (this, e);
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
            var colorAttrs = theme.ColorAttributes;
            foreach (var s in spans) {
                var attrs = colorAttrs[(int)s.Color];
                var range = new NSRange (s.Index, s.Length);
                foreach (var lm in managers) {
                    lm.SetTemporaryAttributes (attrs, range);
                }
            }
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
