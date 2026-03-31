using System;
using CLanguage.Compiler;
using CLanguage.Interpreter;

namespace CLanguage.Syntax
{
    public class GotoStatement : Statement
    {
        public string Label { get; }

        public GotoStatement (string label, Location location)
        {
            Label = label;
            Location = location;
        }

        public override bool AlwaysReturns => false;

        public override void AddDeclarationToBlock (BlockContext context)
        {
        }

        protected override void DoEmit (EmitContext ec)
        {
            var label = ec.ResolveGotoLabel (Label);
            if (label != null) {
                ec.Emit (OpCode.Jump, label);
            }
            else {
                ec.Report.Error (9999, $"goto statement used outside of function body");
            }
        }
    }
}
