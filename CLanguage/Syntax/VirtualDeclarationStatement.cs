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
            // Class body declarations are processed by EmitContext.AddStructMember, not DoEmit
        }

        public override void AddDeclarationToBlock (BlockContext context)
        {
            // Class body declarations are processed by EmitContext.AddStructMember, not AddDeclarationToBlock
        }
    }
}
