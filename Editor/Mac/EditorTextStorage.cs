using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Foundation;

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
using System.Diagnostics;
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

		readonly NSDictionary[] colorAttrs = Enumerable.Repeat (defaultAttrs, 16).ToArray ();

		readonly bool isDark = false;
		readonly CLanguage.MachineInfo machineInfo = null;

		readonly NSMutableAttributedString adata = new NSMutableAttributedString ();
		List<NSDictionary> cdata = new List<NSDictionary> ();

		public override IntPtr LowLevelValue => adata.LowLevelValue;
		public override bool FixesAttributesLazily => true;

		public EditorTextStorage ()
		{
			Initialize ();
		}

		public EditorTextStorage (CLanguage.MachineInfo machineInfo, bool isDark)
		{
			this.machineInfo = machineInfo;
			this.isDark = isDark;
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
			colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Number] = MakeAttrs (Rgb (197, 0, 11), Rgb (255, 211, 32));
			colorAttrs[(int)CLanguage.Syntax.SyntaxColor.String] = MakeAttrs (Rgb (197, 0, 11), Rgb (255, 211, 32));
			colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Identifier] = MakeAttrs (NativeColor.Black, NativeColor.White);
			colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Keyword] = MakeAttrs (Rgb (52, 120, 184), Rgb (52, 120, 184));
			colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Type] = MakeAttrs (Rgb (0, 128, 128), Rgb (0, 164, 164));
			colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Function] = MakeAttrs (Rgb (204, 102, 0), Rgb (204, 102, 0));
			colorAttrs[(int)CLanguage.Syntax.SyntaxColor.Operator] = MakeAttrs (Rgb (96, 96, 96), Rgb (164, 164, 192));
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

					// Parse the file
					var spans = CLanguage.CLanguageService.Colorize (code, machineInfo);

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
