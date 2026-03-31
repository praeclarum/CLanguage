using System;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class LabeledStatement : Statement
    {
        public string Label { get; }
        public Statement Statement { get; }

        public LabeledStatement (string label, Statement statement, Location location)
        {
            Label = label;
            Statement = statement;
            Location = location;
        }

        public override bool AlwaysReturns => Statement.AlwaysReturns;

        public override void AddDeclarationToBlock (BlockContext context)
        {
            Statement.AddDeclarationToBlock (context);
        }

        protected override void DoEmit (EmitContext ec)
        {
            var label = ec.DefineGotoLabel (Label);
            if (label != null) {
                ec.EmitLabel (label);
            }
            Statement.Emit (ec);
        }
    }
}
