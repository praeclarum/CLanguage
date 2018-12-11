using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Syntax;
using CLanguage.Types;

namespace CLanguage.Parser
{
    public partial class CParser
    {
        //
        // Controls the verbosity of the errors produced by the parser
        //
        static public int yacc_verbose_flag;

        TranslationUnit _tu;
        ParserInput lexer;

        public CParser()
        {
            //debug = new yydebug.yyDebugSimple();
        }
		
        public TranslationUnit ParseTranslationUnit (string name, string code, Preprocessor.Include include, Report report)
        {
            var lexed = new LexedDocument (new Document (name, code), report);
            return ParseTranslationUnit (report, System.IO.Path.GetFileNameWithoutExtension (name), include, lexed.Tokens);
        }

        public TranslationUnit ParseTranslationUnit (Report report, string name, Preprocessor.Include include, params Token[][] tokens)
        {
            var preprocessor = new Preprocessor (include, report, tokens);
            lexer = new ParserInput (preprocessor.Preprocess ());

            _tu = new TranslationUnit (name);

            if (lexer.Tokens.Length == 0)
                return _tu;

            try {
                yyparse (lexer);
            }
            catch (NotImplementedException err) {
                report.Error (9000, lexer.CurrentToken.Location, lexer.CurrentToken.EndLocation, 
                    "Not Supported: " + err.Message);
            }
            catch (NotSupportedException err) {
                report.Error (9002, lexer.CurrentToken.Location, lexer.CurrentToken.EndLocation,
                    "Not Supported: " + err.Message);
            }
            catch (Exception err) when (err.Message == "irrecoverable syntax error") {
                report.Error (1001, lexer.CurrentToken.Location, lexer.CurrentToken.EndLocation,
                    "Syntax error");
            }
            catch (Exception err) {
                report.Error (9001, lexer.CurrentToken.Location, lexer.CurrentToken.EndLocation,
                    "Internal Error: " + err.Message);
            }

            return _tu;
        }

        public static Expression ParseExpression (Report report, Token[] tokens)
        {
            var p = new CParser ();
            var prefix = new[] { new Token (TokenKind.AUTO, "auto"), new Token (TokenKind.IDENTIFIER, "_"), new Token ('=') };
            var suffix = new[] { new Token (';') };
            var tu = p.ParseTranslationUnit (report, CLanguageService.DefaultCodePath, ((_,__) => null), prefix, tokens, suffix);
            if (tu.Statements.FirstOrDefault () is MultiDeclaratorStatement mds && mds.InitDeclarators.Count == 1 && mds.InitDeclarators[0].Initializer is ExpressionInitializer ei) {
                return ei.Expression;
            }
            report.Error (9010, "Expression could not be parsed");
            return ConstantExpression.False;
        }

        public static Dictionary<string, Expression> ParseExpressions (Report report, IEnumerable<(string VariableName, Token[] Tokens)> tokens)
        {
            var p = new CParser ();
            var suffix = new[] { new Token (';') };
            var q = from t in tokens
                    let prefix = new[] { new Token (TokenKind.AUTO, "auto"), new Token (TokenKind.IDENTIFIER, t.VariableName), new Token ('=') }
                    from x in prefix.Concat (t.Tokens).Concat (suffix)
                    select x;
            var allTokens = q.ToArray ();
            var tu = p.ParseTranslationUnit (report, CLanguageService.DefaultCodePath, ((_, __) => null), allTokens);
            var r = from s in tu.Statements.OfType<MultiDeclaratorStatement> ()
                    where s.InitDeclarators.Count == 1
                    let init = s.InitDeclarators[0].Initializer as ExpressionInitializer
                    where init != null
                    select (s.InitDeclarators[0].Declarator.DeclaredIdentifier, init.Expression);
            return r.ToDictionary (x => x.Item1, x => x.Item2);
        }

        void AddDeclaration (object a)
        {
            _tu.AddStatement ((Statement)a);

            if (a is MultiDeclaratorStatement mds) {
                switch (mds.Specifiers.StorageClassSpecifier) {
                    case StorageClassSpecifier.Typedef:
                        foreach (var i in mds.InitDeclarators) {
                            lexer.AddTypedef (i.Declarator.DeclaredIdentifier);
                        }
                        break;
                }
            }
        }

        Declarator FixPointerAndArrayPrecedence (Declarator d)
        {
            if (d is PointerDeclarator && d.InnerDeclarator != null && d.InnerDeclarator is ArrayDeclarator)
            {
                var a = d.InnerDeclarator;
                var p = d;
                var i = a.InnerDeclarator;
                a.InnerDeclarator = p;
                p.InnerDeclarator = i;
                return a;
            }
            else
            {
                return null;
            }
        }

        Declarator MakeArrayDeclarator(Declarator left, TypeQualifiers tq, Expression len, bool isStatic)
        {
            var a = new ArrayDeclarator();
            a.LengthExpression = len;

            if (left.StrongBinding)
            {
                var i = left.InnerDeclarator;
                a.InnerDeclarator = i;
                left.InnerDeclarator = a;
                return left;
            }
            else
            {
                a.InnerDeclarator = left;
                return a;
            }
        }

        Location GetLocation(object obj)
        {
            return Location.Null;
            /*
                if (obj is Tokenizer.LocatedToken)
                    return ((Tokenizer.LocatedToken) obj).Location;
                if (obj is MemberName)
                    return ((MemberName) obj).Location;

                if (obj is Expression)
                    return ((Expression) obj).Location;

                return lexer.Location;*/
        }
    }
}
