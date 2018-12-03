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

        readonly Dictionary<string, Define> defines = new Dictionary<string, Define> ();
        class Define
        {
            public string Name;
            //public string[] Parameters;
            public List<Token> Body;
            public override string ToString ()
            {
                return Name + ": [" + string.Join (", ", Body) + "]";
            }
        }

        public Preprocessor (params IEnumerable<Token>[] tokens)
        {
            this.tokens = tokens.SelectMany (x => x).ToList ();
        }

        public Token[] Preprocess ()
        {
            while (PreprocessIteration ()) {
                // Keep going until nothing changes
            }
            return tokens.ToArray ();
        }

        bool PreprocessIteration ()
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
                        tokens.RemoveAt (i);
                        tokens.InsertRange (i, define.Body);
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
                                    var define = new Define {
                                        Name = tokens[i + 2].Value?.ToString (),
                                        Body = tokens.Skip (i + 3).Take (eol - i - 3).ToList (),
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
    }
}
