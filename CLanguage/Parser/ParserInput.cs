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

        public Token CurrentToken {
            get {
                var tok = Tokens[index];
                if (tok.Kind == TokenKind.IDENTIFIER && typedefs.Contains (tok.StringValue)) {
                    // Don't convert to TYPE_NAME if followed by :: (scope qualifier)
                    // This allows `B::f` to be parsed as a declarator in method definitions
                    if (index + 1 < Tokens.Length && Tokens[index + 1].Kind == TokenKind.COLONCOLON) {
                        return tok;
                    }
                    return tok.AsKind (TokenKind.TYPE_NAME);
                }
                return tok;
            }
        }

        public void AddTypedef (string declaredIdentifier)
        {
            typedefs.Add (declaredIdentifier);
        }
    }
}
