using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Parser
{
    public class Lexer : yyParser.yyInput
    {
        Preprocessor _pp;

        int _token = -1;
        object _value = null;

        int _lastR = ' ';
        char[] _chbuf = new char[4 * 1024];
        int _chbuflen = 0;

        public Report Report => _pp.Report;

        public Lexer(Preprocessor pp)
        {
            _pp = pp;
        }

        public Lexer (string code, Report report = null)
            : this (new Preprocessor (code, report))
        {
        }

        bool Eof()
        {
            _value = null;
            _token = -1;
            return false;
        }

        public Func<string, bool> IsTypedef { get; set; }

        Dictionary<string, int> _kwTokens = new Dictionary<string, int>()
        {
            { "void", Token.VOID },
            { "char", Token.CHAR },
            { "short", Token.SHORT },
            { "int", Token.INT },
            { "long", Token.LONG },
            { "float", Token.FLOAT },
            { "double", Token.DOUBLE },
            { "signed", Token.SIGNED },
            { "unsigned", Token.UNSIGNED },
            { "bool", Token.BOOL },
            { "struct", Token.STRUCT },
            { "union", Token.UNION },
            { "enum", Token.ENUM },
            { "typedef", Token.TYPEDEF },
            { "extern", Token.EXTERN },
            { "static", Token.STATIC },
            { "auto", Token.AUTO },
            { "register", Token.REGISTER },
            { "inline", Token.INLINE },
            { "const", Token.CONST },
            { "restrict", Token.RESTRICT },
            { "volatile", Token.VOLATILE },
            { "goto", Token.GOTO },
            { "continue", Token.CONTINUE },
            { "break", Token.BREAK },
            { "return", Token.RETURN },
            { "if", Token.IF },
            { "else", Token.ELSE },
            { "for", Token.FOR },
            { "while", Token.WHILE },
        };

        public bool advance()
        {
            //
            // Skip whitespace
            //
            var r = _lastR;            

            //
            // Skip comments
            //
            var skippedComment = true;
            while (skippedComment)
            {
                //
                // Skip white
                //
                while (r >= 0 && r <= ' ')
                {
                    r = _pp.Read();
                }

                skippedComment = false;

                if (r == '/' && _pp.Peek() == '/')
                {
                    var nr = _pp.Read();
                    while (nr > 0 && nr != '\n')
                    {
                        nr = _pp.Read();
                    }
                    r = _pp.Read();
                    skippedComment = true;
                }
            }

            //
            // Are we done?
            //
            if (r == -1)
            {
                return Eof();
            }

            //
            // Make sense of it
            //
            var ch = (char)r;
            if (ch == '_' || char.IsLetter(ch))
            {
                _chbuf[0] = ch;
                _chbuflen = 0;
                while (ch == '_' || char.IsLetter(ch) || char.IsDigit(ch))
                {
                    _chbuf[_chbuflen++] = ch;
                    r = _pp.Read();
                    ch = (char)r;
                }
                _lastR = r;

                var id = new string(_chbuf, 0, _chbuflen);
                _value = id;

                var tok = 0;
                if (_kwTokens.TryGetValue(id, out tok))
                {
                    _token = tok;
                }
                else
                {
                    if (IsTypedef != null && IsTypedef(id))
                    {
                        _token = Token.TYPE_NAME;
                    }
                    else
                    {
                        _token = Token.IDENTIFIER;
                    }
                }
            }
            else if (char.IsDigit(ch))
            {
                var onlydigits = true;
                var islong = false;
                var isunsigned = false;
                var isfloat = false;
                _chbuf[0] = ch;
                _chbuflen = 0;
                while (ch == '.' || char.IsDigit(ch) || ch == 'E' || ch == 'e' || ch == 'f' || ch == 'F' || ch == 'u' || ch == 'U' || ch == 'l' || ch == 'L')
                {
                    if (ch == 'l' || ch == 'L')
                    {
                        islong = true;
                    }
                    else if (ch == 'u' || ch == 'U')
                    {
                        isunsigned = true;
                    }
                    else if (ch == 'f' || ch == 'F')
                    {
                        isfloat = true;
                    }
                    else
                    {
                        onlydigits = onlydigits & char.IsDigit(ch);
                        _chbuf[_chbuflen++] = ch;
                    }
                    r = _pp.Read();
                    ch = (char)r;
                }
                _lastR = r;

                var vals = new string(_chbuf, 0, _chbuflen);
                if (onlydigits)
                {
                    if (islong)
                    {
                        if (isunsigned)
                        {
                            _value = ulong.Parse(vals);
                        }
                        else
                        {
                            _value = long.Parse(vals);
                        }
                    }
                    else
                    {
                        if (isunsigned)
                        {
                            _value = uint.Parse(vals);
                        }
                        else
                        {
                            _value = int.Parse(vals);
                        }
                    }
                }
                else
                {
                    if (isfloat)
                    {
                        _value = float.Parse(vals);
                    }
                    else
                    {
                        _value = double.Parse(vals);
                    }
                }

                _token = Token.CONSTANT;
            }
            else if (r == '=')
            {
                r = _pp.Read();
                if (r == '=')
                {
                    _token = Token.EQ_OP;
                    _value = null;
                    _lastR = _pp.Read();
                }
                else
                {
                    _token = '=';
                    _value = null;
                    _lastR = r;
                }
            }
            else if (r == '!' || r == '%' || r == ',' || r == ':' || r == ';' || r == '?' || r == '(' || r == ')' || r == '{' || r == '}' || r == '[' || r == ']')
            {
                _token = r;
                _value = null;
                _lastR = _pp.Read();
            }
            else if (r == '.')
            {
                var nr = _pp.Read();

                if (nr == '.' && _pp.Peek() == '.')
                {
                    throw new NotImplementedException();
                }
                else
                {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '*' || r == '/')
            {
                var nr = _pp.Read();

                if (nr == '=')
                {
                    _token = (r == '*') ? Token.MUL_ASSIGN : Token.DIV_ASSIGN;
                    _value = null;
                    _lastR = nr;
                }
                else
                {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '|')
            {
                var nr = _pp.Read();

                if (nr == '|')
                {
                    nr = _pp.Read();

                    if (nr == '=')
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        _token = Token.OR_OP;
                        _value = null;
                        _lastR = nr;
                    }
                }
                else if (nr == '=')
                {
                    throw new NotImplementedException();
                }
                else
                {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '+')
            {
                var nr = _pp.Read();

                if (nr == '=')
                {
                    _token = Token.ADD_ASSIGN;
                    _value = null;
                    _lastR = _pp.Read();
                }
                else if (nr == '+')
                {
                    _token = Token.INC_OP;
                    _value = null;
                    _lastR = _pp.Read();
                }
                else
                {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '-')
            {
                var nr = _pp.Read();

                if (nr == '=')
                {
                    _token = Token.SUB_ASSIGN;
                    _value = null;
                    _lastR = _pp.Read();
                }
                else if (nr == '-')
                {
                    _token = Token.DEC_OP;
                    _value = null;
                    _lastR = _pp.Read();
                }
                else
                {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '<')
            {
                var nr = _pp.Read();

                if (nr == '=')
                {
                    _token = Token.LE_OP;
                    _value = null;
                    _lastR = _pp.Read();
                }
                else
                {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else if (r == '>')
            {
                var nr = _pp.Read();

                if (nr == '=')
                {
                    _token = Token.GE_OP;
                    _value = null;
                    _lastR = _pp.Read();
                }
                else
                {
                    _token = r;
                    _value = null;
                    _lastR = nr;
                }
            }
            else
            {
                throw new NotImplementedException();
            }

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
