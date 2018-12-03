using System;
using CLanguage.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace CLanguage.Parser
{
    public class ParserInput : yyParser.yyInput
    {
        Token[] tokens;
        int index = -1;
        readonly HashSet<string> typedefs = new HashSet<string> ();

        public ParserInput (Token[] tokens)
        {
            this.tokens = tokens;
        }

        public bool advance ()
        {
            if (index + 1 < tokens.Length) {
                index++;
                return true;
            }
            return false;
        }

        public int token () => CurrentToken.Kind;

        public object value () => CurrentToken.Value;

        public Token CurrentToken => tokens[index].Kind == TokenKind.IDENTIFIER && typedefs.Contains((string)tokens[index].Value) ?
            tokens[index].AsKind (TokenKind.TYPE_NAME) :
            tokens[index];

        public void AddTypedef (string declaredIdentifier)
        {
            typedefs.Add (declaredIdentifier);
        }
    }
}
