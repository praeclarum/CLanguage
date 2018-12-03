using System;
using CLanguage.Syntax;
using CLanguage.Types;

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
    }
}
