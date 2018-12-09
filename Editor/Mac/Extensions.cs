using System;

namespace CLanguage.Editor
{
    static class Extensions
    {
        public static string GetIndent (this string line)
        {
            var e = 0;
            while (e < line.Length && char.IsWhiteSpace (line[e]))
                e++;
            if (e == 0)
                return string.Empty;
            return line.Substring (0, e);
        }
    }
}
