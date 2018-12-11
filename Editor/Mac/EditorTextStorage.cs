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
	class EditorTextStorage : NSTextStorage
	{
		static readonly NativeFont CodeFont = Font ("Menlo-Regular", (int)(NativeFont.SystemFontSize + 0.5));

		static readonly NSDictionary defaultAttrs = new NativeStringAttributes {
			Font = CodeFont,
			ForegroundColor = Rgb (128, 128, 128),
		}.Dictionary;

		readonly bool isDark = false;
        readonly NSMutableAttributedString adata = new NSMutableAttributedString ();
		List<NSDictionary> cdata = new List<NSDictionary> ();

		public override IntPtr LowLevelValue => adata.LowLevelValue;
		public override bool FixesAttributesLazily => true;

        Theme theme = new Theme (isDark: false);
        public Theme Theme {
            get => theme;
            set {
                theme = value;
            }
        }

        public MachineInfo MachineInfo { get; set; } = new MachineInfo ();
        public EditorPrinter LastPrinter { get; private set; } = new EditorPrinter ();

        public EditorTextStorage ()
		{
			Initialize ();
		}

		public EditorTextStorage (IntPtr handle)
			: base (handle)
		{
			Initialize ();
		}

		public EditorTextStorage (NSCoder coder)
			: base (coder)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}

		NSDictionary MakeAttrs (NativeColor color, NativeColor darkColor) => new NativeStringAttributes {
			Font = CodeFont,
			ForegroundColor = isDark ? darkColor : color,
		}.Dictionary;

		[Export ("attributesAtIndex:effectiveRange:")]
		IntPtr GetAttributes (nint index, IntPtr rangePointer)
		{
            //Console.WriteLine ("GA " + Thread.CurrentThread.ManagedThreadId + ": " + index);
            var range = new NSRange (index, adata.Length - index);

			var i = (int)index;
			var e = i + 1;
			var a = defaultAttrs.Handle;
			if (i >= 0 && i < cdata.Count) {
				a = cdata[i].Handle;
				while (e < cdata.Count && cdata[e].Handle == a)
					e++;
				range.Location = index;
				range.Length = e - i;
			}

			if (rangePointer != IntPtr.Zero) {
				System.Runtime.InteropServices.Marshal.StructureToPtr<NSRange> (range, rangePointer, false);
			}
			return a;
		}

		[Export ("replaceCharactersInRange:withString:")]
		void ReplaceString (NSRange range, NSString value)
		{
			adata.Replace (range, new NSAttributedString (value));

			int index = (int)range.Location;
			cdata.RemoveRange (index, (int)range.Length);
			var a = index >= 0 && index < cdata.Count ? cdata[index] : defaultAttrs;
			cdata.InsertRange (index, Enumerable.Repeat (a, (int)value.Length));

			Edited (CharsEdited | AttrsEdited, range, value.Length - range.Length);

			BeginFormatting ();
		}

		[Export ("setAttributes:range:")]
		void SetAttributes (NSObject attributes, NSRange range)
		{
		}

		void BeginFormatting ()
		{
			var code = adata.Value;
			ThreadPool.QueueUserWorkItem (_ => {
				try {
                    //Console.WriteLine ("BEGIN FORMATTING");
                    var colorAttrs = theme.ColorAttributes;

                    // Parse the file
                    var printer = new EditorPrinter ();
					var spans = CLanguage.CLanguageService.Colorize (code, MachineInfo, printer);
                    LastPrinter = printer;

					// Flatten the spans
					var ncdata = new List<NSDictionary> (Enumerable.Repeat (defaultAttrs, code.Length));
					foreach (var s in spans) {
						var a = colorAttrs[(int)s.Color];
						for (var i = s.Index; i < s.Index + s.Length; i++) {
							ncdata[i] = a;
						}
					}

					// Show the results
					BeginInvokeOnMainThread (() => {
						var ccode = adata.Value;
						if (ccode == code && cdata.Count == ncdata.Count) {
							//Console.WriteLine ("SET FORMATTING");
							cdata = ncdata;
							Edited (AttrsEdited, new NSRange (0, cdata.Count), 0);
						}
						else {
							//Console.WriteLine ("... failed to SET FORMATTING");
						}
					});
				}
				catch (Exception ex) {
					Debug.WriteLine (ex);
				}
			});
		}

#if __IOS__
		static readonly NSTextStorageEditActions CharsEdited = NSTextStorageEditActions.Characters;
		static readonly NSTextStorageEditActions AttrsEdited = NSTextStorageEditActions.Attributes;
#else
		static readonly nuint CharsEdited = (nuint)(int)(NSTextStorageEditedFlags.EditedCharacters);
		static readonly nuint AttrsEdited = (nuint)(int)(NSTextStorageEditedFlags.EditedAttributed);
#endif
	}
}
