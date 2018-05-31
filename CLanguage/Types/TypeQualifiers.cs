using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Types
{
    [Flags]
    public enum TypeQualifiers
    {
        None = 0,
        Const = 1,
        Restrict = 2,
        Volatile = 4,
    }
}
