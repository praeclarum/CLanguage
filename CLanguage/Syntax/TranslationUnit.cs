using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Syntax
{
    public class TranslationUnit : Block
    {
        public TranslationUnit ()
            : base (Location.Null, Location.Null)
        {
        }
    }
}
