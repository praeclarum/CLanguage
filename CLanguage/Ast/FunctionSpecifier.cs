using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Ast
{
    [Flags]
    public enum FunctionSpecifier
    {
        None = 0,
        Inline = 1
    }
}
