using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class ResolveContext
    {
        public ResolveContext(CompilerContext c, MachineInfo m)
        {
            Compiler = c;
            MachineInfo = m;
        }

        public MachineInfo MachineInfo { get; private set; }

        public CompilerContext Compiler { get; private set; }

        public Report Report
        {
            get
            {
                return Compiler.Report;
            }
        }

        public virtual Expression ResolveVariable(string name, VariableExpression original)
        {
            return original;
        }
    }
}
