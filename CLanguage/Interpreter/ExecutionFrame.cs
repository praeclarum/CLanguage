

namespace CLanguage.Interpreter
{
    public class ExecutionFrame
    {
        public int FP { get; set; }
        public int IP { get; set; }
        public BaseFunction Function { get; set; }

        public ExecutionFrame (BaseFunction function)
        {
            Function = function ?? throw new System.ArgumentNullException (nameof (function));
        }
    }
}
