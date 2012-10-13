using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ValueType = System.Int32;

namespace CLanguage
{
	public struct ExecutionFrame
	{
		public int IP;
		public IFunction Function;
	}

	public class ExecutionState
	{
		public readonly ValueType[] Stack;
		public readonly ExecutionFrame[] Frames;

		public int FP;
		public int SP;

		public int SleepTime;
		public int RemainingTime;

		public int CpuSpeed = 1000;

		public ExecutionState (int maxStack, int maxFrames)
		{
			Stack = new ValueType[maxStack];
			Frames = new ExecutionFrame[maxFrames];
		}
	}

    public class Interpreter
    {
		Executable exe;
		ExecutionState state;

        public Interpreter (Executable exe, int maxStack = 1024, int maxFrames = 24)
        {
			this.exe = exe;
			state = new ExecutionState (maxStack, maxFrames);
        }

		public void Reset (string entrypoint)
		{
			var f = exe.Functions.First (x => x.Name == entrypoint);
			state.FP = 0;
			state.SP = 0;
			state.Frames[0].Function = f;
			state.Frames[0].IP = 0;
			state.SleepTime = 0;
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
