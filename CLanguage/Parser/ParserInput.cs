using System;
using CLanguage.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace CLanguage.Parser
{
    public class ParserInput : yyParser.yyInput
    {
        public readonly Token[] Tokens;
        int index = -1;
        readonly HashSet<string> typedefs = new HashSet<string> ();

        public ParserInput (Token[] tokens)
        {
            this.Tokens = tokens;
        }

        public bool advance ()
        {
            if (index + 1 < Tokens.Length) {
                index++;
                return true;
            }
            return false;
        }

        public int token () => CurrentToken.Kind;

        public object value () => CurrentToken.Value ?? "";

        public Token CurrentToken => Tokens[index].Kind == TokenKind.IDENTIFIER && typedefs.Contains((string)Tokens[index].Value) ?
            Tokens[index].AsKind (TokenKind.TYPE_NAME) :
            Tokens[index];

        public void AddTypedef (string declaredIdentifier)
        {
            typedefs.Add (declaredIdentifier);
        }
    }
}
