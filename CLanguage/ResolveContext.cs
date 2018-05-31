using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Ast;

namespace CLanguage
{
    public class ResolveContext
    {
        public ResolveContext(CompilerContext c)
        {
            Compiler = c;
        }

        public CompilerContext Compiler { get; private set; }
		public MachineInfo MachineInfo { get { return Compiler.MachineInfo; } }

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
