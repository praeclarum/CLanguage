

namespace CLanguage.Interpreter
{
    public class ExecutionFrame
    {
        public int IP { get; set; }
        public BaseFunction Function { get; set; }
        public Value[] Args { get; private set; }
        public Value[] Locals { get; private set; }

        public void AllocateArgs (int numValues)
        {
            if (Args == null || Args.Length < numValues) {
                Args = new Value[numValues];
            }
        }

        public void AllocateLocals (int numValues)
        {
            if (Locals == null || Locals.Length < numValues) {
                Locals = new Value[numValues];
            }
        }
    }
}
