using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class CompilerContext
    {
        public Report Report { get; private set; }

		public MachineInfo MachineInfo { get; private set; }

        public CompilerContext(Report report, MachineInfo machineInfo)
        {
            Report = report;
			MachineInfo = machineInfo;
        }
    }
}
