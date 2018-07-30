using System;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
    public class ResolvedVariable
    {
        public VariableScope Scope { get; private set; }
        public int Index { get; private set; }
        public CType VariableType { get; private set; }
        public BaseFunction Function { get; private set; }

        public ResolvedVariable (VariableScope scope, int index, CType variableType)
        {
            Scope = scope;
            Index = index;
            VariableType = variableType;
        }

        public ResolvedVariable (BaseFunction function, int index)
        {
            Scope = VariableScope.Function;
            Function = function;
            Index = index;
            VariableType = Function.FunctionType;
        }

        public void Emit (EmitContext ec)
        {
            if (Scope == VariableScope.Function) {
                ec.Emit (OpCode.LoadValue, Value.FunctionPointer (Index));
            }
            else if (Scope == VariableScope.Arg) {
                ec.Emit (OpCode.LoadValue, Value.ArgPointer (Index));
            }
            else if (Scope == VariableScope.Global) {
                ec.Emit (OpCode.LoadValue, Value.GlobalPointer (Index));
            }
            else if (Scope == VariableScope.Local) {
                ec.Emit (OpCode.LoadValue, Value.LocalPointer (Index));
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
