using System;
using CLanguage.Syntax;
using CLanguage.Types;
using System.Linq;

namespace CLanguage.Interpreter
{
    public class TranslationUnitContext : EmitContext
    {
        public TranslationUnit TranslationUnit { get; }

        public TranslationUnitContext (TranslationUnit translationUnit, ExecutableContext exeContext)
            : base (exeContext.MachineInfo, exeContext.Report, null, exeContext)
        {
            TranslationUnit = translationUnit;
        }

        public override CType ResolveTypeName (string typeName)
        {
            if (TranslationUnit.Typedefs.TryGetValue (typeName, out var t))
                return t;
            return base.ResolveTypeName (typeName);
        }

        public override ResolvedVariable ResolveVariable (string name, CType[] argTypes)
        {
            foreach (var e in TranslationUnit.Enums) {
                var em = e.Value.Members.FirstOrDefault (x => x.Name == name);
                if (em != null) {
                    return new ResolvedVariable (em.Value, e.Value);
                }
            }
            return base.ResolveVariable (name, argTypes);
        }
    }
}
