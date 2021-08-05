using System;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class VisibilityStatement : Statement
    {
        public DeclarationsVisibility Visibility { get; }

        public override bool AlwaysReturns => throw new NotImplementedException ();

        public VisibilityStatement (DeclarationsVisibility visibility)
        {
            Visibility = visibility;
        }

        protected override void DoEmit (EmitContext ec)
        {
            // Nothing to do
        }

        public override void AddDeclarationToBlock (BlockContext context)
        {
            // Nothing to do
        }
    }

    public enum DeclarationsVisibility
    {
        Public,
        Private,
        Protected,
    }
}
