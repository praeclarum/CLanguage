using System;
using CLanguage.Interpreter;
using CLanguage.Syntax;
using CLanguage.Types;

namespace CLanguage.Compiler
{
    public class BlockContext : EmitContext
    {
        public Block Block { get; }

        public BlockContext (Block block, EmitContext parentContext)
            : base (parentContext)
        {
            Block = block;
        }

        public BlockContext (Block block, MachineInfo machineInfo, Report report, CompiledFunction fdecl, EmitContext parentContext)
            : base (machineInfo, report, fdecl, parentContext)
        {
            Block = block;
        }

        public override ResolvedVariable? TryResolveVariable (string name, CType[]? argTypes)
        {
            foreach (var s in Block.Statements) {
                if (s is MultiDeclaratorStatement mds && mds.InitDeclarators != null) {
                    foreach (var i in mds.InitDeclarators) {
                        if (i.Declarator.DeclaredIdentifier == name) {
                            var type = MakeCType (mds.Specifiers, i.Declarator, i.Initializer, Block);
                            return new ResolvedVariable (Block.VariableScope, address: 0, variableType: type);
                        }
                    }
                }
            }
            return base.TryResolveVariable (name, argTypes);
        }
    }
}
