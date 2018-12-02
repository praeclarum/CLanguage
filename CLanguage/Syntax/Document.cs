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

        public Document (string path, string content, Encoding encoding)
        {
            Path = path;
            Content = content;
            Encoding = encoding;
        }

        public override string ToString () => Path;
    }
}
