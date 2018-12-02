using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Syntax;

namespace CLanguage.Parser
{
    public class Lexer : yyParser.yyInput
    {
        Preprocessor _pp;

        int _token = -1;
        object _value = null;

        int _lastR = -2;
        char[] _chbuf = new char[4 * 1024];
        int _chbuflen = 0;

        Location location = Location.Null;
        int line = 1;
        int column = 1;

        public Report Report => _pp.Report;

        public Lexer(Preprocessor pp)
        {
            _pp = pp;
            location = new Location (pp.CurrentDocument, 1, 1);
        }

        public Lexer (string name, string code, Report report = null)
            : this (new Preprocessor (name, code, report))
        {
        }

        public Location CurrentLocation => location;

        bool Eof()
        {
            _value = null;
            _token = -1;
            return false;
        }

        public Func<string, bool> IsTypedef { get; set; }

        static readonly Dictionary<string, int> _kwTokens = new Dictionary<string, int>()
        {
            { "void", TokenKind.VOID },
            { "char", TokenKind.CHAR },
            { "short", TokenKind.SHORT },
            { "int", TokenKind.INT },
            { "long", TokenKind.LONG },
            { "float", TokenKind.FLOAT },
            { "double", TokenKind.DOUBLE },
            { "signed", TokenKind.SIGNED },
            { "unsigned", TokenKind.UNSIGNED },
            { "bool", TokenKind.BOOL },
            { "struct", TokenKind.STRUCT },
            { "class", TokenKind.CLASS },
            { "union", TokenKind.UNION },
            { "enum", TokenKind.ENUM },
            { "typedef", TokenKind.TYPEDEF },
            { "extern", TokenKind.EXTERN },
            { "static", TokenKind.STATIC },
            { "auto", TokenKind.AUTO },
            { "register", TokenKind.REGISTER },
            { "inline", TokenKind.INLINE },
            { "const", TokenKind.CONST },
            { "restrict", TokenKind.RESTRICT },
            { "volatile", TokenKind.VOLATILE },
            { "goto", TokenKind.GOTO },
            { "continue", TokenKind.CONTINUE },
            { "break", TokenKind.BREAK },
            { "return", TokenKind.RETURN },
            { "if", TokenKind.IF },
            { "else", TokenKind.ELSE },
            { "for", TokenKind.FOR },
            { "while", TokenKind.WHILE },
            { "true", TokenKind.TRUE },
            { "false", TokenKind.FALSE },
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

        public void SkipWhiteSpace ()
        {
            if (_pp.CurrentDocument?.Path != location.Document?.Path) {
                line = 1;
                column = 1;
            }

            //
            // Skip whitespace
            //
            var r = _lastR;
            if (r == -2) {
                r = _pp.Read ();
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
                    if (r == '\n') {
                        line++;
                        column = 1;
                    }
                    r = _pp.Read ();
                }

                skippedComment = false;

                if (r == '/' && _pp.Peek () == '/') {
                    var nr = _pp.Read ();
                    while (nr > 0 && nr != '\n') {
                        nr = _pp.Read ();
                    }
                    r = _pp.Read ();
                    line++;
                    column = 1;
                    skippedComment = true;
                }
                else if (r == '/' && _pp.Peek () == '*') {
                    var nr = _pp.Read ();
                    while (nr > 0 && !(nr == '*' && _pp.Peek () == '/')) {
                        if (nr == '\n') {
                            line++;
                            column = 1;
                        }
                        nr = _pp.Read ();
                    }
                    _pp.Read (); // Consume ending /
                    r = _pp.Read ();
                    skippedComment = true;
                }
                else if (r == '#') {
                    var nr = _pp.Read ();
                    while (nr > 0 && nr != '\n') {
                        nr = _pp.Read ();
                    }
                    r = _pp.Read ();
                    line++;
                    column = 1;
                    skippedComment = true;
                }
            }

            _lastR = r;
        }

        public bool advance()
        {
            SkipWhiteSpace ();

            var r = _lastR;            

            //
            // Are we done?
            //
            if (r == -1)
            {
                return Eof();
            }

            //
            // Record where we are
            //
            location = new Location (_pp.CurrentDocument, line, column);

            //
            // Make sense of it
            //
            var ch = (char)r;
            if (char.IsDigit (ch)) {
                var onlydigits = true;
                var islong = false;
                var isunsigned = false;
                var isfloat = false;
                var ishex = false;
                _chbuf[0] = ch;
                _chbuflen = 0;
                while (ch == '.' || char.IsDigit (ch) || ch == 'E' || ch == 'e' || ch == 'f' || ch == 'F' || ch == 'u' || ch == 'U' || ch == 'l' || ch == 'L' || (!ishex && ch=='x')) {
                    if (ch == 'l' || ch == 'L') {
                        islong = true;
                    }
                    else if (ch == 'u' || ch == 'U') {
                        isunsigned = true;
                    }
                    else if (ch == 'f' || ch == 'F') {
                        isfloat = true;
                    }
                    else if (ch == 'x' && _chbuflen == 1 && _chbuf[0] == '0') {
                        ishex = true;
                    }
                    else {
                        onlydigits = onlydigits && char.IsDigit (ch);
                        _chbuf[_chbuflen++] = ch;
                    }
                    r = _pp.Read ();
                    ch = (char)r;
                }
                _lastR = r;

                var vals = new string (_chbuf, 0, _chbuflen);
                var icult = System.Globalization.CultureInfo.InvariantCulture;
                if (onlydigits) {
                    var style = ishex ? System.Globalization.NumberStyles.HexNumber : System.Globalization.NumberStyles.None;
                    if (islong) {
                        if (isunsigned) {
                            _value = ulong.Parse (vals, style, icult);
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
                r = _pp.Read ();
                if (r == '=') {
                    _token = TokenKind.EQ_OP;
                    _value = null;
                    _lastR = _pp.Read ();
                }
                else {
                    _token = '=';
                    _value = null;
                    _lastR = r;
                }
            }
            else if (r == '!') {
                r = _pp.Read ();
                if (r == '=') {
                    _token = TokenKind.NE_OP;
                    _value = null;
                    _lastR = _pp.Read ();
                }
                else {
                    _token = '!';
                    _value = null;
                    _lastR = r;
                }
            }
            else if (r == ':') {
                r = _pp.Read ();
                if (r == ':') {
                    _token = TokenKind.COLONCOLON;
                    _value = null;
                    _lastR = _pp.Read ();
                }
                else {
                    _token = ':';
                    _value = null;
                    _lastR = r;
                }
            }
            else if (r == ',' || r == ';' || r == '?' || r == '(' || r == ')' || r == '{' || r == '}' || r == '[' || r == ']' || r == '~' || r == '%' || r == '^') {
                _token = r;
                _value = null;
                _lastR = _pp.Read ();
            }
            else if (r == '.') {
                var nr = _pp.Read ();

                if (nr == '.' && _pp.Peek () == '.') {
                    throw new NotImplementedException ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '*' || r == '/') {
                var nr = _pp.Read ();

                if (nr == '=') {
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
            else if (r == '&') {
                var nr = _pp.Read ();

                if (nr == '&') {
                    nr = _pp.Read ();

                    if (nr == '=') {
                        throw new NotImplementedException ();
                    }
                    else {
                        _token = TokenKind.AND_OP;
                        _value = null;
                        _lastR = nr;
                    }
                }
                else if (nr == '=') {
                    throw new NotImplementedException ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '|') {
                var nr = _pp.Read ();

                if (nr == '|') {
                    nr = _pp.Read ();

                    if (nr == '=') {
                        throw new NotImplementedException ();
                    }
                    else {
                        _token = TokenKind.OR_OP;
                        _value = null;
                        _lastR = nr;
                    }
                }
                else if (nr == '=') {
                    throw new NotImplementedException ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '+') {
                var nr = _pp.Read ();

                if (nr == '=') {
                    _token = TokenKind.ADD_ASSIGN;
                    _value = null;
                    _lastR = _pp.Read ();
                }
                else if (nr == '+') {
                    _token = TokenKind.INC_OP;
                    _value = null;
                    _lastR = _pp.Read ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '-') {
                var nr = _pp.Read ();

                if (nr == '=') {
                    _token = TokenKind.SUB_ASSIGN;
                    _value = null;
                    _lastR = _pp.Read ();
                }
                else if (nr == '-') {
                    _token = TokenKind.DEC_OP;
                    _value = null;
                    _lastR = _pp.Read ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '<') {
                var nr = _pp.Read ();

                if (nr == '=') {
                    _token = TokenKind.LE_OP;
                    _value = null;
                    _lastR = _pp.Read ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '>') {
                var nr = _pp.Read ();

                if (nr == '=') {
                    _token = TokenKind.GE_OP;
                    _value = null;
                    _lastR = _pp.Read ();
                }
                else {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '\"') {
                _chbuflen = 0;
                r = _pp.Read ();
                ch = (char)r;
                var done = r < 0 || ch == '\"';
                while (!done && _chbuflen + 1 < _chbuf.Length) {
                    if (ch == '\\') {
                        r = _pp.Read ();
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
                                            while (r > 0 && r != '\n') {
                                                r = _pp.Read ();
                                            }
                                        }
                                        else {
                                            throw new NotSupportedException ("Unrecognized string escape sequence");
                                        }
                                    }
                                    break;
                            }
                            r = _pp.Read ();
                            ch = (char)r;
                        }
                    }
                    else {
                        _chbuf[_chbuflen++] = ch;
                        r = _pp.Read ();
                        ch = (char)r;
                    }
                    done = r < 0 || ch == '\"';
                }

                _lastR = _pp.Read ();

                _token = TokenKind.STRING_LITERAL;
                _value = new string (_chbuf, 0, _chbuflen);
            }
            else if (r == '\'') {
                _chbuflen = 0;
                r = _pp.Read ();
                ch = (char)r;
                var done = r < 0 || ch == '\'';
                while (!done && _chbuflen + 1 < _chbuf.Length) {
                    _chbuf[_chbuflen++] = ch;
                    r = _pp.Read ();
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

                _lastR = _pp.Read ();
                _token = TokenKind.CONSTANT;
                _value = _chbuf[0];
            }
            else {
                _chbuf[0] = ch;
                _chbuflen = 0;
                while (ch == '_' || char.IsLetterOrDigit (ch) || r > 127) {
                    _chbuf[_chbuflen++] = ch;
                    r = _pp.Read ();
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

            Console.WriteLine ("LEX " + _pp.CurrentPosition);

            return true;
        }

        public int token()
        {
            return _token;
        }

        public object value()
        {
            return _value;
        }
    }
}
