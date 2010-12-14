using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class Interpreter
    {
        CompilerContext _c;
        MachineInfo _m;

        public Interpreter(CompilerContext c, MachineInfo m)
        {
            _c = c;
            _m = m;
            Reset();
        }

        public void Reset()
        {
        }
    }
}
