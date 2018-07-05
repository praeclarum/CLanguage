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
    }

    public enum VariableScope
    {
        Global,
        Arg,
        Local,
        Function,
    }
}
