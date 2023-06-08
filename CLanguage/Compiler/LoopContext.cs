using System;
using CLanguage.Interpreter;
using CLanguage.Types;

namespace CLanguage.Compiler
{
    public class LoopContext : EmitContext
    {
        public Label LoopBreakLabel { get; }
        public Label? LoopContinueLabel { get; }

        public override Label? BreakLabel => LoopBreakLabel;
        public override Label? ContinueLabel => LoopContinueLabel ?? ParentContext?.ContinueLabel;

        public LoopContext (Label breakLabel, Label? continueLabel, EmitContext parentContext)
            : base (parentContext)
        {
            LoopBreakLabel = breakLabel ?? throw new ArgumentNullException (nameof (breakLabel));
            LoopContinueLabel = continueLabel;
        }
    }
}
