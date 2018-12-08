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

#if __IOS__
#elif __MACOS__
#endif

        public CTextEditor (NSCoder coder) : base (coder)
        {
            Initialize ();
        }

        public CTextEditor (IntPtr handle) : base (handle)
        {
            Initialize ();
        }

        public CTextEditor (CGRect frameRect) : base (frameRect)
        {
            Initialize ();
        }

        void Initialize ()
        {

        }
    }
}
