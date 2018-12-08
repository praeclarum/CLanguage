using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Foundation;
using CoreGraphics;

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
    public class CTextEditor : NativeView
    {
        readonly NativeTextView textView;

        readonly CTextStorage storage = new CTextStorage (new MachineInfo (), isDark: true);

        public string Text {
            get => textView.Value;
            set => textView.Value = value ?? "";
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
            var bounds = Bounds;

            textView.LayoutManager.ReplaceTextStorage (storage);

            textView.MinSize = new CGSize (44, 44);
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

        public override bool AcceptsFirstResponder ()
        {
            return true;
        }

        void TextView_TextDidChange (object sender, EventArgs e)
        {
        }
    }
}
