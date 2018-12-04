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

        public Preprocessor (Report report, params IEnumerable<Token>[] tokens)
        {
            this.tokens = tokens.SelectMany (x => x).ToList ();
            this.report = report;
        }

        public Token[] Preprocess ()
        {
            var defines = new Dictionary<string, Define> ();
            while (PreprocessIteration (defines, tokens)) {
                // Keep going until nothing changes
            }
            return tokens.ToArray ();
        }

        static bool PreprocessIteration (Dictionary<string, Define> defines, List<Token> tokens)
        {
            var changed = false;

            var i = 0;
            while (i < tokens.Count) {
                var t = tokens[i];
                if (t.Kind == TokenKind.EOL) {
                    tokens.RemoveAt (i);
                    changed = true;
                }
                else if (t.Kind == TokenKind.IDENTIFIER) {
                    var ident = t.Value.ToString ();
                    if (defines.TryGetValue (ident, out var define)) {
                        if (define.HasParameters) {
                            var (args, len) = ReadDefineArgs (i + 1, tokens);
                            var newDefines = new Dictionary<string, Define> (defines);
                            for (var ai = 0; ai < Math.Min (args.Count, define.Parameters.Length); ai++) {
                                args[ai].Name = define.Parameters[ai];
                                newDefines[args[ai].Name] = args[ai];
                            }
                            var newBody = define.Body.ToList ();
                            while (PreprocessIteration (newDefines, newBody)) {
                                // Do as much as we can
                            }
                            tokens.RemoveRange (i, len + 1);
                            tokens.InsertRange (i, newBody);
                            //report.Error (9000, "PARAMETERIZED DEFINE");
                        }
                        else {
                            tokens.RemoveAt (i);
                            tokens.InsertRange (i, define.Body);
                        }
                        changed = true;
                    }
                    else {
                        i++;
                    }
                }
                else if (t.Kind == '#' && i + 1 < tokens.Count && tokens[i + 1].Kind == TokenKind.IDENTIFIER) {
                    switch (tokens[i + 1].Value.ToString ()) {
                        case "define": {
                                var eol = i + 1;
                                while (eol < tokens.Count && tokens[eol].Kind != TokenKind.EOL) {
                                    // Look for end of line
                                    eol++;
                                }
                                if (eol - i >= 2) {
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
                                if (eol < tokens.Count)
                                    eol++;
                                Console.WriteLine ("EOL " + eol);
                                changed = true;
                                tokens.RemoveRange (i, eol - i);
                            }
                            break;
                        default:
                            i++;
                            break;
                    }
                }
                else {
                    i++;
                }
            }

            return changed;
        }

        static (List<Define> Defines, int TokenLength) ReadDefineArgs (int startIndex, List<Token> tokens)
        {
            var defines = new List<Define> ();

            if (startIndex < 0 || startIndex >= tokens.Count || tokens[startIndex].Kind != '(')
                return (defines, 0);

            int parenDepth = 0;
            var i = startIndex;
            var startArgIndex = startIndex + 1;
            for (; i < tokens.Count && startArgIndex > startIndex; i++) {
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
