using System;
using CLanguage.Interpreter;
using CLanguage.Types;

namespace CLanguage.Compiler
{
    public class LoopContext : EmitContext
    {
        public Label BreakLabel { get; }
        public Label ContinueLabel { get; }

        public LoopContext (Label breakLabel, Label continueLabel, EmitContext parentContext)
            : base (parentContext)
        {
            BreakLabel = breakLabel ?? throw new ArgumentNullException (nameof (breakLabel));
            ContinueLabel = continueLabel ?? throw new ArgumentNullException (nameof (continueLabel));
        }

        public override ResolvedVariable? TryResolveVariable (string name, CType[]? argTypes)
        {
            return base.TryResolveVariable (name, argTypes);
        }
    }
}
