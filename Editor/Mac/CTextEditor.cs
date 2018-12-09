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
    public partial class CTextEditor : NativeView, INSTextViewDelegate, INSTextStorageDelegate
    {
        readonly EditorTextView textView;

        public string Text {
            get => textView.Value;
            set {
                value = value ?? "";
                var oldText = textView.Value;
                if (oldText == value)
                    return;
                textView.TextStorage.SetString (new NSAttributedString (value ?? "", theme.CommentAttributes));
                BeginInvokeOnMainThread (() => {
                    ColorizeCode (textView.TextStorage);
                    UpdateMargin ();
                    if (oldText.Length == 0 && value.Length > 0) {
                        textView.SelectedRange = new NSRange (0, 0);
                    }
                    NeedsLayout = true;
                });
            }
        }

        public NSRange SelectedRange {
            get => textView.SelectedRange;
            set => textView.SelectedRange = value;
        }

        Theme theme;
        public Theme Theme {
            get => theme;
            set {
                theme = value;
                OnThemeChanged ();
            }
        }

        int lineCount = 1;

        public event EventHandler TextChanged;

        MarginView margin = new MarginView ();
        nfloat marginWidth = (nfloat)44;
        NSLayoutConstraint marginWidthConstraint;

#if __IOS__
#elif __MACOS__
        readonly NSScrollView scroll;
        IDisposable scrolledSubscription;
        IDisposable appearanceObserver;
#endif

        public CTextEditor (NSCoder coder) : base (coder)
        {
            textView = new EditorTextView (Bounds);
            scroll = new NSScrollView (Bounds);
            theme = new Theme (IsDark (EffectiveAppearance));
            Initialize ();
        }

        public CTextEditor (IntPtr handle) : base (handle)
        {
            textView = new EditorTextView (Bounds);
            scroll = new NSScrollView (Bounds);
            theme = new Theme (IsDark (EffectiveAppearance));
            Initialize ();
        }

        public CTextEditor (CGRect frameRect) : base (frameRect)
        {
            textView = new EditorTextView (Bounds);
            scroll = new NSScrollView (Bounds);
            theme = new Theme (IsDark (EffectiveAppearance));
            Initialize ();
        }

        void Initialize ()
        {
            var sframe = Bounds;
            var mframe = sframe;
            mframe.Width = marginWidth;
            sframe.X += marginWidth;
            sframe.Width -= marginWidth;

            //textView.LayoutManager.ReplaceTextStorage (storage);
            textView.LayoutManager.TextStorage.Delegate = this;
            textView.Font = theme.CodeFont;
            textView.TypingAttributes = theme.TypingAttributes;

            textView.VerticallyResizable = true;
            textView.HorizontallyResizable = true;
            textView.AutoresizingMask = NSViewResizingMask.WidthSizable;
            textView.TranslatesAutoresizingMaskIntoConstraints = true;
            textView.AutomaticTextReplacementEnabled = false;
            textView.AutomaticDashSubstitutionEnabled = false;
            textView.AutomaticQuoteSubstitutionEnabled = false;
            textView.AutomaticSpellingCorrectionEnabled = false;
            textView.SmartInsertDeleteEnabled = false;
            textView.TextContainer.ContainerSize = new CGSize (nfloat.MaxValue, nfloat.MaxValue);
            textView.TextContainer.WidthTracksTextView = false;
            textView.AllowsUndo = true;
            textView.SelectedTextAttributes = theme.SelectedAttributes;

            scroll.VerticalScrollElasticity = NSScrollElasticity.Allowed;
            scroll.HorizontalScrollElasticity = NSScrollElasticity.Allowed;
            scroll.HasVerticalScroller = true;
            scroll.HasHorizontalScroller = true;
            scroll.DocumentView = textView;

            TranslatesAutoresizingMaskIntoConstraints = false;
            scroll.TranslatesAutoresizingMaskIntoConstraints = false;
            margin.TranslatesAutoresizingMaskIntoConstraints = false;

            scroll.Frame = sframe;
            margin.Frame = mframe;

            AddSubview (scroll);
            AddSubview (margin);
            AddConstraint (NSLayoutConstraint.Create (margin, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Leading, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Trailing, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Top, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Top, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Bottom, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, margin, NSLayoutAttribute.Leading, 1, 0));
            AddConstraint (marginWidthConstraint = NSLayoutConstraint.Create (margin, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, marginWidth));
            AddConstraint (NSLayoutConstraint.Create (margin, NSLayoutAttribute.Top, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Top, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (margin, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Bottom, 1, 0));

            textView.Delegate = this;

            scroll.ContentView.PostsBoundsChangedNotifications = true;
            scrolledSubscription = NativeView.Notifications.ObserveBoundsChanged (scroll.ContentView, (sender, e) => {
                UpdateMargin ();
            });

            appearanceObserver = this.AddObserver ("effectiveAppearance", NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, change => {
                if (change.NewValue is NSAppearance a) {
                    Theme = new Theme (isDark: IsDark (a));
                }
            });
            OnThemeChanged ();
        }

        static bool IsDark (NSAppearance a) => a.Name.Contains ("dark", StringComparison.InvariantCultureIgnoreCase);

        [Export ("textStorage:didProcessEditing:range:changeInLength:")]
        async void DidProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editedMask, NSRange editedRange, nint delta)
        {
            if (editedMask.HasFlag (NSTextStorageEditActions.Characters)) {
                //
                // Have to yield here because this is called *before* the layout managers are updated.
                // And we need them to be in sync. So we yield and catch the next run loop.
                //
                await Task.Yield ();

                ColorizeCode (textStorage);
                UpdateMargin ();
                TextChanged?.Invoke (this, EventArgs.Empty);
            }
        }

        void UpdateMargin ()
        {
            var lineHeight = textView.LayoutManager.DefaultLineHeightForFont (theme.CodeFont);
            var baseline = textView.LayoutManager.DefaultBaselineOffsetForFont (theme.CodeFont);
            margin.SetLinePositions (lineHeight, baseline, scroll.ContentView.Bounds, lineCount);
        }

        void ColorizeCode (NSTextStorage textStorage)
        {
            var code = textStorage.Value;
            var managers = textStorage.LayoutManagers;
            var lc = 1;
            var li = code.IndexOf ('\n');
            while (li >= 0) {
                lc++;
                li = li + 1 < code.Length ? code.IndexOf ('\n', li + 1) : -1;
            }
            lineCount = lc;
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

        void OnThemeChanged ()
        {
            margin.Theme = theme;
            ColorizeCode (textView.TextStorage);
            textView.SelectedTextAttributes = theme.SelectedAttributes;
            scroll.BackgroundColor = textView.BackgroundColor;
            scroll.DrawsBackground = true;
            SetNeedsDisplayInRect (Bounds);
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
