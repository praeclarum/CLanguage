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
using CLanguage.Compiler;
using CLanguage.Syntax;
#endif

namespace CLanguage.Editor
{
    [Register ("CTextEditor")]
    public partial class CTextEditor : NativeView, INSTextViewDelegate, INSTextStorageDelegate
    {
        readonly EditorTextView textView;

        readonly ErrorView errorView = new ErrorView () { AlphaValue = 0 };
        nfloat errorHeight = (nfloat)32;
        nfloat errorHMargin = (nfloat)16;
        nfloat errorVMargin = (nfloat)8;

        readonly MarginView margin = new MarginView ();
        nfloat marginWidth = (nfloat)36;
        NSLayoutConstraint marginWidthConstraint;

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

        CompilerOptions options = new CompilerOptions (new MachineInfo (), new Report (), Enumerable.Empty<Document> ());
        public CompilerOptions Options {
            get => options;
            set {
                options = value;
                ColorizeCode (textView.TextStorage);
            }
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
            var eframe = sframe;
            mframe.Width = marginWidth;
            sframe.X += marginWidth;
            sframe.Width -= marginWidth;
            eframe.Height = errorHeight;

            //textView.LayoutManager.ReplaceTextStorage (storage);
            textView.LayoutManager.TextStorage.Delegate = this;
            textView.Font = theme.CodeFont;
            textView.TypingAttributes = theme.TypingAttributes;

            textView.MaxSize = new CGSize (nfloat.MaxValue, nfloat.MaxValue);
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
            textView.TextContainer.LineBreakMode = NSLineBreakMode.Clipping;
            textView.AllowsUndo = true;
            textView.SelectedTextAttributes = theme.SelectedAttributes;

            scroll.VerticalScrollElasticity = NSScrollElasticity.Allowed;
            scroll.HorizontalScrollElasticity = NSScrollElasticity.Allowed;
            scroll.HasVerticalScroller = true;
            scroll.HasHorizontalScroller = true;
            scroll.DocumentView = textView;
            scroll.BackgroundColor = textView.BackgroundColor;
            scroll.DrawsBackground = true;

            TranslatesAutoresizingMaskIntoConstraints = false;
            scroll.TranslatesAutoresizingMaskIntoConstraints = false;
            margin.TranslatesAutoresizingMaskIntoConstraints = false;
            errorView.TranslatesAutoresizingMaskIntoConstraints = false;

            scroll.Frame = sframe;
            margin.Frame = mframe;
            errorView.Frame = eframe;

            AddSubview (scroll);
            AddSubview (margin);
            AddSubview (errorView);

            AddConstraint (NSLayoutConstraint.Create (margin, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Leading, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Trailing, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Top, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Top, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Bottom, 1, 0));

            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, margin, NSLayoutAttribute.Leading, 1, 0));
            AddConstraint (marginWidthConstraint = NSLayoutConstraint.Create (margin, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, marginWidth));
            AddConstraint (NSLayoutConstraint.Create (margin, NSLayoutAttribute.Top, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Top, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (margin, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Bottom, 1, 0));

            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, errorView, NSLayoutAttribute.Leading, 1, -errorHMargin));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, errorView, NSLayoutAttribute.Trailing, 1, errorHMargin));
            AddConstraint (NSLayoutConstraint.Create (errorView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, errorHeight));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, errorView, NSLayoutAttribute.Bottom, 1, errorVMargin));

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

            //
            // Count the lines
            //
            var lc = 1;
            var li = code.IndexOf ('\n');
            while (li >= 0) {
                lc++;
                li = li + 1 < code.Length ? code.IndexOf ('\n', li + 1) : -1;
            }
            lineCount = lc;

            //
            // Use the language service to determine colors and errors
            //
            var printer = new EditorPrinter ();
            var spans = CLanguage.CLanguageService.Colorize (code, options.MachineInfo, printer);

            //
            // Color the text
            //
            foreach (var lm in managers) {
                lm.RemoveTemporaryAttribute (NSStringAttributeKey.ForegroundColor, new NSRange (0, code.Length));
                lm.RemoveTemporaryAttribute (NSStringAttributeKey.BackgroundColor, new NSRange (0, code.Length));
                lm.RemoveTemporaryAttribute (NSStringAttributeKey.ToolTip, new NSRange (0, code.Length));
                lm.RemoveTemporaryAttribute (NSStringAttributeKey.UnderlineStyle, new NSRange (0, code.Length));
            }
            var colorAttrs = theme.ColorAttributes;
            foreach (var s in spans) {
                var attrs = colorAttrs[(int)s.Color];
                var range = new NSRange (s.Index, s.Length);
                foreach (var lm in managers) {
                    lm.SetTemporaryAttributes (attrs, range);
                }
            }

            //
            // Mark errors
            //
            foreach (var m in printer.Messages) {
                if (m.Location.IsNull || m.EndLocation.IsNull)
                    continue;
                if (m.Location.Document.Path != CLanguageService.DefaultCodePath)
                    continue;
                var range = new NSRange (m.Location.Index, m.EndLocation.Index - m.Location.Index);
                if (range.Location >= 0 && range.Length > 0 && range.Location < code.Length && range.Location + range.Length <= code.Length) {
                    var attrs = theme.ErrorAttributes (m.Text);
                    foreach (var lm in managers) {
                        lm.AddTemporaryAttributes (attrs, range);
                    }
                }
            }

            //
            // Inform the error view
            //
            errorView.Message = printer.Messages.FirstOrDefault (x => x.MessageType == "Error") ?? new Report.AbstractMessage ("Info", "");
        }

        void OnThemeChanged ()
        {
            margin.Theme = theme;
            errorView.Theme = theme;
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
