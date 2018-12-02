using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CLanguage.Syntax
{
    public struct Location : IEquatable<Location>
    {
        public bool IsNull => Document == null;
        public string Document { get; }
        public int Line { get; }
        public int Column { get; }

        public Location (string document, int line, int column)
        {
            Document = document;
            Line = line;
            Column = column;
        }

        public override string ToString ()
        {
            if (IsNull)
                return "?(?,?)";
            return $"{Document}({Line},{Column})";
        }

        public static readonly Location Null = new Location ();

        public static bool operator == (Location x, Location y) => x.Line == y.Line && x.Column == y.Column && x.Document == y.Document;
        public static bool operator != (Location x, Location y) => x.Line != y.Line || x.Column != y.Column || x.Document != y.Document;

        public override bool Equals (object obj) => obj is Location && Equals ((Location)obj);
        public bool Equals (Location y) => Line == y.Line && Column == y.Column && Document == y.Document;

        public override int GetHashCode ()
        {
            var hashCode = 1439312346;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode (Document);
            hashCode = hashCode * -1521134295 + Line.GetHashCode ();
            hashCode = hashCode * -1521134295 + Column.GetHashCode ();
            return hashCode;
        }
    }
}
