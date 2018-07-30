using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Interpreter
{
    public class CInterpreter
    {
		Executable exe;
        BaseFunction entrypoint;

        public readonly Value[] Stack;
        public int SP;
        readonly ExecutionFrame[] Frames;
        int FP;
        public int SleepTime { get; set; }
        public int RemainingTime { get; set; }
		
        public int CpuSpeed = 1000;

        public ExecutionFrame CallerFrame { get { return (0 <= (FP - 1) && (FP - 1) < Frames.Length) ? Frames[FP - 1] : null; } }
        public ExecutionFrame ActiveFrame { get { return (0 <= FP && FP < Frames.Length) ? Frames[FP] : null; } }

        public CInterpreter (Executable exe, int maxStack = 1024, int maxFrames = 24)
        {
            this.exe = exe;
            Stack = new Value[maxStack];
            Frames = (from i in Enumerable.Range (0, maxFrames) select new ExecutionFrame ()).ToArray ();
        }

        public void Call (Value functionAddress)
        {
            if (functionAddress.Type != ValueType.FunctionPointer) {
                throw new Exception ($"Cannot call {functionAddress.Type}");
            }
            Call (exe.Functions[functionAddress.PointerValue.Index]);
        }

        public void Call (BaseFunction function)
        {
            FP++;
            Frames[FP].Function = function;
            Frames[FP].IP = 0;

            var frame = ActiveFrame;

            var functionType = function.FunctionType;
            var nargs = functionType.Parameters.Count + (functionType.IsInstance ? 1 : 0);
            frame.AllocateArgs (nargs);
            var args = frame.Args;
            for (var i = nargs - 1; i >= 0; i--) {
                args[i] = Stack[SP - 1];
                SP--;
            }

            function.Init (this);
        }

        public void Push (Value value)
        {
            Stack[SP++] = value;
        }

        public void Return ()
        {
            FP--;
        }

		public void Reset (string entrypoint)
		{
			this.entrypoint = exe.Functions.FirstOrDefault (x => x.Name == entrypoint);
			Reset ();
		}

		void Reset ()
		{
            FP = -1;
            SP = exe.Globals.Count;
            SleepTime = 0;
			if (entrypoint != null) {
				Call (entrypoint);
			}
		}

		public void Step ()
		{
			Step (1000000);
		}

		public void Step (int microseconds)
		{
			if (ActiveFrame == null)
				return;

			if (microseconds <= SleepTime) {
				SleepTime -= microseconds;
			} else {
				RemainingTime = microseconds - SleepTime;
				SleepTime = 0;

				try {
					while (RemainingTime > 0 && ActiveFrame != null) {
						ActiveFrame.Function.Step (this);
					}
				}
				catch (IndexOutOfRangeException ex) {
					var cname = CallerFrame != null ? CallerFrame.Function.Name : "?";
					var name = ActiveFrame != null ? ActiveFrame.Function.Name : "?";
					Reset ();
					throw new ExecutionException ("Stack overflow while executing '" + name + "' from '" + cname + "'", ex);
				}
				catch (Exception) {
					Reset ();
					throw;
				}
			}
		}
    }
}
