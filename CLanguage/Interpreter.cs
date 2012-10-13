using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ValueType = System.Int32;

namespace CLanguage
{
	public class ExecutionFrame
	{
		public int IP;
		public BaseFunction Function;
		public ValueType[] Locals;
	}

	public class ExecutionState
	{
		Executable exe;

		public readonly ValueType[] Stack;
		public readonly ExecutionFrame[] Frames;

		public int FP;
		public int SP;

		public int SleepTime;
		public int RemainingTime;

		public int CpuSpeed = 1000;

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
			function.Init (this);
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
			state.FP = 0;
			state.SP = 0;
			state.SleepTime = 0;

			var f = exe.Functions.First (x => x.Name == entrypoint);
			state.Call (f);
		}

		public void Step ()
		{
			Step (1000000);
		}

		public void Step (int microseconds)
		{
			if (state.FP < 0)
				return;
			if (state.FP >= state.Frames.Length)
				return;

			if (microseconds <= state.SleepTime) {
				state.SleepTime -= microseconds;
			} else {
				state.RemainingTime = microseconds - state.SleepTime;
				state.SleepTime = 0;

				while (state.RemainingTime > 0) {
					state.Frames [state.FP].Function.Step (state);
				}
			}
		}
    }
}
