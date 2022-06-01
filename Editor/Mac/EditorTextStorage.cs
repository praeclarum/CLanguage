using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Foundation;

using CLanguage.Compiler;
using static CLanguage.Editor.Extensions;
using CLanguage.Syntax;

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
        readonly NSMutableAttributedString adata = new NSMutableAttributedString ();
		List<NSDictionary> cdata = new List<NSDictionary> ();

		public override IntPtr LowLevelValue => adata.LowLevelValue;
		public override bool FixesAttributesLazily => true;

        Theme theme = new Theme (isDark: false, fontScale: 1);
        public Theme Theme {
            get => theme;
            set {
                if (!ReferenceEquals (theme, value)) {
                    theme = value;
                    BeginFormatting ();
                }
            }
        }

		//public CompilerOptions Options { get; set; } = new CompilerOptions (new MachineInfo (), new Report (), Enumerable.Empty<CLanguage.Syntax.Document> ());
		public ColorizeFunc Colorize { get; set; } = (x, y) => Array.Empty<ColorSpan> ();
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

		[Export ("attributesAtIndex:effectiveRange:")]
		IntPtr GetAttributes (nint index, IntPtr rangePointer)
		{
            //Console.WriteLine ("GA " + Thread.CurrentThread.ManagedThreadId + ": " + index);
            var range = new NSRange (index, adata.Length - index);

			var i = (int)index;
			var e = i + 1;
			var a = theme.CommentAttributes.Handle;
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
			var a = index >= 0 && index < cdata.Count ? cdata[index] : theme.CommentAttributes;
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
					var spans = Colorize (code, printer);
                    LastPrinter = printer;

                    // Flatten the spans
                    var defaultAttrs = theme.CommentAttributes;
                    var ncdata = new List<NSDictionary> (Enumerable.Repeat (defaultAttrs, code.Length));
					foreach (var s in spans) {
						var a = colorAttrs[(int)s.Color];
						for (var i = s.Index; i < s.Index + s.Length; i++) {
							ncdata[i] = a;
						}
					}

                    // Add error and warning underlines
                    foreach (var m in printer.Messages) {
                        if (m.Location.IsNull || m.EndLocation.IsNull)
                            continue;
                        if (m.Location.Document.Path != CLanguageService.DefaultCodePath)
                            continue;
                        var s = new NSRange (m.Location.Index, m.EndLocation.Index - m.Location.Index);
                        if (s.Location >= 0 && s.Length > 0 && s.Location < code.Length && s.Location + s.Length <= code.Length) {
                            var existingA = ncdata[(int)s.Location];
                            var a = m.MessageType == "Error" ? theme.ErrorAttributes (m.Text, existingA) : theme.WarningAttributes (m.Text, existingA);
                            for (var i = s.Location; i < s.Location + s.Length; i++) {
                                ncdata[(int)i] = a;
                            }
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
