
using ValueType = System.Int32;

namespace CLanguage.Interpreter
{
    public class ExecutionFrame
    {
        public int IP { get; set; }
        public BaseFunction Function { get; set; }
        public ValueType[] Args { get; private set; }
        public ValueType[] Locals { get; private set; }

        public void AllocateArgs (int count)
        {
            if (Args == null || Args.Length < count) {
                Args = new ValueType[count];
            }
        }

        public void AllocateLocals (int count)
        {
            if (Locals == null || Locals.Length < count) {
                Locals = new ValueType[count];
            }
        }
    }
}
