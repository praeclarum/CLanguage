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
            var pp = new Preprocessor (report);
            pp.AddCode ("machine.h", mi.HeaderCode);
            pp.AddCode (name, code);

            var lexer = new Lexer (pp);

            var infile = false;
            var p = 0;

            var tokens = new List<ColorSpan> ();

            var funcs = new HashSet<string> (mi.InternalFunctions.Where (x => string.IsNullOrEmpty (x.NameContext)).Select (x => x.Name));

            while (lexer.advance ()) {
                if (!infile && pp.CurrentFilePath == name) {
                    infile = true;
                }
                if (infile && (pp.CurrentFilePath == name || pp.CurrentFilePath == null)) {
                    var e = pp.CurrentPosition;
                    //Console.WriteLine ($"{pp.CurrentFilePath}@{e} \"{code.Substring (p, e - p)}\" = {lexer.token()} ({lexer.value()})");
                    var color = ColorizeToken (lexer.token (), lexer.value (), funcs);
                    tokens.Add (new ColorSpan {
                        Index = p,
                        Length = e - p,
                        Color = color,
                    });
                    p = e;
                }
            }

            return tokens.ToArray ();
        }

        public static SyntaxColor ColorizeToken (int token, object value, HashSet<string> funcs)
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
                case Token.STRING_LITERAL:
                case Token.TRUE:
                case Token.FALSE:
                    return SyntaxColor.Constant;
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
