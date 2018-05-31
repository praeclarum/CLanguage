using System;
using CLanguage.Ast;
using CLanguage.Parser;

namespace CLanguage
{
    public static class CLanguageService
    {
        public static TranslationUnit ParseTranslationUnit (string code)
        {
            var report = new Report ();
            var lexer = new Lexer (code, report);
            var parser = new CParser ();
            return parser.ParseTranslationUnit (lexer);
        }
    }
}
