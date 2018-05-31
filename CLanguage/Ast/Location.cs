using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Ast
{
    public class Location
    {
        public bool IsNull { get; private set; }

        Location (bool n)
        {
            IsNull = n;
        }

        public static readonly Location Null = new Location (true);
    }
}
