using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ValueType = System.Int32;

namespace CLanguage
{
	public class ExecutionException : Exception
	{
		public ExecutionException (string message)
			: base (message)
		{
		}
	}

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
		public ExecutionFrame ActiveFrame { get { return FP < 0 ? null : Frames[FP]; } }

		public ExecutionState (Executable exe, int maxStack, int maxFrames)
		{
			this.exe = exe;
			Stack = new ValueType[maxStack];
			Frames = (from i in Enumerable.Range (0, maxFrames) select new ExecutionFrame ()).ToArray ();
		}

		public void Call (int functionAddress)
		{
			Call (exe.Functions [functionAddress]);
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
				args[i] = Stack[SP-1];
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

    public class Interpreter
    {
		Executable exe;
		ExecutionState state;

        public Interpreter (Executable exe, int maxStack = 1024, int maxFrames = 24)
        {
			this.exe = exe;
			state = new ExecutionState (exe, maxStack, maxFrames);
        }

		public void Reset (string entrypoint)
		{
			state.Reset ();

			var f = exe.Functions.First (x => x.Name == entrypoint);
			state.Call (f);
		}

		public void Step ()
		{
			Step (1000000);
		}

		public void Step (int microseconds)
		{
			if (state.ActiveFrame == null)
				return;

			if (microseconds <= state.SleepTime) {
				state.SleepTime -= microseconds;
			} else {
				state.RemainingTime = microseconds - state.SleepTime;
				state.SleepTime = 0;

				while (state.RemainingTime > 0 && state.ActiveFrame != null) {
					state.ActiveFrame.Function.Step (state);
				}
			}
		}
    }
}
