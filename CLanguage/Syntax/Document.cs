using System;
using System.Collections.Generic;
using System.Text;

namespace CLanguage.Syntax
{
    public class Document
    {
        public readonly string Path;
        public readonly string Content;
        public readonly Encoding Encoding;

        public bool IsCompilable {
            get {
                switch (System.IO.Path.GetExtension (Path).ToLowerInvariant ()) {
                    case ".c":
                    case ".cpp":
                    case ".cxx":
                    case ".m":
                    case ".mpp":
                    case ".ino":
                        return true;
                    default:
                        return false;
                }
            }
        }

        public Document (string path, string content, Encoding encoding)
        {
            Path = path ?? throw new ArgumentNullException (nameof (path));
            Content = content ?? throw new ArgumentNullException (nameof (content));
            Encoding = encoding ?? throw new ArgumentNullException (nameof (encoding));
        }

        public Document (string path, string content)
        {
            Path = path ?? throw new ArgumentNullException (nameof (path));
            Content = content ?? throw new ArgumentNullException (nameof (content));
            Encoding = Encoding.UTF8;
        }

        public override string ToString () => Path;
    }
}
