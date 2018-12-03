using System;
using System.Linq;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
    public class ExecutableContext : EmitContext
    {
        public Executable Executable { get; }

        public ExecutableContext (Executable executable, Report report)
            : base (executable.MachineInfo, report, null, null)
        {
            Executable = executable;
        }

        public override ResolvedVariable ResolveMethodFunction (CStructType structType, CStructMethod method)
        {
            if (method.MemberType is CFunctionType ftype) {

                var nameContext = structType.Name;

                var funcs = Executable.Functions.Select ((f, i) => (f, i)).Where (x => x.Item1.NameContext == nameContext && x.Item1.Name == method.Name);
                for (var i = 0; i < Executable.Functions.Count; i++) {
                    var f = Executable.Functions[i];
                    if (f.NameContext == nameContext && f.Name == method.Name && f.FunctionType.ParameterTypesEqual (ftype)) {
                        return new ResolvedVariable (f, i);
                    }
                }
            }

            return base.ResolveMethodFunction (structType, method);
        }

        public override ResolvedVariable ResolveVariable (string name, CType[] argTypes)
        {
            //
            // Look for global variables
            //
            foreach (var g in Executable.Globals) {
                if (g.Name == name) {
                    return new ResolvedVariable (VariableScope.Global, g.Offset, g.VariableType);
                }
            }

            //
            // Look for global functions
            //
            BaseFunction ff = null;
            var fi = -1;
            var fs = 0;
            for (var i = 0; i < Executable.Functions.Count; i++) {
                var f = Executable.Functions[i];
                if (f.Name == name && string.IsNullOrEmpty (f.NameContext)) {
                    var score = f.FunctionType.ScoreParameterTypeMatches (argTypes);
                    if (score > fs) {
                        ff = f;
                        fi = i;
                        fs = score;
                    }
                }
            }
            if (ff != null) {
                return new ResolvedVariable (ff, fi);
            }

            return base.ResolveVariable (name, argTypes);
        }
    }
}
