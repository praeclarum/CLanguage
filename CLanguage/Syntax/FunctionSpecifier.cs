using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Syntax
{
    [Flags]
    public enum FunctionSpecifier
    {
        None = 0,
        Inline = 1,
        Virtual = 2 // Added for virtual methods
    }
}
