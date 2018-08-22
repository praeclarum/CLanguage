using System;
using CLanguage.Syntax;
using CLanguage.Parser;
using System.Collections.Generic;
using System.Linq;

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



        public static ColorSpan[] Colorize (string code, MachineInfo machineInfo = null, Report.Printer printer = null)
        {
            var report = new Report (printer);

            var mi = machineInfo ?? new MachineInfo ();

            var name = "colorize.cpp";
            var pp = new Preprocessor (report, passthrough: true);
            pp.AddCode (name, code);

            var lexer = new Lexer (pp);

            var tokens = new List<ColorSpan> ();

            var funcs = new HashSet<string> (mi.InternalFunctions.Where (x => string.IsNullOrEmpty (x.NameContext)).Select (x => x.Name));

            while (true) {
                lexer.SkipWhiteSpace ();

                var p = pp.CurrentPosition - 1;

                if (!lexer.advance ())
                    break;

                var tok = lexer.token ();
                var val = lexer.value ();

                var e = pp.CurrentPosition - 1;
                if (e < 0 || p + 1 >= code.Length) e = code.Length;
                //Console.WriteLine ($"{e-p}@{p} \"{code.Substring (p, e - p)}\" = {tok} ({val})");
                var color = ColorizeToken (tok, val, funcs);
                tokens.Add (new ColorSpan {
                    Index = p,
                    Length = e - p,
                    Color = color,
                });
            }

            return tokens.ToArray ();
        }

        static SyntaxColor ColorizeToken (int token, object value, HashSet<string> funcs)
        {
            switch (token) {
                case Token.INT:
                case Token.SHORT:
                case Token.LONG:
                case Token.CHAR:
                case Token.FLOAT:
                case Token.DOUBLE:
                case Token.BOOL:
                    return SyntaxColor.Type;
                case Token.IDENTIFIER:
                    if (value is string s && funcs.Contains (s))
                        return SyntaxColor.Function;
                    return SyntaxColor.Identifier;
                case Token.CONSTANT:
                    if (value is char) return SyntaxColor.String;
                    return SyntaxColor.Number;
                case Token.TRUE:
                case Token.FALSE:
                    return SyntaxColor.Number;
                case Token.STRING_LITERAL:
                    return SyntaxColor.String;
                default:
                    if (token < 128 || Lexer.OperatorTokens.Contains (token))
                        return SyntaxColor.Operator;
                    else if (Lexer.KeywordTokens.Contains (token))
                        return SyntaxColor.Keyword;
                    break;
            }
            return SyntaxColor.Comment;
        }
    }
}
