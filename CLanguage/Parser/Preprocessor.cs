using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CLanguage.Syntax;

namespace CLanguage.Parser
{
    public class Preprocessor
    {
        readonly List<Token> tokens;

        private readonly Report report;

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

        public Preprocessor (Report report, params Token[][] tokens)
        {
            this.tokens = tokens.SelectMany (x => x).ToList ();
            this.report = report;
        }

        public Token[] Preprocess ()
        {
            var defines = new Dictionary<string, Define> ();
            while (PreprocessIteration (defines, tokens, report)) {
                // Keep going until nothing changes
            }
            return tokens.ToArray ();
        }

        static bool PreprocessIteration (Dictionary<string, Define> defines, List<Token> tokens, Report report)
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
                            while (PreprocessIteration (newDefines, newBody, report)) {
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
                else if (t.Kind == '#' && i + 1 < tokens.Count && tokens[i + 1].Kind == TokenKind.IDENTIFIER) {
                    var eol = i + 1;
                    while (eol < tokens.Count && tokens[eol].Kind != TokenKind.EOL) {
                        if (tokens[eol].Kind == '\\' && eol + 1 < tokens.Count && tokens[eol + 1].Kind == TokenKind.EOL) {
                            eol++;
                        }
                        eol++;
                    }
                    switch (tokens[i + 1].Value.ToString ()) {
                        case "define" when eol - i >= 2:
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
                            break;
                        default:
                            report.Warning (1024, tokens[i].Location, tokens[eol-1].EndLocation, "Cannot understand preprocessor");
                            break;
                    }
                    if (eol < tokens.Count)
                        eol++;
                    tokens.RemoveRange (i, eol - i);
                    anotherIterationNeeded = true;
                }
                else {
                    i++;
                }
            }

            return anotherIterationNeeded;
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
