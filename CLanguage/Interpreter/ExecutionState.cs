using System.Linq;

using ValueType = System.Int32;

namespace CLanguage.Interpreter
{
    public class ExecutionState
    {
        public readonly ValueType[] Stack;
        public int SP;
        readonly ExecutionFrame[] Frames;
        int FP;
        public int SleepTime { get; set; }
        public int RemainingTime { get; set; }

        Executable exe;
        public int CpuSpeed = 1000;

        public ExecutionFrame CallerFrame { get { return (0 <= (FP - 1) && (FP - 1) < Frames.Length) ? Frames[FP - 1] : null; } }
        public ExecutionFrame ActiveFrame { get { return (0 <= FP && FP < Frames.Length) ? Frames[FP] : null; } }

        public ExecutionState (Executable exe, int maxStack, int maxFrames)
        {
            this.exe = exe;
            Stack = new ValueType[maxStack];
            Frames = (from i in Enumerable.Range (0, maxFrames) select new ExecutionFrame ()).ToArray ();
        }

        public void Call (int functionAddress)
        {
            Call (exe.Functions[functionAddress]);
        }

        public void Call (BaseFunction function)
        {
            FP++;
            Frames[FP].Function = function;
            Frames[FP].IP = 0;

            var frame = ActiveFrame;

            var nargs = function.FunctionType.Parameters.Count;
            frame.AllocateArgs (nargs);
            var args = frame.Args;
            for (var i = nargs - 1; i >= 0; i--) {
                args[i] = Stack[SP - 1];
                SP--;
            }

            function.Init (this);
        }

        public void Return ()
        {
            FP--;
        }

        public void Reset ()
        {
            FP = -1;
            SP = exe.Globals.Count;
            SleepTime = 0;
        }
    }
}
