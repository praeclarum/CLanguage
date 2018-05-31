using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Interpreter
{
    public class CInterpreter
    {
		Executable exe;
		ExecutionState state;

		BaseFunction entrypoint;
		//BaseFunction loop;

        public CInterpreter (Executable exe, int maxStack = 1024, int maxFrames = 24)
        {
			this.exe = exe;
			state = new ExecutionState (exe, maxStack, maxFrames);
        }

		public void Reset (string entrypoint, string loop = null)
		{
			this.entrypoint = exe.Functions.FirstOrDefault (x => x.Name == entrypoint);
			Reset ();
		}

		void Reset ()
		{
			state.Reset ();
			if (entrypoint != null) {
				state.Call (entrypoint);
			}
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

				try {
					while (state.RemainingTime > 0 && state.ActiveFrame != null) {
						state.ActiveFrame.Function.Step (state);
					}
				}
				catch (IndexOutOfRangeException ex) {
					var cname = state.CallerFrame != null ? state.CallerFrame.Function.Name : "?";
					var name = state.ActiveFrame != null ? state.ActiveFrame.Function.Name : "?";
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
