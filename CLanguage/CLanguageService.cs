using System;
using CLanguage.Ast;
using CLanguage.Parser;

namespace CLanguage
{
    public static class CLanguageService
    {
        public static TranslationUnit ParseTranslationUnit (string code)
        {
            return ParseTranslationUnit (code, new Report ());
        }

        public static TranslationUnit ParseTranslationUnit (string code, Report report)
        {
            var lexer = new Lexer (code, report);
            var parser = new CParser ();
            return parser.ParseTranslationUnit (lexer);
        }

        public static TranslationUnit ParseTranslationUnit (string code, ReportPrinter printer)
        {
            return ParseTranslationUnit (code, new Report (printer));
        }
    }
}
