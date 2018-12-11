using System.Linq;
using System.Text;

namespace CLanguage.Syntax
{
    public abstract class Declaration : Statement
    {
        public DeclarationSpecifiers Specifiers { get; set; }
        public Declarator Declarator { get; set; }
        public Initializer Initializer { get; set; }

        public override bool AlwaysReturns => false;

        protected Declaration (DeclarationSpecifiers specs, Declarator decl, Initializer init)
        {
            Specifiers = specs;
            Declarator = decl;
            Initializer = init;
        }
    }
}
