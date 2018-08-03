

namespace CLanguage.Interpreter
{
    public class ExecutionFrame
    {
        public int FP { get; set; }
        public int IP { get; set; }
        public BaseFunction Function { get; set; }
    }
}
