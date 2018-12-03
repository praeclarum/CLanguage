using System;
using CLanguage.Syntax;
using System.Collections.Generic;

namespace CLanguage.Parser
{
    public class LexedDocument
    {
        public readonly Document Document;
        public readonly Token[] Tokens;

        public LexedDocument (Document document, Report report)
        {
            Document = document;

            var tokens = new List<Token> ();
            var lexer = new Lexer (Document.Path, Document.Content, report);
            while (lexer.advance()) {
                tokens.Add (new Token (lexer.token (), lexer.CurrentLocation, lexer.value ()));
            }
            Tokens = tokens.ToArray ();
        }
    }
}
