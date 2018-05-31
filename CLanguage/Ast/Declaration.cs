using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Syntax
{
    public abstract class Declaration
    {
        public DeclarationSpecifiers Specifiers { get; set; }
        public Declarator Declarator { get; set; }
        public Initializer Initializer { get; set; }

        public Declaration ()
        {
        }

        public Declaration (DeclarationSpecifiers specs, Declarator decl, Initializer init)
        {
            Specifiers = specs;
            Declarator = decl;
            Initializer = init;
        }

        public void Emit(EmitContext ec)
        {
            DoEmit(ec);
        }

        protected abstract void DoEmit(EmitContext ec);
    }

    public class MultiDeclaration
    {
        public DeclarationSpecifiers Specifiers;
        public List<InitDeclarator> InitDeclarators;
    }
}
