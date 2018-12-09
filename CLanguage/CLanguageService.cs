using System;
using CLanguage.Syntax;
using CLanguage.Parser;
using System.Collections.Generic;
using System.Linq;
using CLanguage.Compiler;

namespace CLanguage
{
    public static class CLanguageService
    {
        public const string DefaultCodePath = "main.cpp";

        public static TranslationUnit ParseTranslationUnit (string code)
        {
            return ParseTranslationUnit (code, new Report ());
        }

        public static TranslationUnit ParseTranslationUnit (string code, Report report)
        {
            var parser = new CParser ();
            return parser.ParseTranslationUnit (DefaultCodePath, code, report);
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
            var doc = new Document (DefaultCodePath, code);
            var options = new Compiler.CompilerOptions (mi, report, new[] { doc });

            var c = new Compiler.CCompiler (options);
            var exe = c.Compile ();

            return exe;
        }

        public static ColorSpan[] Colorize (string code, MachineInfo machineInfo = null, Report.Printer printer = null)
        {
            var report = new Report (printer);

            var mi = machineInfo ?? new MachineInfo ();

            var doc = new Document (DefaultCodePath, code);
            var lexed = new LexedDocument (doc, report);

            var compiler = new CCompiler (new CompilerOptions (mi, report, new[] { doc }));
            var exe = compiler.Compile ();

            var funcs = new HashSet<string> (mi.InternalFunctions.Where (x => string.IsNullOrEmpty (x.NameContext)).Select (x => x.Name));

            var tokens = lexed.Tokens.Where (x => x.Kind != TokenKind.EOL).Select (ColorizeToken).ToArray ();

            return tokens;

            ColorSpan ColorizeToken (Token token) => new ColorSpan {
                Index = token.Location.Index,
                Length = token.EndLocation.Index - token.Location.Index,
                Color = GetTokenColor (token),
            };

            SyntaxColor GetTokenColor (Token token)
            {
                switch (token.Kind) {
                    case TokenKind.INT:
                    case TokenKind.SHORT:
                    case TokenKind.LONG:
                    case TokenKind.CHAR:
                    case TokenKind.FLOAT:
                    case TokenKind.DOUBLE:
                    case TokenKind.BOOL:
                        return SyntaxColor.Type;
                    case TokenKind.IDENTIFIER:
                        if (token.Value is string s && funcs.Contains (s))
                            return SyntaxColor.Function;
                        return SyntaxColor.Identifier;
                    case TokenKind.CONSTANT:
                        if (token.Value is char)
                            return SyntaxColor.String;
                        return SyntaxColor.Number;
                    case TokenKind.TRUE:
                    case TokenKind.FALSE:
                        return SyntaxColor.Number;
                    case TokenKind.STRING_LITERAL:
                        return SyntaxColor.String;
                    default:
                        if (token.Kind < 128 || Lexer.OperatorTokens.Contains (token.Kind))
                            return SyntaxColor.Operator;
                        else if (Lexer.KeywordTokens.Contains (token.Kind)) {
                            switch ((string)token.Value) {
                                case "unsigned":
                                case "signed":
                                    return SyntaxColor.Type;
                                default:
                                    return SyntaxColor.Keyword;
                            }
                        }
                        break;
                }
                return SyntaxColor.Comment;
            }
        }
    }
}
