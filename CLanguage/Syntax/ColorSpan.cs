using System;
using CLanguage.Parser;

namespace CLanguage.Syntax
{
    public struct ColorSpan
    {
        public int Index;
        public int Length;
        public SyntaxColor Color;

        public override string ToString ()
        {
            return Color.ToString ();
        }
    }

    public enum SyntaxColor
    {
        Comment,
        Identifier,
        Number,
        String,
        Keyword,
        Operator,
        Function,
        Type,
    }
}
