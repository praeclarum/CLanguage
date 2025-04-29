using System;
using System.Collections.Generic;

namespace CLanguage.Syntax
{
    public struct Token : IEquatable<Token>
    {
        public readonly int Kind;
        public readonly Location Location;
        public readonly Location EndLocation;
        public readonly object? Value;

        public string StringValue => Value is string s ? s : ((Value is not null ? Convert.ToString (Value, System.Globalization.CultureInfo.InvariantCulture) : "") ?? "");

        public Token (int kind, object? value, Location location, Location endLocation)
        {
            Kind = kind;
            Value = value ?? "";
            Location = location;
            EndLocation = endLocation;
        }

        public Token (int kind, object? value)
        {
            Kind = kind;
            Value = value;
            Location = Location.Null;
            EndLocation = Location.Null;
        }

        public Token (char kind)
        {
            Kind = kind;
            Value = null;
            Location = Location.Null;
            EndLocation = Location.Null;
        }

        public string Text => Location.IsNull || EndLocation.IsNull || Location.Document.Path != EndLocation.Document.Path ? "" :
            Location.Document.Content.Substring (Location.Index, EndLocation.Index - Location.Index);

        public override string ToString ()
        {
            var text = Location.IsNull ? (Value?.ToString () ?? (Kind < 127 ? ((char)Kind).ToString () : "")) : Text;
            return Kind < 127 ? $"\"{text}\"" : $"\"{text}\": {Parser.CParser.yyname (Kind)}";
        }

        public Token AsKind (int kind)
        {
            return new Token (kind, Value, Location, EndLocation);
        }

        public override bool Equals (object? obj) => obj is Token && Equals ((Token)obj);

        public bool Equals (Token other)
        {
            return Kind == other.Kind &&
                   Location.Equals (other.Location) &&
                   ((Value == null && other.Value == null) ||
                    (Value != null && Value.Equals (other)));
        }

        public override int GetHashCode ()
        {
            var hashCode = 666775603;
            hashCode = hashCode * -1521134295 + Kind.GetHashCode ();
            hashCode = hashCode * -1521134295 + EqualityComparer<Location>.Default.GetHashCode (Location);
            hashCode = hashCode * -1521134295 + (Value != null ? EqualityComparer<object>.Default.GetHashCode (Value) : 0);
            return hashCode;
        }

        public static bool operator == (Token token1, Token token2)
        {
            return token1.Equals (token2);
        }

        public static bool operator != (Token token1, Token token2)
        {
            return !(token1 == token2);
        }
    }
}
