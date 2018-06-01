using System.Collections.Generic;

namespace CLanguage.Syntax
{
    public class FunctionDefinition : Statement
    {
        public DeclarationSpecifiers Specifiers { get; set; }
        public Declarator Declarator { get; set; }
        public List<Declaration> ParameterDeclarations { get; set; }
        public Block Body { get; set; }

        public override bool AlwaysReturns => false;

        protected override void DoEmit (EmitContext ec)
        {
            throw new System.NotImplementedException ();
        }
    }
}
