#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Foundation;
using CoreGraphics;

#if __IOS__
using UIKit;
using NativeTextView = UIKit.UITextView;
using INativeTextViewDelegate = UIKit.IUITextViewDelegate;
using NativeView = UIKit.UIView;
using NativeColor = UIKit.UIColor;
using NativeFont = UIKit.UIFont;
using NativeStringAttributes = UIKit.UIStringAttributes;
#elif __MACOS__
using AppKit;
using NativeTextView = AppKit.NSTextView;
using INativeTextViewDelegate = AppKit.INSTextViewDelegate;
using NativeView = AppKit.NSView;
using NativeColor = AppKit.NSColor;
using NativeFont = AppKit.NSFont;
using NativeStringAttributes = AppKit.NSStringAttributes;
#endif

using CLanguage.Compiler;
using CLanguage.Syntax;
using static CLanguage.Editor.Extensions;

namespace CLanguage.Editor
{
    [Register ("CEditor")]
    public class CEditor : NativeView, INSTextStorageDelegate, INativeTextViewDelegate
    {
        readonly EditorTextView textView;
        public NativeTextView TextView => textView;

        readonly ErrorView errorView = new ErrorView () { AlphaValue = 0 };
        nfloat errorHeight = (nfloat)32;
        nfloat errorHMargin = (nfloat)18;
        nfloat errorVMargin = (nfloat)16;
        NSLayoutConstraint? errorBottomConstraint;

        readonly MarginView margin = new MarginView ();
        nfloat marginWidth = (nfloat)36;
        NSLayoutConstraint? marginWidthConstraint;

        public string Text {
            get => textView.TextStorage.Value;
            set {
                var val = value ?? "";
                var oldText = textView.TextStorage.Value;
                if (oldText == val)
                    return;
                textView.TextStorage.SetString (new NSAttributedString (value ?? "", theme.CommentAttributes));
                ColorizeCode (textView.TextStorage);
                if (oldText.Length == 0 && val.Length > 0) {
                    textView.SelectedRange = new NSRange (0, 0);
                }
                NeedsLayout = true;
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
                if (!ReferenceEquals (theme, value)) {
                    theme = value;
                    OnThemeChanged ();
                }
            }
        }

        List<int> lineStarts = new List<int> (1) { 0 };

        public event EventHandler? TextChanged;

#if __IOS__
        public event EventHandler EditingEnded;
        NativeColor EffectiveAppearance => TintColor;
        static bool IsDark (NativeColor a) => true;
        bool NeedsLayout { get => false; set => SetNeedsLayout (); }
        static readonly bool ios11 = UIDevice.CurrentDevice.CheckSystemVersion (11, 0);
        EditorKeyboardAccessory keyboardAccessory;
#elif __MACOS__
        readonly bool Is1010 = false;
        readonly bool Is1011 = false;
        readonly NSScrollView scroll;
        IDisposable? scrolledSubscription;
        IDisposable? appearanceObserver;
        static bool IsDark (NSAppearance? a) =>
            a != null ?
                a.Name.Contains ("dark", StringComparison.OrdinalIgnoreCase) :
                false;
#endif

        public CEditor (NSCoder coder) : base (coder)
        {
            textView = new EditorTextView (Bounds);
#if __MACOS__
            scroll = new NSScrollView (Bounds);
#elif __IOS__
#endif
            theme = new Theme (IsDark (EffectiveAppearance), fontScale: 1);
            Initialize ();
        }

        public CEditor (IntPtr handle) : base (handle)
        {
            textView = new EditorTextView (Bounds);
#if __MACOS__
            scroll = new NSScrollView (Bounds);
#elif __IOS__
#endif
            theme = new Theme (IsDark (EffectiveAppearance), fontScale: 1);
            Initialize ();
        }

        public CEditor (CGRect frameRect) : base (frameRect)
        {
            textView = new EditorTextView (Bounds);
#if __MACOS__
            scroll = new NSScrollView (Bounds);
#elif __IOS__
#endif
            theme = new Theme (IsDark (EffectiveAppearance), fontScale: 1);
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

            textView.Font = theme.CodeFont;
            textView.TypingAttributes = theme.TypingAttributes;

            textView.Delegate = this;
            textView.TextStorage.Delegate = this;

#if __MACOS__
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
            if (Is1011) {
                textView.TextContainer.LineBreakMode = NSLineBreakMode.Clipping;
            }
            textView.AllowsUndo = true;
            textView.SelectedTextAttributes = theme.SelectedAttributes;
            NSUserDefaults.StandardUserDefaults.SetInt (50, "NSInitialToolTipDelay");
            textView.DisplaysLinkToolTips = true;

            scroll.VerticalScrollElasticity = NSScrollElasticity.Allowed;
            scroll.HorizontalScrollElasticity = NSScrollElasticity.Allowed;
            scroll.HasVerticalScroller = true;
            scroll.HasHorizontalScroller = true;
            scroll.DocumentView = textView;
            scroll.BackgroundColor = textView.BackgroundColor;
            scroll.DrawsBackground = true;
            if (Is1010) {
                scroll.AutomaticallyAdjustsContentInsets = true;
            }

            scroll.ContentView.PostsBoundsChangedNotifications = true;
            scrolledSubscription = NativeView.Notifications.ObserveBoundsChanged (scroll.ContentView, (sender, e) => {
                UpdateMargin ();
            });

            appearanceObserver = this.AddObserver ("effectiveAppearance", NSKeyValueObservingOptions.Initial | NSKeyValueObservingOptions.New, change => {
                if (change.NewValue is NSAppearance a) {
                    Theme = new Theme (isDark: IsDark (a), fontScale: Theme.FontScale);
                }
            });
#elif __IOS__
            var scroll = textView;
            textView.AlwaysBounceVertical = true;
            textView.AlwaysBounceHorizontal = false;
            textView.InputAssistantItem.LeadingBarButtonGroups = null;
            textView.InputAssistantItem.TrailingBarButtonGroups = null;
            textView.AutocorrectionType = UITextAutocorrectionType.No;
            textView.AutocapitalizationType = UITextAutocapitalizationType.None;
            textView.AllowsEditingTextAttributes = false;
            textView.KeyboardType = UIKeyboardType.Default;
            keyboardAccessory = new EditorKeyboardAccessory (this);
            textView.InputAccessoryView = keyboardAccessory;
            if (ios11) {
                textView.SmartInsertDeleteType = UITextSmartInsertDeleteType.No;
                textView.SmartDashesType = UITextSmartDashesType.No;
                textView.SmartQuotesType = UITextSmartQuotesType.No;
                errorVMargin = 0; // Safe area insets are used instead
            }
            var initialFontScale = theme.FontScale;
            var pinch = new PinchGesture ((UIPinchGestureRecognizer obj) => {
                if (obj.State == UIGestureRecognizerState.Began) {
                    initialFontScale = theme.FontScale;
                }
                else if (obj.State == UIGestureRecognizerState.Changed) {
                    Theme = theme.WithFontScale (initialFontScale * obj.Scale);
                }
            });
            AddGestureRecognizer (pinch);
#endif

            scroll.Frame = sframe;
            margin.Frame = mframe;
            errorView.Frame = eframe;

            scroll.TranslatesAutoresizingMaskIntoConstraints = false;
            margin.TranslatesAutoresizingMaskIntoConstraints = false;
            errorView.TranslatesAutoresizingMaskIntoConstraints = false;

            AddSubview (scroll);
            AddSubview (margin);
            AddSubview (errorView);

            AddConstraint (NSLayoutConstraint.Create (margin, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Leading, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Top, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Top, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Bottom, 1, 0));

            AddConstraint (marginWidthConstraint = NSLayoutConstraint.Create (margin, NSLayoutAttribute.Width, NSLayoutRelation.Equal, 1, marginWidth));
            AddConstraint (NSLayoutConstraint.Create (margin, NSLayoutAttribute.Top, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Top, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (margin, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Bottom, 1, 0));

            AddConstraint (NSLayoutConstraint.Create (errorView, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, errorHeight));

#if __MACOS__
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Trailing, 1, 0));
            AddConstraint (errorBottomConstraint = NSLayoutConstraint.Create (this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, errorView, NSLayoutAttribute.Bottom, 1, errorVMargin));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, margin, NSLayoutAttribute.Leading, 1, 0));
            AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, errorView, NSLayoutAttribute.CenterX, 1, 0));
#elif __IOS__
            if (ios11) {
                AddConstraint (NSLayoutConstraint.Create (SafeAreaLayoutGuide, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Trailing, 1, 0));
                AddConstraint (NSLayoutConstraint.Create (SafeAreaLayoutGuide, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, margin, NSLayoutAttribute.Leading, 1, 0));
                AddConstraint (errorBottomConstraint = NSLayoutConstraint.Create (SafeAreaLayoutGuide, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, errorView, NSLayoutAttribute.Bottom, 1, errorVMargin));
                AddConstraint (NSLayoutConstraint.Create (SafeAreaLayoutGuide, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, errorView, NSLayoutAttribute.CenterX, 1, 0));
            }
            else {
                AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, scroll, NSLayoutAttribute.Trailing, 1, 0));
                AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, margin, NSLayoutAttribute.Leading, 1, 0));
                AddConstraint (errorBottomConstraint = NSLayoutConstraint.Create (this, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, errorView, NSLayoutAttribute.Bottom, 1, errorVMargin));
                AddConstraint (NSLayoutConstraint.Create (this, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, errorView, NSLayoutAttribute.CenterX, 1, 0));
            }
#endif
            UpdateMarginWidth ();
            OnThemeChanged ();
        }

        [Export ("toggleComment:")]
        public void ToggleComment (NSObject sender)
        {
            textView.ChangeSelectedLines (ToggleCommentMapper);
        }

        IEnumerable<string> ToggleCommentMapper (string code, NSRange range, List<string> lines)
        {
            var line0T = lines[0].TrimStart ();
            var remove = line0T.Length > 1 && line0T[0] == '/' && line0T[1] == '/';
            foreach (var line in lines) {
                var indent = line.GetIndent ();
                var nline = indent;
                var s = indent.Length;
                if (remove) {
                    var hasComment = s + 1 < line.Length && line[s] == '/' && line[s] == '/';
                    if (hasComment) {
                        nline = indent + line.Substring (s + 2);
                    }
                }
                else {
                    nline = indent + "//" + line.Substring (s);
                }
                yield return nline;
            }
        }

        [Export ("indent:")]
        public void Indent (NSObject sender)
        {
            textView.ChangeSelectedLines (IndentMapper);
        }

        IEnumerable<string> IndentMapper (string code, NSRange range, List<string> lines)
        {
            return from line in lines
                   let indent = line.GetIndent ()
                   select indent + "  " + line.Substring (indent.Length);
        }

        [Export ("outdent:")]
        public void Outdent (NSObject sender)
        {
            textView.ChangeSelectedLines (OutdentMapper);
        }

        List<string> OutdentMapper (string code, NSRange range, List<string> lines)
        {
            var r = new List<string> (lines.Count);
            foreach (var line in lines) {
                var indent = line.GetIndent ();
                if (indent.Length == 0) {
                    r.Add (line);
                }
                else if (indent.Length == 1) {
                    r.Add (line.Substring (1));
                }
                else {
                    var newIndent = indent.Substring (0, indent.Length - 2);
                    r.Add (newIndent + line.Substring (indent.Length));
                }
            }
            return r;
        }

        [Export ("increaseFontSize:")]
        public void IncreaseFontSize (NSObject sender)
        {
            Theme = Theme.WithFontScale (Theme.FontScale * 1.25);
        }

        [Export ("decreaseFontSize:")]
        public void DecreaseFontSize (NSObject sender)
        {
            Theme = Theme.WithFontScale (Theme.FontScale / 1.25);
        }

        [Export ("restoreFontSize:")]
        public void RestoreFontSize (NSObject sender)
        {
            Theme = Theme.WithFontScale (1);
        }

        [Export ("zoomImageToActualSize:")]
        public void ZoomImageToActualSize (NSObject sender)
        {
            RestoreFontSize (sender);
        }

        [Export ("zoomIn:")]
        public void ZoomIn (NSObject sender)
        {
            IncreaseFontSize (sender);
        }

        [Export ("zoomOut:")]
        public void ZoomOut (NSObject sender)
        {
            DecreaseFontSize (sender);
        }

        //[Export ("textStorage:didProcessEditing:range:changeInLength:")]
        //async void DidProcessEditing (NSTextStorage textStorage, NSTextStorageEditActions editedMask, NSRange editedRange, nint delta)
        //{
        //    if (Is1011) {
        //        //
        //        // Have to yield here because this is called *before* the layout managers are updated.
        //        // And we need them to be in sync. So we yield and catch the next run loop.
        //        //
        //        await Task.Yield ();

        //        if (editedMask.HasFlag (NSTextStorageEditActions.Characters)) {
        //            ColorizeCode (textStorage);
        //            Console.WriteLine ("Text changed");
        //            TextChanged?.Invoke (this, EventArgs.Empty);
        //        }
        //        else if (editedMask.HasFlag (NSTextStorageEditActions.Attributes)) {
        //            UpdateMargin ();
        //        }
        //    }
        //}

        [Export ("textStorageDidProcessEditing:")]
        public async void DidProcessEditingDep (NSNotification aNotification)
        {
            //
            // Have to yield here because this is called *before* the layout managers are updated.
            // And we need them to be in sync. So we yield and catch the next run loop.
            //
            await Task.Yield ();

            UpdateMargin ();
            ColorizeCode (TextView.TextStorage);
            //Console.WriteLine ("Text changed old");
            TextChanged?.Invoke (this, EventArgs.Empty);
        }

#if __IOS__
        [Export ("scrollViewDidScroll:")]
        void Scrolled (UIScrollView scrollView)
        {
            UpdateMargin ();
        }
        [Export ("textViewDidEndEditing:")]
        void TextViewDidEndEditing (NativeTextView view)
        {
            EditingEnded?.Invoke (this, EventArgs.Empty);
        }
        class PinchGesture : UIPinchGestureRecognizer
        {
            public PinchGesture (Action<UIPinchGestureRecognizer> action) : base (action)
            {
            }

            public override bool CanPreventGestureRecognizer (UIGestureRecognizer preventedGestureRecognizer)
            {
                //Console.WriteLine ("? " + preventedGestureRecognizer);
                return false;
            }
        }
#elif __MACOS__
        public override void MagnifyWithEvent (NSEvent theEvent)
        {
            base.MagnifyWithEvent (theEvent);

            var fs = theme.FontScale;
            var nfs = fs * (1 + theEvent.Magnification);
            nfs = Math.Max (0.1, Math.Min (10, nfs));
            Theme = theme.WithFontScale (nfs);
        }
#endif

        void UpdateMargin ()
        {
            var layoutManager = textView.LayoutManager;
            var textContainer = textView.TextContainer;
#if __MACOS__
            var bounds = scroll.ContentView.Bounds;
            var lfpad = textView.TextContainerInset.Height;
#elif __IOS__
            var bounds = textView.Bounds;
            var lfpad = textView.TextContainerInset.Top;
#endif
            var visibleGlyphs = layoutManager.GlyphRangeForBoundingRect (bounds, textContainer);
            var visibleChars = layoutManager.CharacterRangeForGlyphRange (visibleGlyphs);
            var lines = textView.GetLinesInRange (visibleChars);
            var index = lines.Range.Location;
            var lineBounds = new List<CGRect> (lines.Lines.Count);
            for (var i = 0; i < lines.Lines.Count && index <= lines.AllText.Length; i++) {
                var line = lines.Lines[i];
                var cr = new NSRange (index, line.Length);
                var gr = layoutManager.GlyphRangeForCharacterRange (cr);
                var b = layoutManager.BoundingRectForGlyphRange (gr, textContainer);
                b.Y += lfpad;
                lineBounds.Add (b);
                index += line.Length + 1;
            }

            margin.SetLinePositions ((int)lines.Range.Location, lineBounds, bounds, lineStarts);

            UpdateMarginWidth ();
        }

        void UpdateMarginWidth ()
        {
            if (marginWidthConstraint != null) {
                var lineCount = Math.Max (10, lineStarts.Count);
                var size = lineCount.ToString ().StringSize (theme.LineNumberAttributes);
                marginWidthConstraint.Constant = (nfloat)(size.Width + 10 * theme.FontScale);
            }
        }

        static readonly char[] newlineChars = { '\n', (char)8232 };

        void ColorizeCode (NSTextStorage textStorage)
        {
            //await Task.Delay (1000);

            var code = textStorage.Value;
            var managers = textStorage.LayoutManagers;

            //
            // Count the lines
            //
            var lineStarts = new List<int> (code.Length / 20) { 0 };
            var lc = 1;
            var li = code.IndexOfAny (newlineChars);
            while (li >= 0) {
                if (li + 1 <= code.Length)
                    lineStarts.Add (li + 1);
                lc++;
                li = li + 1 < code.Length ? code.IndexOfAny (newlineChars, li + 1) : -1;
            }
            Debug.Assert (lc == lineStarts.Count, $"Line count mismatch: {lc} != {lineStarts.Count}");
            this.lineStarts = lineStarts;
            //await Task.Delay (1000);
            UpdateMargin ();
            //await Task.Delay (1000);

#if __MACOS__
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
                    var attrs = m.MessageType == "Error" ? theme.ErrorAttributes (m.Text, null) : theme.WarningAttributes (m.Text, null);
                    foreach (var lm in managers) {
                        lm.AddTemporaryAttributes (attrs, range);
                    }
                }
            }
#elif __IOS__
            var ts = (EditorTextStorage)textView.TextStorage;
            ts.Options = options;
            ts.Theme = Theme;
            var printer = ts.LastPrinter;
#endif

            //
            // Inform the error view
            //
            errorView.Message = printer.Messages.FirstOrDefault (x => x.MessageType == "Error") ?? new Report.AbstractMessage ("Info", "");
        }

        void OnThemeChanged ()
        {
            margin.Theme = theme;
            errorView.Theme = theme;
            textView.BackgroundColor = theme.BackgroundColor;
            textView.Font = theme.CodeFont;
            ColorizeCode (textView.TextStorage);
#if __MACOS__
            textView.SelectedTextAttributes = theme.SelectedAttributes;
            textView.TypingAttributes = theme.TypingAttributes;
            scroll.DrawsBackground = true;
            scroll.BackgroundColor = textView.BackgroundColor;
#elif __IOS__
            ((EditorTextStorage)textView.TextStorage).Theme = theme;
            BackgroundColor = theme.BackgroundColor;
            textView.KeyboardAppearance = theme.IsDark ? UIKeyboardAppearance.Dark : UIKeyboardAppearance.Light;
            keyboardAccessory.Theme = theme;
#endif
            SetNeedsDisplayInRect (Bounds);
        }
    }
}
