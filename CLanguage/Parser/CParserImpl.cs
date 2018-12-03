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
		
        public TranslationUnit ParseTranslationUnit (string name, string code, Report report)
        {
            var lexed = new LexedDocument (new Document (name, code), report);
            return ParseTranslationUnit (report, lexed.Tokens);
        }

        public TranslationUnit ParseTranslationUnit (Report report, params IEnumerable<Token>[] tokens)
        {
            var preprocessor = new Preprocessor (tokens);
            lexer = new ParserInput (preprocessor.Preprocess ());

            _tu = new TranslationUnit ();
            //lexer.IsTypedef = _tu.Typedefs.ContainsKey;

            try {
                yyparse (lexer);
            }
            catch (NotImplementedException err) {
                report.Error (9003, lexer.CurrentToken.Location, lexer.CurrentToken.EndLocation, 
                    "Feature not implemented: " + err.Message);
            }
            catch (NotSupportedException err) {
                report.Error (9002, lexer.CurrentToken.Location, lexer.CurrentToken.EndLocation,
                    "Feature not supported: " + err.Message);
            }
            catch (Exception err) when (err.Message == "irrecoverable syntax error") {
                report.Error (1001, lexer.CurrentToken.Location, lexer.CurrentToken.EndLocation,
                    "Syntax error");
            }
            catch (Exception err) {
                report.Error (9001, lexer.CurrentToken.Location, lexer.CurrentToken.EndLocation,
                    "Internal compiler error: " + err.Message);
            }

            //lexer.IsTypedef = null;
            return _tu;
        }

        void AddDeclaration(object a)
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
