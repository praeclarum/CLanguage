using System;
using CLanguage.Ast;
using CLanguage.Parser;

namespace CLanguage
{
    public static class CLanguageService
    {
        const string DefaultName = "stdin";

        public static TranslationUnit ParseTranslationUnit (string code)
        {
            return ParseTranslationUnit (code, new Report ());
        }

        public static TranslationUnit ParseTranslationUnit (string code, Report report)
        {
            var lexer = new Lexer (DefaultName, code, report);
            var parser = new CParser ();
            return parser.ParseTranslationUnit (lexer);
        }

        public static TranslationUnit ParseTranslationUnit (string code, Report.Printer printer)
        {
            return ParseTranslationUnit (code, new Report (printer));
        }

        public static Interpreter.CInterpreter CreateInterpreter (string code, MachineInfo machineInfo = null, Report.Printer printer = null)
        {
            var exe = Compile (code, machineInfo, printer);

            var i = new Interpreter.CInterpreter (exe);

            return i;
        }

        public static Interpreter.Executable Compile (string code, MachineInfo machineInfo = null, Report.Printer printer = null)
        {
            var report = new Report (printer);

            var mi = machineInfo ?? new MachineInfo ();

            var pp = new Preprocessor (report);
            pp.AddCode ("machine.h", mi.HeaderCode);
            pp.AddCode (DefaultName, code);
            var lexer = new Lexer (pp);
            var parser = new CParser ();
            var tu = parser.ParseTranslationUnit (lexer);

            var c = new Interpreter.Compiler (mi, report);
            c.Add (tu);
            var exe = c.Compile ();

            return exe;
        }
    }
}
