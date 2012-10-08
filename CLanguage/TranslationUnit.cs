using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class TranslationUnit : Block
    {
        public TranslationUnit()
            : base(null, Location.Null)
        {
			LocalSymbolName = "_";
        }
    }
}
