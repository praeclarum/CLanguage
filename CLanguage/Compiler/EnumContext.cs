using CLanguage.Syntax;
using CLanguage.Types;
using System.Linq;

namespace CLanguage.Compiler
{
    public class EnumContext : EmitContext
    {
        private TypeSpecifier enumTs;
        private CEnumType et;
        private EmitContext emitContext;

        public EnumContext (TypeSpecifier enumTs, CEnumType et, EmitContext parentContext)
            : base (parentContext)
        {
            this.enumTs = enumTs;
            this.et = et;
            this.emitContext = parentContext;
        }

        public override ResolvedVariable? TryResolveVariable (string name, CType[]? argTypes)
        {
            var r = et.Members.FirstOrDefault (x => x.Name == name);
            if (r != null) {
                return new ResolvedVariable ((Value)r.Value, et);
            }

            return base.TryResolveVariable (name, argTypes);
        }
    }
}
