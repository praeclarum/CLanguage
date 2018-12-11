using System;
using System.Collections.Generic;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class FunctionDefinition : Statement
    {
        public FunctionDefinition (DeclarationSpecifiers specifiers, Declarator declarator, List<Declaration>? parameterDeclarations, Block body)
        {
            Specifiers = specifiers ?? throw new ArgumentNullException (nameof (specifiers));
            Declarator = declarator ?? throw new ArgumentNullException (nameof (declarator));
            ParameterDeclarations = parameterDeclarations;
            Body = body ?? throw new ArgumentNullException (nameof (body));
        }

        public DeclarationSpecifiers Specifiers { get; set; }
        public Declarator Declarator { get; set; }
        public List<Declaration>? ParameterDeclarations { get; set; }
        public Block Body { get; set; }

        public override bool AlwaysReturns => false;

        protected override void DoEmit (EmitContext ec)
        {
            // Emitted by the compiler
        }
    }
}
