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
        //Lexer _lexer;

        public CParser()
        {
            //debug = new yydebug.yyDebugSimple();
        }
		
		public TranslationUnit ParseTranslationUnit(string name, string code, Report report = null)
        {
            var lex = new Lexer (name, code, report);
			return ParseTranslationUnit (lex);
		}

        public TranslationUnit ParseTranslationUnit(Lexer lexer)
        {
            var report = lexer.Report;

            _tu = new TranslationUnit();
            lexer.IsTypedef = _tu.Typedefs.ContainsKey;
			
			try {
	            yyparse(lexer);
			}
			catch (NotImplementedException err) {
				report.Error (9003, "Feature not implemented: " + err.Message);
			}
			catch (NotSupportedException err) {
				report.Error (9002, "Feature not supported: " + err.Message);
			}
			catch (Exception err) {
				report.Error (9001, "Internal compiler error: " + err.Message);
			}
			
            lexer.IsTypedef = null;
            return _tu;
        }

        void AddDeclaration(object a)
        {
            AddDeclaration(a, _tu);
        }

        void AddDeclaration(object a, Block block)
        {
            if (a is MultiDeclaration multi)
            {
                foreach (var idecl in multi.InitDeclarators)
                {
                    if ((multi.Specifiers.StorageClassSpecifier & StorageClassSpecifier.Typedef) != 0)
                    {
                        if (idecl.Declarator != null)
                        {
                            var name = idecl.Declarator.DeclaredIdentifier;
                            //Typedefs[name] = decl;
                        }
                    }
                    else
                    {
                        var ctype = MakeCType(multi.Specifiers, idecl.Declarator);
                        var name = idecl.Declarator.DeclaredIdentifier;

                        if (ctype is CFunctionType && !HasStronglyBoundPointer(idecl.Declarator))
                        {
                            var ftype = (CFunctionType)ctype;
                            var f = new FunctionDeclaration(name, ftype);

                            for (var i = 0; i < ftype.Parameters.Count; i++)
                            {
                                f.ParameterInfos[i].Name = ftype.Parameters[i].Name;
                            }

                            block.AddFunction(f);
                        }
                        else
                        {
                            if ((ctype is CArrayType) &&
                                (((CArrayType)ctype).LengthExpression == null) &&
                                (idecl.Initializer != null))
                            {
                                if (idecl.Initializer is StructuredInitializer)
                                {
                                    var atype = (CArrayType)ctype;
                                    var len = 0;
                                    foreach (var i in ((StructuredInitializer)idecl.Initializer).Initializers)
                                    {
                                        if (i.Designation == null)
                                        {
                                            len++;
                                        }
                                        else
                                        {
                                            foreach (var de in i.Designation.Designators)
                                            {
                                                // TODO: Pay attention to designators
                                                len++;
                                            }
                                        }
                                    }
                                    atype.LengthExpression = new ConstantExpression(len);
                                }
                                else
                                {
                                    //Report.Error();
                                }
                            }
                            //var init = GetInitExpression(idecl.Initializer);
                            var vdecl = new VariableDeclaration(name, ctype);
                            block.AddVariable(vdecl);
                        }

                        if (idecl.Initializer != null)
                        {
                            var varExpr = new VariableExpression(name);
                            var initExpr = GetInitializerExpression(idecl.Initializer);
                            block.AddStatement(new ExpressionStatement(new AssignExpression(varExpr, initExpr)));
                        }
                    }
                }
            }
            else if (a is FunctionDefinition)
            {
                var fdef = (FunctionDefinition)a;

                var ftype = (CFunctionType)MakeCType(fdef.Specifiers, fdef.Declarator);
                var name = fdef.Declarator.DeclaredIdentifier;

                var f = new FunctionDeclaration(name, ftype);

                for (var i = 0; i < ftype.Parameters.Count; i++)
                {
                    f.ParameterInfos[i].Name = ftype.Parameters[i].Name;
                }

                f.Body = fdef.Body;

                block.AddFunction(f);
            }
        }

        Expression GetInitializerExpression(Initializer init)
        {
            if (init is ExpressionInitializer)
            {
                return ((ExpressionInitializer)init).Expression;
            }
            else if (init is StructuredInitializer)
            {
                var sinit = (StructuredInitializer)init;

                var sexpr = new StructureExpression();

                foreach (var i in sinit.Initializers)
                {
                    var e = GetInitializerExpression(i);

                    if (i.Designation == null || i.Designation.Designators.Count == 0)
                    {
                        var ie = new StructureExpression.Item();
                        ie.Expression = GetInitializerExpression(i);
                        sexpr.Items.Add(ie);
                    }
                    else
                    {
                        
                        foreach (var d in i.Designation.Designators)
                        {
                            var ie = new StructureExpression.Item();
                            ie.Field = d.ToString();
                            ie.Expression = e;
                            sexpr.Items.Add(ie);
                        }
                    }
                }

                return sexpr;
            }
            else
            {
                throw new NotSupportedException(init.ToString());
            }
        }

        FunctionDeclarator GetFunctionDeclarator(Declarator d)
        {
            if (d == null) return null;
            else if (d is FunctionDeclarator) return (FunctionDeclarator)d;
            else return GetFunctionDeclarator(d.InnerDeclarator);
        }

        bool HasStronglyBoundPointer(Declarator d)
        {
            if (d == null) return false;
            else if (d is PointerDeclarator && ((PointerDeclarator)d).StrongBinding) return true;
            else return HasStronglyBoundPointer(d.InnerDeclarator);
        }

        CType MakeCType(DeclarationSpecifiers specs, Declarator decl)
        {
            var type = MakeCType(specs);
            return MakeCType(type, decl);
        }

        CType MakeCType(CType type, Declarator decl)
        {
            if (decl is IdentifierDeclarator)
            {
                // This is the name
            }
            else if (decl is PointerDeclarator)
            {
                var pdecl = (PointerDeclarator)decl;
                var isPointerToFunc = false;

                if (pdecl.StrongBinding)
                {
                    type = MakeCType(type, pdecl.InnerDeclarator);
                    isPointerToFunc = type is CFunctionType;
                }

                var p = pdecl.Pointer;
                while (p != null)
                {
                    type = new CPointerType(type);
                    type.TypeQualifiers = p.TypeQualifiers;
                    p = p.NextPointer;
                }

                if (!pdecl.StrongBinding)
                {
                    type = MakeCType(type, pdecl.InnerDeclarator);
                }

                //
                // Remove 1 level of pointer indirection if this is
                // a pointer to a function since functions are themselves
                // pointers
                //
                if (isPointerToFunc)
                {
                    type = ((CPointerType)type).InnerType;
                }
            }
            else if (decl is ArrayDeclarator)
            {
                var adecl = (ArrayDeclarator)decl;

                while (adecl != null)
                {
                    type = new CArrayType(type, adecl.LengthExpression);
                    adecl = adecl.InnerDeclarator as ArrayDeclarator;
                    if (adecl != null && adecl.InnerDeclarator != null)
                    {
                        if (adecl.InnerDeclarator is IdentifierDeclarator)
                        {
                        }
                        else if (!(adecl.InnerDeclarator is ArrayDeclarator))
                        {
                            type = MakeCType(type, adecl.InnerDeclarator);
                        }
                        else
                        {
                            //throw new NotSupportedException("Unrecognized array syntax");
                        }
                    }
                }
            }
            else if (decl is FunctionDeclarator)
            {
                type = MakeCFunctionType(type, decl);
            }

            return type;
        }

        private CType MakeCFunctionType(CType type, Declarator decl)
        {
            var fdecl = (FunctionDeclarator)decl;

            var name = decl.DeclaredIdentifier;
            var returnType = type;
            var ftype = new CFunctionType(returnType);
            foreach (var pdecl in fdecl.Parameters)
            {
                var pt = MakeCType(pdecl.DeclarationSpecifiers, pdecl.Declarator);
                if (!pt.IsVoid)
                {
                    ftype.Parameters.Add(new CFunctionType.Parameter(pdecl.Name, pt));
                }
            }
            type = ftype;

            type = MakeCType(type, fdecl.InnerDeclarator);

            return type;
        }

        CType MakeCType(DeclarationSpecifiers specs)
        {
            //
            // Try for Basic. The TypeSpecifiers are recorded in reverse from what is actually declared
            // in code.
            //
            var basicTs = specs.TypeSpecifiers.FirstOrDefault(x => x.Kind == TypeSpecifierKind.Builtin);
            if (basicTs != null)
            {
                if (basicTs.Name == "void")
                {
                    return CType.Void;
                }
                else
                {
                    var sign = Signedness.Signed;
                    var size = "";
                    TypeSpecifier trueTs = null;

                    foreach (var ts in specs.TypeSpecifiers)
                    {
                        if (ts.Name == "unsigned")
                        {
                            sign = Signedness.Unsigned;
                        }
                        else if (ts.Name == "signed")
                        {
                            sign = Signedness.Signed;
                        }
                        else if (ts.Name == "short" || ts.Name == "long")
                        {
                            if (size.Length == 0) size = ts.Name;
                            else size = size + " " + ts.Name;
                        }
                        else
                        {
                            trueTs = ts;
                        }
                    }

                    var type = new CBasicType(trueTs == null ? "int" : trueTs.Name, sign, size);
                    type.TypeQualifiers = specs.TypeQualifiers;
                    return type;
                }
            }

            //
            // Rest
            //
            throw new NotImplementedException();
        }

        Declarator FixPointerAndArrayPrecedence(Declarator d)
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

        Block _currentBlock;

        void StartBlock(Location loc)
        {
            if (_currentBlock == null)
            {
                _currentBlock = new Block(_currentBlock, loc);
            }
            else
            {
                _currentBlock = new Block(_currentBlock, loc);
            }
        }

        Block EndBlock(Location loc)
        {
            var retval = _currentBlock;
            retval.EndLocation = loc;
            _currentBlock = retval.Parent;
            return retval;
        }

        Location GetLocation(object obj)
        {
            return null;
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
