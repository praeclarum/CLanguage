using System;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Linq;
using System.Diagnostics;
using CLanguage.Syntax;

namespace CLanguage.Editor
{
    public class EditorKeyboardAccessory : UIView, IUIInputViewAudioFeedback
    {
        readonly WeakReference<CEditor> editor;
        readonly UIVisualEffectView visualEffectView;

        static readonly bool ios11 = UIDevice.CurrentDevice.CheckSystemVersion (11, 0);

        bool isBlurDark = false;

        readonly UIStackView stackView;

        UIButton[] activeKeys = Array.Empty<UIButton> ();

        Theme theme = new Theme (isDark: false, fontScale: 1);
        public Theme Theme {
            get => theme;
            set {
                if (!ReferenceEquals (theme, value)) {
                    theme = value;
                    OnThemeChanged ();
                }
            }
        }

        public bool EnableInputClicksWhenVisible => true;

        public EditorKeyboardAccessory (CEditor editor)
            : base (new CGRect (0, 0, 320, 44))
        {
            this.editor = new WeakReference<CEditor> (editor);
            BackgroundColor = UIColor.Clear;
            visualEffectView = new UIVisualEffectView (UIBlurEffect.FromStyle (isBlurDark ? UIBlurEffectStyle.Dark : UIBlurEffectStyle.Light)) {
                Frame = Bounds,
                AutoresizingMask = UIViewAutoresizing.FlexibleDimensions,
            };
            stackView = new UIStackView (Bounds) {
                AutoresizingMask = UIViewAutoresizing.FlexibleDimensions,
                Distribution = UIStackViewDistribution.FillEqually,
            };
            visualEffectView.ContentView.AddSubview (stackView);
            AddSubview (visualEffectView);
            OnThemeChanged ();

            SetKeys (
                new IndentKey (),
                new OutdentKey (),
                new ToggleCommentKey (),
                new InsertTextKey ("{"),
                new InsertTextKey ("}"),
                new InsertTextKey ("("),
                new InsertTextKey (")"),
                new InsertTextKey ("="),
                new InsertTextKey (","),
                new InsertTextKey (";")
                );
        }

        abstract class Key
        {
            public string Title = "";
            public abstract void Execute (NSObject sender, CEditor editor);
        }

        class IndentKey : Key
        {
            public IndentKey () => Title = "\u21A6";
            public override void Execute (NSObject sender, CEditor editor) => editor.Indent (sender);
        }

        class OutdentKey : Key
        {
            public OutdentKey () => Title = "\u21A4";
            public override void Execute (NSObject sender, CEditor editor) => editor.Outdent (sender);
        }

        class ToggleCommentKey : Key
        {
            public ToggleCommentKey () => Title = "//";
            public override void Execute (NSObject sender, CEditor editor) => editor.ToggleComment (sender);
        }

        class InsertTextKey : Key
        {
            private readonly string text;
            public InsertTextKey (string text)
            {
                Title = text;
                this.text = text;
            }
            public override void Execute (NSObject sender, CEditor editor)
            {
                var textView = editor.TextView;
                var sr = textView.SelectedRange;
                var ir = new NSRange (sr.Location + sr.Length, 0);
                var p = textView.GetPosition (textView.BeginningOfDocument, sr.Location + sr.Length);
                var range = textView.GetTextRange (p, p);
                //textView.ReplaceText (range, text);
                textView.InsertText (text);
            }
        }

        void OnThemeChanged ()
        {
            if (isBlurDark != theme.IsDark) {
                isBlurDark = theme.IsDark;
                visualEffectView.Effect = UIBlurEffect.FromStyle (isBlurDark ? UIBlurEffectStyle.Dark : UIBlurEffectStyle.Light);
            }
            var a = new UIStringAttributes (new NSMutableDictionary (theme.ColorAttributes[(int)SyntaxColor.Operator])) {
                Font = UIFont.BoldSystemFontOfSize (UIFont.SystemFontSize),
            };
            foreach (var k in activeKeys) {
                var title = k.Title (UIControlState.Normal);
                k.SetAttributedTitle (new NSAttributedString (title, a), UIControlState.Normal);
            }
        }

        void SetKeys (params Key[] keys)
        {
            var keyViews = keys.Select (CreateKeyView).ToArray ();
            foreach (var s in activeKeys)
                s.RemoveFromSuperview ();
            foreach (var v in keyViews) {
                stackView.AddArrangedSubview (v);
            }
            activeKeys = keyViews;
            OnThemeChanged ();
        }

        UIButton CreateKeyView (Key key)
        {
            var v = UIButton.FromType (UIButtonType.RoundedRect);
            v.SetTitle (key.Title, UIControlState.Normal);
            v.TouchUpInside += (sender, e) => {
                OnKey (sender as NSObject, key);
            };
            return v;
        }

        void OnKey (NSObject sender, Key key)
        {
            try {
                if (editor.TryGetTarget (out var e)) {
                    UIDevice.CurrentDevice.PlayInputClick ();
                    key.Execute (sender, e);
                }
            }
            catch (Exception ex) {
                Debug.WriteLine (ex);
            }
        }
    }
}
