using System;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
    public class ResolvedVariable
    {
        public VariableScope Scope { get; private set; }
        public int Address { get; private set; }
        public CType VariableType { get; private set; }
        public BaseFunction Function { get; private set; }

        public ResolvedVariable (VariableScope scope, int address, CType variableType)
        {
            Scope = scope;
            Address = address;
            VariableType = variableType;
        }

        public ResolvedVariable (BaseFunction function, int address)
        {
            Scope = VariableScope.Function;
            Function = function;
            Address = address;
            VariableType = Function.FunctionType;
        }

        public void Emit (EmitContext ec)
        {
            if (Scope == VariableScope.Function) {
                ec.Emit (OpCode.LoadConstant, Value.FunctionPointer (Address));
            }
            else if (Scope == VariableScope.Global) {
                ec.Emit (OpCode.LoadConstant, Value.GlobalPointer (Address));
            }
            else if (Scope == VariableScope.Arg) {
                ec.Emit (OpCode.LoadConstant, Value.ArgPointer (Address));
                ec.Emit (OpCode.LoadFramePointer);
                ec.Emit (OpCode.OffsetPointer);
            }
            else if (Scope == VariableScope.Local) {
                ec.Emit (OpCode.LoadConstant, Value.LocalPointer (Address));
                ec.Emit (OpCode.LoadFramePointer);
                ec.Emit (OpCode.OffsetPointer);
            }
            else {
                throw new NotSupportedException ("Cannot get address of variable scope '" + Scope + "'");
            }
        }
    }

    public enum VariableScope
    {
        Global,
        Arg,
        Local,
        Function,
    }
}
