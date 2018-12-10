using System.Collections.Generic;
using CLanguage.Compiler;

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
            // Emitted by the compiler
        }
    }
}
