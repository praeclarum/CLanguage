using System;
using CLanguage.Types;
using CLanguage.Interpreter;

namespace CLanguage.Compiler
{
    public class ResolvedVariable
    {
        public VariableScope Scope { get; }
        public int Address { get; }
        public CType VariableType { get; }
        public BaseFunction Function { get; }
        public Value Constant { get; }

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

        public ResolvedVariable (Value constantValue, CType variableType)
        {
            Scope = VariableScope.Constant;
            Constant = constantValue;
            Address = 0;
            VariableType = variableType;
        }

        public void EmitPointer (EmitContext ec)
        {
            switch (Scope) {
                case VariableScope.Function:
                    ec.Emit (OpCode.LoadConstant, Value.Pointer (Address));
                    break;
                case VariableScope.Global:
                    ec.Emit (OpCode.LoadConstant, Value.Pointer (Address));
                    break;
                case VariableScope.Arg:
                    ec.Emit (OpCode.LoadConstant, Value.Pointer (Address));
                    ec.Emit (OpCode.LoadFramePointer);
                    ec.Emit (OpCode.OffsetPointer);
                    break;
                case VariableScope.Local:
                    ec.Emit (OpCode.LoadConstant, Value.Pointer (Address));
                    ec.Emit (OpCode.LoadFramePointer);
                    ec.Emit (OpCode.OffsetPointer);
                    break;
                case VariableScope.Constant:
                    ec.Emit (OpCode.LoadConstant, 0);
                    break;
                default:
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
        Constant,
    }
}
