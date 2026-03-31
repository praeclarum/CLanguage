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
                // When we advance past a struct/class/union keyword followed by
                // IDENTIFIER '{' or IDENTIFIER ':', register the identifier as a
                // TYPE_NAME immediately. This allows the struct's own name to be
                // recognized inside its body (e.g., `struct V { V operator+(V o); };`)
                // without requiring a forward declaration.
                TryRegisterStructName ();
                return true;
            }
            return false;
        }

        void TryRegisterStructName ()
        {
            var tok = Tokens[index];
            if (tok.Kind == TokenKind.STRUCT || tok.Kind == TokenKind.CLASS || tok.Kind == TokenKind.UNION) {
                if (index + 2 < Tokens.Length) {
                    var nameTok = Tokens[index + 1];
                    var afterName = Tokens[index + 2];
                    if (nameTok.Kind == TokenKind.IDENTIFIER &&
                        (afterName.Kind == '{' || afterName.Kind == ':')) {
                        typedefs.Add (nameTok.StringValue);
                    }
                }
            }
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
