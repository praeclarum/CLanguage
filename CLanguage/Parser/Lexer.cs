using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Syntax;

namespace CLanguage.Parser
{
    public class Lexer
    {
        int _token = -1;
        object? _value = null;

        int _lastR = -2;
        char[] _chbuf = new char[4 * 1024];
        int _chbuflen = 0;

        Location location;
        Location endLocation;
        int line = 1;
        int column = 1;

        public Report Report { get; }
        public Document Document { get; }

        public Token CurrentToken => new Token (_token, _value, location, endLocation);

        public Lexer (Document document, Report? report = null)
        {
            Report = report ?? new Report ();
            Document = document;
            location = new Location (document, 0, 1, 1);
            endLocation = location;
        }

        public Lexer (string name, string code, Report? report = null)
            : this (new Document (name, code), report)
        {
        }

        bool Eof ()
        {
            _value = null;
            _token = -1;
            return false;
        }

        public Func<string, bool> IsTypedef { get; set; } = _ => false;

        static readonly Dictionary<string, int> _kwTokens = new Dictionary<string, int> ()
        {
            { "auto", TokenKind.AUTO },
            { "bool", TokenKind.BOOL },
            { "break", TokenKind.BREAK },
            { "case", TokenKind.CASE },
            { "char", TokenKind.CHAR },
            { "class", TokenKind.CLASS },
            { "const", TokenKind.CONST },
            { "continue", TokenKind.CONTINUE },
            { "default", TokenKind.DEFAULT },
            { "do", TokenKind.DO },
            { "double", TokenKind.DOUBLE },
            { "else", TokenKind.ELSE },
            { "enum", TokenKind.ENUM },
            { "extern", TokenKind.EXTERN },
            { "false", TokenKind.FALSE },
            { "float", TokenKind.FLOAT },
            { "for", TokenKind.FOR },
            { "goto", TokenKind.GOTO },
            { "if", TokenKind.IF },
            { "inline", TokenKind.INLINE },
            { "int", TokenKind.INT },
            { "long", TokenKind.LONG },
            { "public", TokenKind.PUBLIC },
            { "private", TokenKind.PRIVATE },
            { "protected", TokenKind.PROTECTED },
            { "register", TokenKind.REGISTER },
            { "restrict", TokenKind.RESTRICT },
            { "return", TokenKind.RETURN },
            { "short", TokenKind.SHORT },
            { "signed", TokenKind.SIGNED },
            { "sizeof", TokenKind.SIZEOF },
            { "static", TokenKind.STATIC },
            { "struct", TokenKind.STRUCT },
            { "switch", TokenKind.SWITCH },
            { "true", TokenKind.TRUE },
            { "typedef", TokenKind.TYPEDEF },
            { "union", TokenKind.UNION },
            { "unsigned", TokenKind.UNSIGNED },
            { "void", TokenKind.VOID },
            { "volatile", TokenKind.VOLATILE },
            { "while", TokenKind.WHILE },
        };

        public static readonly HashSet<int> KeywordTokens = new HashSet<int> (_kwTokens.Values);
        public static readonly HashSet<int> OperatorTokens = new HashSet<int> {
            TokenKind.EQ_OP,
            TokenKind.GE_OP,
            TokenKind.LE_OP,
            TokenKind.NE_OP,
            TokenKind.OR_OP,
            TokenKind.AND_OP,
            TokenKind.DEC_OP,
            TokenKind.INC_OP,
            TokenKind.PTR_OP,
            TokenKind.LEFT_OP,
            TokenKind.RIGHT_OP,
        };

        int nextPosition = 0;

        int Read ()
        {
            if (nextPosition < Document.Content.Length) {
                var r = Document.Content[nextPosition];
                nextPosition++;
                column++;
                return r;
            }
            return -1;
        }

        int Peek ()
        {
            if (nextPosition < Document.Content.Length) {
                return Document.Content[nextPosition];
            }
            return -1;
        }

        public void SkipWhiteSpace ()
        {
            //
            // Skip whitespace
            //
            var r = _lastR;
            if (r == -2) {
                r = Read ();
            }

            //
            // Skip comments
            //
            var skippedComment = true;
            while (skippedComment) {
                //
                // Skip white
                //
                while (r >= 0 && r <= ' ') {
                    if (r == '\n' && r != 8232) {
                        break;
                    }
                    r = Read ();
                }

                skippedComment = false;

                if (r == '/' && Peek () == '/') {
                    var nr = Read ();
                    while (nr > 0 && nr != '\n' && nr != 8232) {
                        nr = Read ();
                    }
                    r = Read ();
                    line++;
                    column = 1;
                    skippedComment = true;
                }
                else if (r == '/' && Peek () == '*') {
                    var nr = Read ();
                    while (nr > 0 && !(nr == '*' && Peek () == '/')) {
                        if (nr == '\n' || nr == 8232) {
                            line++;
                            column = 1;
                        }
                        nr = Read ();
                    }
                    Read (); // Consume ending /
                    r = Read ();
                    skippedComment = true;
                }
            }

            _lastR = r;
        }

        public bool Advance ()
        {
            SkipWhiteSpace ();

            var r = _lastR;

            //
            // Are we done?
            //
            if (r == -1) {
                return Eof ();
            }

            //
            // Record where we are
            //
            location = new Location (location.Document, nextPosition - 1, line, column);

            //
            // Make sense of it
            //
            var ch = (char)r;
            if (ch == '\n' || ch == 8232) {
                _token = TokenKind.EOL;
                _value = null;
                _lastR = Read ();
                line++;
                column = 1;
            }
            else if (char.IsDigit (ch)) {
                var onlydigits = true;
                var islong = false;
                var isunsigned = false;
                var isfloat = false;
                var ishex = false;
                _chbuf[0] = ch;
                _chbuflen = 0;
                while (ch == '.' || char.IsDigit (ch) || ch == 'E' || ch == 'e' || ch == 'f' || ch == 'F' || ch == 'u' || ch == 'U' || ch == 'l' || ch == 'L' || (!ishex && ch == 'x') || (ishex && IsHex (ch))) {
                    if (ch == 'l' || ch == 'L') {
                        islong = true;
                    }
                    else if (ch == 'u' || ch == 'U') {
                        isunsigned = true;
                    }
                    else if (!ishex && (ch == 'f' || ch == 'F')) {
                        isfloat = true;
                    }
                    else if (ch == 'x' && _chbuflen == 1 && _chbuf[0] == '0') {
                        ishex = true;
                    }
                    else {
                        onlydigits = onlydigits && char.IsDigit (ch);
                        _chbuf[_chbuflen++] = ch;
                    }
                    r = Read ();
                    ch = (char)r;
                }
                _lastR = r;

                var vals = new string (_chbuf, 0, _chbuflen);
                var icult = System.Globalization.CultureInfo.InvariantCulture;
                endLocation = new Location (location.Document, _lastR >= 0 ? nextPosition - 1 : location.Document.Content.Length, line, column);
                if (onlydigits || ishex) {
                    var style = ishex ? System.Globalization.NumberStyles.HexNumber : System.Globalization.NumberStyles.None;
                    if (islong) {
                        if (isunsigned) {
                            if (ulong.TryParse (vals, style, icult, out var v)) {
                                _value = v;
                            }
                            else {
                                _value = (ulong)0;
                                Report.Error (1021, location, endLocation, "Integral constant is too large");
                            }
                        }
                        else {
                            _value = long.Parse (vals, style, icult);
                        }
                    }
                    else {
                        if (isunsigned) {
                            _value = uint.Parse (vals, style, icult);
                        }
                        else {
                            _value = int.Parse (vals, style, icult);
                        }
                    }
                }
                else {
                    if (isfloat) {
                        _value = float.Parse (vals, icult);
                    }
                    else {
                        _value = double.Parse (vals, icult);
                    }
                }

                _token = TokenKind.CONSTANT;
            }
            else if (r == '=') {
                r = Read ();
                if (r == '=') {
                    _token = TokenKind.EQ_OP;
                    _value = null;
                    _lastR = Read ();
                }
                else {
                    _token = '=';
                    _value = null;
                    _lastR = r;
                }
            }
            else if (r == '!') {
                r = Read ();
                if (r == '=') {
                    _token = TokenKind.NE_OP;
                    _value = null;
                    _lastR = Read ();
                }
                else {
                    _token = '!';
                    _value = null;
                    _lastR = r;
                }
            }
            else if (r == ':') {
                r = Read ();
                if (r == ':') {
                    _token = TokenKind.COLONCOLON;
                    _value = null;
                    _lastR = Read ();
                }
                else {
                    _token = ':';
                    _value = null;
                    _lastR = r;
                }
            }
            else if (r == ',' || r == ';' || r == '?' || r == '(' || r == ')' || r == '{' || r == '}' || r == '[' || r == ']' || r == '~' || r == '%' || r == '#' || r == '\\') {
                _token = r;
                _value = null;
                _lastR = Read ();
            }
            else if (r == '.') {
                var nr = Read (); // Dot #2

                if (nr == '.' && Peek () == '.') {
                    r = Read (); // Dot #3
                    if (r == '.') {
                        _token = TokenKind.ELLIPSIS;
                        _value = null;
                        _lastR = Read ();
                    }
                    else {
                        _token = '.';
                        _value = null;
                        _lastR = r;
                        Report.Error (1001, location + 1, location + 2, "Identifier expected");
                    }
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '*' || r == '/') {
                var nr = Read ();

                if (nr == '=') {
                    nr = Read ();
                    _token = (r == '*') ? TokenKind.MUL_ASSIGN : TokenKind.DIV_ASSIGN;
                    _value = null;
                    _lastR = nr;
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '^') {
                var nr = Read ();

                if (nr == '=') {
                    nr = Read ();
                    _token = TokenKind.BINARY_XOR_ASSIGN;
                    _value = null;
                    _lastR = nr;
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '&') {
                var nr = Read ();

                if (nr == '&') {
                    nr = Read ();

                    if (nr == '=') {
                        nr = Read ();
                        _token = TokenKind.AND_ASSIGN;
                        _value = null;
                        _lastR = nr;
                    }
                    else {
                        _token = TokenKind.AND_OP;
                        _value = null;
                        _lastR = nr;
                    }
                }
                else if (nr == '=') {
                    nr = Read ();
                    _token = TokenKind.BINARY_AND_ASSIGN;
                    _value = null;
                    _lastR = nr;
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '|') {
                var nr = Read ();

                if (nr == '|') {
                    nr = Read ();

                    if (nr == '=') {
                        nr = Read ();
                        _token = TokenKind.OR_ASSIGN;
                        _value = null;
                        _lastR = nr;
                    }
                    else {
                        _token = TokenKind.OR_OP;
                        _value = null;
                        _lastR = nr;
                    }
                }
                else if (nr == '=') {
                    nr = Read ();
                    _token = TokenKind.BINARY_OR_ASSIGN;
                    _value = null;
                    _lastR = nr;
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '+') {
                var nr = Read ();

                if (nr == '=') {
                    _token = TokenKind.ADD_ASSIGN;
                    _value = null;
                    _lastR = Read ();
                }
                else if (nr == '+') {
                    _token = TokenKind.INC_OP;
                    _value = null;
                    _lastR = Read ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '-') {
                var nr = Read ();

                if (nr == '=') {
                    _token = TokenKind.SUB_ASSIGN;
                    _value = null;
                    _lastR = Read ();
                }
                else if (nr == '-') {
                    _token = TokenKind.DEC_OP;
                    _value = null;
                    _lastR = Read ();
                }
                else if (nr == '>') {
                    _token = TokenKind.PTR_OP;
                    _value = null;
                    _lastR = Read ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '<') {
                var nr = Read ();

                if (nr == '=') {
                    _token = TokenKind.LE_OP;
                    _value = null;
                    _lastR = Read ();
                }
                else if (nr == '<') {
                    _token = TokenKind.LEFT_OP;
                    _value = null;
                    _lastR = Read ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '>') {
                var nr = Read ();

                if (nr == '=') {
                    _token = TokenKind.GE_OP;
                    _value = null;
                    _lastR = Read ();
                }
                else if (nr == '>') {
                    _token = TokenKind.RIGHT_OP;
                    _value = null;
                    _lastR = Read ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '>') {
                var nr = Read ();

                if (nr == '=') {
                    _token = TokenKind.GE_OP;
                    _value = null;
                    _lastR = Read ();
                }
                else if (nr == '>') {
                    _token = TokenKind.RIGHT_OP;
                    _value = null;
                    _lastR = Read ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '\"') {
                _chbuflen = 0;
                r = Read ();
                ch = (char)r;
                var done = r < 0 || ch == '\"';
                while (!done && _chbuflen + 1 < _chbuf.Length) {
                    if (ch == '\\') {
                        r = Read ();
                        ch = (char)r;
                        if (r >= 0) {
                            switch (ch) {
                                case '\\':
                                    _chbuf[_chbuflen++] = '\\';
                                    break;
                                case 'r':
                                    _chbuf[_chbuflen++] = '\r';
                                    break;
                                case 'n':
                                    _chbuf[_chbuflen++] = '\n';
                                    break;
                                case 't':
                                    _chbuf[_chbuflen++] = '\t';
                                    break;
                                case '\'':
                                    _chbuf[_chbuflen++] = '\'';
                                    break;
                                case '\"':
                                    _chbuf[_chbuflen++] = '\"';
                                    break;
                                default: {
                                        if (char.IsWhiteSpace ((char)r)) {
                                            while (r > 0 && r != '\n' && r != 8232) {
                                                r = Read ();
                                            }
                                        }
                                        else {
                                            throw new NotSupportedException ("Unrecognized string escape sequence");
                                        }
                                    }
                                    break;
                            }
                            r = Read ();
                            ch = (char)r;
                        }
                    }
                    else if (ch == '\n' || ch == 8232) {
                        endLocation = new Location (location.Document, _lastR >= 0 ? nextPosition - 1 : location.Document.Content.Length, line, column);
                        Report.Error (1010, location, endLocation, "Newline in constant");
                        done = true;
                    }
                    else {
                        _chbuf[_chbuflen++] = ch;
                        r = Read ();
                        ch = (char)r;
                    }
                    done = done || r < 0 || ch == '\"';
                }

                _lastR = Read ();

                _token = TokenKind.STRING_LITERAL;
                _value = new string (_chbuf, 0, _chbuflen);
            }
            else if (r == '\'') {
                _chbuflen = 0;
                r = Read ();
                ch = (char)r;
                var done = r < 0 || ch == '\'';
                while (!done && _chbuflen + 1 < _chbuf.Length) {
                    _chbuf[_chbuflen++] = ch;
                    r = Read ();
                    ch = (char)r;
                    done = r < 0 || ch == '\'';
                }

                if (_chbuflen > 1 && _chbuf[0] == '\\') {
                    switch (_chbuf[1]) {
                        case '\\':
                            _chbuf[0] = '\\';
                            _chbuflen = 1;
                            break;
                        case 'r':
                            _chbuf[0] = '\r';
                            _chbuflen = 1;
                            break;
                        case 'n':
                            _chbuf[0] = '\n';
                            _chbuflen = 1;
                            break;
                        case 't':
                            _chbuf[0] = '\t';
                            _chbuflen = 1;
                            break;
                        case '\'':
                            _chbuf[0] = '\'';
                            _chbuflen = 1;
                            break;
                        case '\"':
                            _chbuf[0] = '\"';
                            _chbuflen = 1;
                            break;
                        default:
                            throw new NotSupportedException ("Unrecognized char escape sequence");
                    }
                }

                _lastR = Read ();
                _token = TokenKind.CONSTANT;
                _value = _chbuf[0];
            }
            else {
                _chbuf[0] = ch;
                _chbuflen = 0;
                while (ch == '_' || char.IsLetterOrDigit (ch) || r > 127) {
                    _chbuf[_chbuflen++] = ch;
                    r = Read ();
                    ch = (char)r;
                }

                if (_chbuflen == 0) {
                    throw new NotSupportedException ($"Character '{(char)r}' is not parsable");
                }

                _lastR = r;

                var id = new string (_chbuf, 0, _chbuflen);
                _value = id;

                var tok = 0;
                if (_kwTokens.TryGetValue (id, out tok)) {
                    _token = tok;
                }
                else {
                    if (IsTypedef != null && IsTypedef (id)) {
                        _token = TokenKind.TYPE_NAME;
                    }
                    else {
                        _token = TokenKind.IDENTIFIER;
                    }
                }
            }

            endLocation = new Location (location.Document, _lastR >= 0 ? nextPosition - 1 : location.Document.Content.Length, line, column);
            return true;
        }

        static bool IsHex (char c)
        {
            switch (c) {
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return true;
            }
            return false;
        }
    }
}
