using System;
using System.IO;
using System.Globalization;
namespace CLanguage
{
    public class CodeWriter
    {
        bool needsIndent = true;
        int indent = 0;
        readonly string eol = Environment.NewLine;
        readonly StringWriter w = new StringWriter (CultureInfo.InvariantCulture);

        public string Code => w.ToString ();

        public override string ToString ()
        {
            return Code;
        }

        public CodeWriter Write (string code)
        {
            var lines = code.Split ('\n');
            for (var i = 0; i < lines.Length - 1; i++) {
                WriteIndent ();
                w.Write (lines[i].TrimEnd ());
                w.Write (eol);
                needsIndent = true;
            }
            WriteIndent ();
            w.Write (lines[lines.Length - 1]);
            return this;
        }

        public CodeWriter WriteLine (string code)
        {
            var lines = code.Split ('\n');
            for (var i = 0; i < lines.Length; i++) {
                WriteIndent ();
                w.Write (lines[i].TrimEnd ());
                w.Write (eol);
                needsIndent = true;
            }
            return this;
        }

        public CodeWriter Indent ()
        {
            indent++;
            return this;
        }

        public CodeWriter Outdent ()
        {
            indent--;
            return this;
        }

        public CodeWriter Comment (string comment)
        {
            return Write ("/* " + comment + " */");
        }

        void WriteIndent ()
        {
            if (needsIndent) {
                needsIndent = false;
                w.Write (new string (' ', indent * 4));
            }
        }
    }
}
