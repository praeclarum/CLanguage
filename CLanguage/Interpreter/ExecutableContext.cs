using System;

namespace CLanguage.Interpreter
{
    public class ExecutableContext : EmitContext
    {
        public ExecutableContext (MachineInfo machineInfo, Report report)
            : base (machineInfo, report, null, null)
        {
        }
    }
}
