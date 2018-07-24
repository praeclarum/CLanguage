

namespace CLanguage.Interpreter
{
    public class ExecutionFrame
    {
        public int IP { get; set; }
        public BaseFunction Function { get; set; }
        public Value[] Args { get; private set; }
        public Value[] Locals { get; private set; }

        public void AllocateArgs (int count)
        {
            if (Args == null || Args.Length < count) {
                Args = new Value[count];
            }
        }

        public void AllocateLocals (int count)
        {
            if (Locals == null || Locals.Length < count) {
                Locals = new Value[count];
            }
        }
    }
}
