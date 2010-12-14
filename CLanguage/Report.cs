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

        List<string> _extraInformation = new List<string>();

        ReportPrinter _printer = null;

        public Report(ReportPrinter p)
        {
            _printer = p;
        }

        public abstract class AbstractMessage
        {
            public string MessageType { get; protected set; }
            public Location Location { get; protected set; }
            public bool IsWarning { get; protected set; }
            public int Code { get; protected set; }
            public string Text { get; protected set; }
            public List<string> RelatedSymbols { get; protected set; }

            public AbstractMessage()
            {
            }

            public override bool Equals(object obj)
            {
                var o = obj as AbstractMessage;
                return (o != null) &&
                    (o.Code == Code) &&
                    (o.Location == Location) &&
                    (o.IsWarning == IsWarning) &&
                    (o.Text == Text);                    
            }

            public override int GetHashCode()
            {
                return Code.GetHashCode() + Location.GetHashCode();
            }
        }

        public class ErrorMessage : AbstractMessage
        {
            public ErrorMessage(int code, Location loc, string error, List<string> extraInformation)
            {
                MessageType = "Error";
                Code = code;
                Text = error;
                Location = loc;
                IsWarning = false;
                if (extraInformation != null)
                {
                    RelatedSymbols = new List<string>();
                    RelatedSymbols.AddRange(extraInformation);
                }
            }
        }

        Dictionary<AbstractMessage, bool> _previousErrors = new Dictionary<AbstractMessage, bool>();

        public void Error(int code, Location loc, string error)
        {
            if (_reportingDisabled > 0)
                return;

            ErrorMessage msg = new ErrorMessage(code, loc, error, _extraInformation);
            _extraInformation.Clear();

            if (!_previousErrors.ContainsKey(msg))
            {
                _previousErrors[msg] = true;
                _printer.Print(msg);
            }
        }

        public void Error(int code, Location loc, string format, params object[] args)
        {
            Error(code, loc, String.Format(format, args));
        }

        public void Error(int code, string error)
        {
            Error(code, Location.Null, error);
        }

        public void Error(int code, string format, params object[] args)
        {
            Error(code, Location.Null, String.Format(format, args));
        }

        //
        // Generic base for any message writer
        //
        public abstract class ReportPrinter
        {
            /// <summary>  
            ///   Whether to dump a stack trace on errors. 
            /// </summary>
            public bool Stacktrace;

            int warnings, errors;

            public int WarningsCount
            {
                get { return warnings; }
            }

            public int ErrorsCount
            {
                get { return errors; }
            }

            protected virtual string FormatText(string txt)
            {
                return txt;
            }

            //
            // When (symbols related to previous ...) can be used
            //
            public virtual bool HasRelatedSymbolSupport
            {
                get { return true; }
            }

            public virtual void Print(AbstractMessage msg)
            {
                if (msg.IsWarning)
                    ++warnings;
                else
                    ++errors;
            }

            protected void Print(AbstractMessage msg, TextWriter output)
            {
                StringBuilder txt = new StringBuilder();
                if (!msg.Location.IsNull)
                {
                    txt.Append(msg.Location.ToString());
                    txt.Append(" ");
                }

                txt.AppendFormat("{0} C{1:0000}: {2}", msg.MessageType, msg.Code, msg.Text);

                if (!msg.IsWarning)
                    output.WriteLine(FormatText(txt.ToString()));
                else
                    output.WriteLine(txt.ToString());

                if (msg.RelatedSymbols != null)
                {
                    foreach (string s in msg.RelatedSymbols)
                        output.WriteLine(s + msg.MessageType + ")");
                }
            }
        }
    }
}
