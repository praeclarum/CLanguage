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

		public CompilerContext (MachineInfo machineInfo, Report report)
        {
			if (machineInfo == null) throw new ArgumentNullException ("machineInfo");
			if (report == null) throw new ArgumentNullException ("report");

			MachineInfo = machineInfo;
            Report = report;
        }
    }
}
