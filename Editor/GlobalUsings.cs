#if __IOS__ || __MACCATALYST__ || __MACOS__

global using Foundation;
global using CoreGraphics;
global using ObjCRuntime;

#if __IOS__ || __MACCATALYST__
global using UIKit;
global using INativeTextViewDelegate = UIKit.IUITextViewDelegate;
global using NativeBezierPath = UIKit.UIBezierPath;
global using NativeColor = UIKit.UIColor;
global using NativeFont = UIKit.UIFont;
global using NativeGraphics = UIKit.UIGraphics;
global using NativeLineBreakMode = UIKit.UILineBreakMode;
global using NativeStringAttributes = UIKit.UIStringAttributes;
global using NativeTextAlignment = UIKit.UITextAlignment;
global using NativeTextView = UIKit.UITextView;
global using NativeView = UIKit.UIView;
#elif __MACOS__
global using AppKit;
global using INativeTextViewDelegate = AppKit.INSTextViewDelegate;
global using NativeBezierPath = AppKit.NSBezierPath;
global using NativeColor = AppKit.NSColor;
global using NativeFont = AppKit.NSFont;
global using NativeGraphics = AppKit.NSGraphics;
global using NativeLineBreakMode = AppKit.NSLineBreakMode;
global using NativeStringAttributes = AppKit.NSStringAttributes;
global using NativeTextAlignment = AppKit.NSTextAlignment;
global using NativeTextView = AppKit.NSTextView;
global using NativeView = AppKit.NSView;
#endif

#endif
