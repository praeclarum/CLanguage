using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class VirtualDeclarationStatement : Statement
    {
        public Statement InnerDeclaration { get; }
        public bool IsVirtual { get; set; }
        public bool IsOverride { get; set; }
        public bool IsPureVirtual { get; set; }

        public override bool AlwaysReturns => false;

        public VirtualDeclarationStatement (Statement innerDeclaration)
        {
            InnerDeclaration = innerDeclaration;
        }

        protected override void DoEmit (EmitContext ec)
        {
            // Nothing to do at this phase — codegen is Phase 3
        }

        public override void AddDeclarationToBlock (BlockContext context)
        {
            // Nothing to do at this phase — codegen is Phase 3
        }
    }
}
