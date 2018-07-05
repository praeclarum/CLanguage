using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CLanguage
{
    public class Report
    {
        int _reportingDisabled = 0;

        List<string> _extraInformation = new List<string> ();

        Printer _printer = null;

        public Report (Printer printer = null)
        {
            _printer = printer ?? new Printer ();
        }

        Dictionary<AbstractMessage, bool> _previousErrors = new Dictionary<AbstractMessage, bool> ();

        public void Error (int code, Syntax.Location loc, string error)
        {
            if (_reportingDisabled > 0)
                return;

            ErrorMessage msg = new ErrorMessage (code, loc, error, _extraInformation);
            _extraInformation.Clear ();

            if (!_previousErrors.ContainsKey (msg)) {
                _previousErrors[msg] = true;
                _printer.Print (msg);
            }
        }

        public void Error (int code, Syntax.Location loc, string format, params object[] args)
        {
            Error (code, loc, String.Format (format, args));
        }

        public void Error (int code, string error)
        {
            Error (code, Syntax.Location.Null, error);
        }

        public void Error (int code, string format, params object[] args)
        {
            Error (code, Syntax.Location.Null, String.Format (format, args));
        }


        public abstract class AbstractMessage
        {
            public string MessageType { get; protected set; }
            public Syntax.Location Location { get; protected set; }
            public bool IsWarning { get; protected set; }
            public int Code { get; protected set; }
            public string Text { get; protected set; }
            public List<string> RelatedSymbols { get; protected set; }

            public AbstractMessage ()
            {
            }

            public override bool Equals (object obj)
            {
                var o = obj as AbstractMessage;
                return (o != null) &&
                    (o.Code == Code) &&
                    (o.Location == Location) &&
                    (o.IsWarning == IsWarning) &&
                    (o.Text == Text);
            }

            public override int GetHashCode ()
            {
                return Code.GetHashCode () + Location.GetHashCode ();
            }

            public override string ToString ()
            {
                return string.Format ("{0} C{1:0000}: {2}", MessageType, Code, Text);
            }
        }

        public class ErrorMessage : AbstractMessage
        {
            public ErrorMessage (int code, Syntax.Location loc, string error, List<string> extraInformation)
            {
                MessageType = "Error";
                Code = code;
                Text = error;
                Location = loc;
                IsWarning = false;
                if (extraInformation != null) {
                    RelatedSymbols = new List<string> ();
                    RelatedSymbols.AddRange (extraInformation);
                }
            }
        }

        public class Printer
        {
            int warnings, errors;

            public int WarningsCount {
                get { return warnings; }
            }

            public int ErrorsCount {
                get { return errors; }
            }

            public virtual void Print (AbstractMessage msg)
            {
                if (msg.IsWarning)
                    ++warnings;
                else
                    ++errors;
            }
        }

        public class SavedPrinter : Printer
        {
            public readonly List<AbstractMessage> Messages = new List<AbstractMessage> ();

            public override void Print (AbstractMessage msg)
            {
                base.Print (msg);

                Messages.Add (msg);
            }
        }

        public class TextWriterPrinter : Printer
        {
            TextWriter output;

            public TextWriterPrinter (TextWriter output)
            {
                this.output = output;
            }

            public override void Print (AbstractMessage msg)
            {
                base.Print (msg);

                if (!msg.Location.IsNull) {
                    output.Write (msg.Location.ToString ());
                    output.Write (" ");
                }

                output.WriteLine ("{0} C{1:0000}: {2}", msg.MessageType, msg.Code, msg.Text);

                if (msg.RelatedSymbols != null) {
                    foreach (string s in msg.RelatedSymbols) {
                        output.WriteLine ("  " + s);
                    }
                }
            }
        }
    }
}
