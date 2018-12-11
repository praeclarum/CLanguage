using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLanguage.Syntax;
using CLanguage.Compiler;
using CLanguage.Interpreter;
using System.Diagnostics;
using CLanguage.Types;

namespace CLanguage.Parser
{
    public class Preprocessor
    {
        readonly List<Token> tokens;
        private readonly Include include;
        private readonly Report report;

        public delegate Token[] Include (string filePath, bool relative);

        class Define
        {
            public string Name;
            public string[] Parameters;
            public bool HasParameters;
            public List<Token> Body;
            public override string ToString ()
            {
                return Name + ": [" + string.Join (", ", Body) + "]";
            }
        }

        public Preprocessor (Include include, Report report, params Token[][] tokens)
        {
            this.tokens = tokens.SelectMany (x => x).ToList ();
            this.include = include;
            this.report = report;
        }

        public Token[] Preprocess ()
        {
            var defines = new Dictionary<string, Define> ();
            while (PreprocessIteration (defines, IncludeBuiltins, tokens, report)) {
                // Keep going until nothing changes
            }
            return tokens.ToArray ();
        }

        Token[] IncludeBuiltins (string filePath, bool relative)
        {
            if (filePath == "stdint.h") {
                return Array.Empty<Token> ();
            }
            return include (filePath, relative);
        }

        static bool PreprocessIteration (Dictionary<string, Define> defines, Include include, List<Token> tokens, Report report)
        {
            var anotherIterationNeeded = false;

            var i = 0;
            while (i < tokens.Count) {
                var t = tokens[i];
                if (t.Kind == TokenKind.EOL || t.Kind == '\\') {
                    tokens.RemoveAt (i);
                }
                else if (t.Kind == TokenKind.IDENTIFIER) {
                    var ident = t.Value.ToString ();
                    if (defines.TryGetValue (ident, out var define)) {
                        if (define.HasParameters) {
                            var (args, len) = ReadDefineArgs (i + 1, tokens);
                            var newDefines = new Dictionary<string, Define> (defines);
                            newDefines.Remove (define.Name); // Prevent recursion
                            for (var ai = 0; ai < Math.Min (args.Count, define.Parameters.Length); ai++) {
                                args[ai].Name = define.Parameters[ai];
                                newDefines[args[ai].Name] = args[ai];
                            }
                            var newBody = define.Body.ToList ();
                            while (PreprocessIteration (newDefines, include, newBody, report)) {
                                // Do as much as we can
                            }
                            tokens.RemoveRange (i, len + 1);
                            tokens.InsertRange (i, newBody);
                        }
                        else {
                            tokens.RemoveAt (i);
                            tokens.InsertRange (i, define.Body);
                        }
                        anotherIterationNeeded = true;
                    }
                    else {
                        i++;
                    }
                }
                else if (t.Kind == '#' && i + 1 < tokens.Count &&
                    (tokens[i + 1].Kind == TokenKind.IDENTIFIER || tokens[i + 1].Kind == TokenKind.IF || tokens[i + 1].Kind == TokenKind.ELSE)) {
                    var eol = i + 1;
                    while (eol < tokens.Count && tokens[eol].Kind != TokenKind.EOL) {
                        if (tokens[eol].Kind == '\\' && eol + 1 < tokens.Count && tokens[eol + 1].Kind == TokenKind.EOL) {
                            eol++;
                        }
                        eol++;
                    }
                    var insertTokens = default (Token[]);
                    var tokenValueString = tokens[i + 1].Value?.ToString () ?? "";
                    switch (tokenValueString) {
                        case "define":
                            if (eol - i > 2) {
                                var nameToken = tokens[i + 2];
                                var body = tokens.Skip (i + 3).Take (eol - i - 3).ToList ();
                                var ps = Array.Empty<string> ();
                                var hasPs = false;
                                if (body.Count >= 2 && body[0].Kind == '(' && body[0].Location.Index == nameToken.EndLocation.Index) {
                                    var endParam = body.FindIndex (1, x => x.Kind == ')');
                                    if (endParam >= 0 && endParam + 1 < body.Count) {
                                        ps = body.Take (endParam).Where (x => x.Kind == TokenKind.IDENTIFIER).Select (x => (string)x.Value).ToArray ();
                                        body.RemoveRange (0, endParam + 1);
                                        hasPs = true;
                                    }
                                }
                                var define = new Define {
                                    Name = nameToken.Value?.ToString (),
                                    Body = body,
                                    Parameters = ps,
                                    HasParameters = hasPs,
                                };
                                if (!string.IsNullOrWhiteSpace (define.Name)) {
                                    defines[define.Name] = define;
                                }
                            }
                            else {
                                report.Warning (1025, tokens[i].Location, tokens[eol - 1].EndLocation, "Incomplete #define");
                            }
                            break;
                        case "include":
                            if (eol - i > 2) {
                                var relative = tokens[i + 2].Kind == TokenKind.STRING_LITERAL;
                                var iname = "";
                                for (var j = i + 2; j < eol; j++) {
                                    var k = tokens[j].Kind;
                                    if (k == '/' || k == '\\' || k == '.') {
                                        iname += (char)k;
                                    }
                                    else if (k == TokenKind.IDENTIFIER || k == TokenKind.STRING_LITERAL) {
                                        iname += tokens[j].Value.ToString ();
                                    }
                                }
                                insertTokens = include (iname, relative);
                                if (insertTokens == null) {
                                    report.Warning (1027, tokens[i + 2].Location, tokens[eol - 1].EndLocation, "Failed to find file");
                                }
                            }
                            else {
                                report.Warning (1026, tokens[i].Location, tokens[eol - 1].EndLocation, "Incomplete #include");
                            }
                            break;
                        case "endif":
                        case "else":
                            report.Warning (1028, tokens[i].Location, tokens[eol - 1].EndLocation, "Unexpected preprocessor directive");
                            break;
                        case "if":
                        case "ifdef":
                        case "ifndef": {
                                var isTrue = true;
                                if (tokenValueString == "if") {
                                    isTrue = EvalIfCondition (defines, tokens.Skip (i + 2).Take (eol - (i + 2)).ToArray ());
                                }
                                else {
                                    var isDefined = (i + 2 < tokens.Count && tokens[i + 2].Value is string s && defines.ContainsKey (s));
                                    isTrue = tokenValueString == "ifdef" ? isDefined : !isDefined;
                                }

                                //
                                // Look for else and endif
                                //
                                int elseStartIndex = -1;
                                int elseEndIndex = -1;
                                int endifStartIndex = tokens.Count;
                                int endifEndIndex = tokens.Count;
                                int ifDepth = 1;
                                for (var j = i + 3; j < tokens.Count - 1 && endifEndIndex == tokens.Count; j++) {
                                    if (tokens[j].Kind == '#' && tokens[j+1].Value is string eis) {
                                        switch (eis) {
                                            case "if":
                                            case "ifdef":
                                            case "ifndef":
                                                ifDepth++;
                                                break;
                                            case "else":
                                                if (ifDepth == 1) {
                                                    elseStartIndex = j;
                                                    elseEndIndex = j + 2;
                                                }
                                                break;
                                            case "endif":
                                                ifDepth--;
                                                if (ifDepth == 0) {
                                                    endifStartIndex = j;
                                                    endifEndIndex = j + 2;
                                                }
                                                break;
                                        }
                                    }
                                }

                                //
                                // Do the replacement
                                //
                                if (isTrue) {
                                    if (elseStartIndex >= eol) {
                                        insertTokens = tokens.Skip (eol).Take (elseStartIndex - eol).ToArray ();
                                    }
                                    else {
                                        insertTokens = tokens.Skip (eol).Take (endifStartIndex - eol).ToArray ();
                                    }
                                }
                                else {
                                    if (elseEndIndex >= eol) {
                                        insertTokens = tokens.Skip (elseEndIndex).Take (endifStartIndex - elseEndIndex).ToArray ();
                                    }
                                }
                                eol = endifEndIndex;
                            }
                            break;
                        default:
                            report.Warning (1024, tokens[i].Location, tokens[eol - 1].EndLocation, "Cannot understand preprocessor");
                            break;
                    }
                    if (eol < tokens.Count)
                        eol++;
                    tokens.RemoveRange (i, eol - i);
                    if (insertTokens != null) {
                        tokens.InsertRange (i, insertTokens);
                    }
                    anotherIterationNeeded = true;
                }
                else {
                    i++;
                }
            }

            return anotherIterationNeeded;
        }

        static bool EvalIfCondition (Dictionary<string, Define> defines, Token[] tokens)
        {
            try {
                var r = new Report ();
                var code = string.Join ("", tokens.Select (x => x.Text));
                var q = from kv in defines
                        where kv.Value.Body.Count > 0
                        select (kv.Key, kv.Value.Body.ToArray ());
                var expressions = CParser.ParseExpressions (r, q.Concat (new[] { ("__value__", tokens) }));
                var expression = expressions["__value__"];
                var context = new PreprocessorContext (r, expressions);
                var value = expression.EvalConstant (context);
                return value.Int32Value != 0;
            }
            catch (Exception ex) {
                Debug.WriteLine (ex);
                return false;
            }
        }

        class PreprocessorContext : EmitContext
        {
            readonly Dictionary<string, Expression> expressions;

            public PreprocessorContext (Report report, Dictionary<string, Expression> expressions) : base (new MachineInfo (), report, null, null)
            {
                this.expressions = expressions;
            }

            public override ResolvedVariable ResolveVariable (string name, CType[] argTypes)
            {
                if (expressions.TryGetValue (name, out var expression)) {
                    // New context to prevent infinitie recursion
                    var nex = new Dictionary<string, Expression> (expressions);
                    var nctx = new PreprocessorContext (Report, nex);
                    var value = expression.EvalConstant (nctx);
                    return new ResolvedVariable (value, CBasicType.SignedInt);
                }
                return base.ResolveVariable (name, argTypes);
            }
        }

        static (List<Define> Defines, int TokenLength) ReadDefineArgs (int startIndex, List<Token> tokens)
        {
            var defines = new List<Define> ();

            if (startIndex < 0 || startIndex >= tokens.Count || tokens[startIndex].Kind != '(')
                return (defines, 0);

            int parenDepth = 0;
            var i = startIndex;
            var startArgIndex = startIndex + 1;
            for (; i < tokens.Count && startArgIndex > startIndex && tokens[i].Kind != TokenKind.EOL; i++) {
                switch (tokens[i].Kind) {
                    case '(':
                        parenDepth++;
                        break;
                    case ',':
                        if (parenDepth == 1) {
                            var body = tokens.Skip (startArgIndex).Take (i - startArgIndex).ToList ();
                            defines.Add (new Define { Body = body });
                            startArgIndex = i + 1;
                        }
                        break;
                    case ')':
                        parenDepth--;
                        if (parenDepth == 0) {
                            var body = tokens.Skip (startArgIndex).Take (i - startArgIndex).ToList ();
                            defines.Add (new Define { Body = body });
                            startArgIndex = -1;
                        }
                        break;
                }
            }

            return (defines, i - startIndex);
        }
    }
}
