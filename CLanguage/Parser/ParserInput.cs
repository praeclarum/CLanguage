using System;
using CLanguage.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace CLanguage.Parser
{
    public class ParserInput : yyParser.yyInput
    {
        Token[] tokens;
        int index = -1;

        public ParserInput (params IEnumerable<Token>[] tokens)
        {
            this.tokens = tokens.SelectMany (x => x).ToArray ();
        }

        public bool advance ()
        {
            if (index + 1 < tokens.Length) {
                index++;
                return true;
            }
            return false;
        }

        public int token () => tokens[index].Kind;

        public object value () => tokens[index].Value;
    }
}
