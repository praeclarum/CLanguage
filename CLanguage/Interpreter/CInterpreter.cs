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
        int FI;
        public int SleepTime { get; set; }
        public int RemainingTime { get; set; }
		
        public int CpuSpeed = 1000;

        public ExecutionFrame CallerFrame { get { return (0 <= (FI - 1) && (FI - 1) < Frames.Length) ? Frames[FI - 1] : null; } }
        public ExecutionFrame ActiveFrame { get { return (0 <= FI && FI < Frames.Length) ? Frames[FI] : null; } }

        public CInterpreter (Executable exe, int maxStack = 1024, int maxFrames = 24)
        {
            this.exe = exe;
            Stack = new Value[maxStack];
            Frames = (from i in Enumerable.Range (0, maxFrames) select new ExecutionFrame ()).ToArray ();
        }

        public Value ReadRelativeMemory (int frameOffset)
        {
            var address = ActiveFrame.FP + frameOffset;
            return Stack[address];
        }

        public Value ReadMemory (int address)
        {
            return Stack[address];
        }

        public Value ReadArg (int index)
        {
            var frame = ActiveFrame;
            var functionType = frame.Function.FunctionType;
            int frameOffset;
            if (index < functionType.Parameters.Count) {
                frameOffset = functionType.Parameters[index].Offset;
            }
            else if (index == functionType.Parameters.Count && functionType.IsInstance) {
                frameOffset = -1;
            }
            else {
                throw new ArgumentOutOfRangeException ("Cannot read argument #" + index);
            }
            return ReadRelativeMemory (frameOffset);
        }

        public void Call (Value functionAddress)
        {
            if (functionAddress.Type != ValueType.FunctionPointer) {
                throw new Exception ($"Cannot call {functionAddress.Type}");
            }
            Call (exe.Functions[functionAddress.PointerValue]);
        }

        public void Call (BaseFunction function)
        {
            if (FI + 1 >= Frames.Length) {
                var name = function.Name;
                var cname = ActiveFrame != null ? ActiveFrame.Function.Name : "?";
                Reset ();
                throw new ExecutionException ("Stack overflow while calling '" + name + "' from '" + cname + "'");
            }

            FI++;
            var frame = ActiveFrame;

            frame.Function = function;
            frame.FP = SP;
            frame.IP = 0;

            function.Init (this);
        }

        public void Push (Value value)
        {
            Stack[SP++] = value;
        }

        public void Return ()
        {
            //
            // Pop the stack
            //
            var frame = ActiveFrame;
            var ftype = frame.Function.FunctionType;
            var numArgsAndLocals = 0;
            foreach (var p in ftype.Parameters) {
                numArgsAndLocals += p.ParameterType.NumValues;
            }
            if (ftype.IsInstance) {
                numArgsAndLocals++;
            }
            if (frame.Function is CompiledFunction cf) {
                foreach (var v in cf.LocalVariables) {
                    numArgsAndLocals += v.VariableType.NumValues;
                }
            }

            var numReturnVals = ftype.ReturnType.NumValues;
            var newSP = SP - numArgsAndLocals;
            var retSP = newSP - numReturnVals;
            for (var i = 0; i < numReturnVals; i++) {
                Stack[retSP + i] = Stack[SP - numReturnVals + i];
            }
            SP = newSP;

            //
            // Pop the frame stack
            //
            FI--;
        }

		public void Reset (string entrypoint)
		{
			this.entrypoint = exe.Functions.FirstOrDefault (x => x.Name == entrypoint);
			Reset ();
		}

		void Reset ()
		{
            FI = -1;
            SP = 0;
            foreach (var g in exe.Globals) {
                if (g.InitialValue != null) {
                    for (var i = 0; i < g.InitialValue.Length; i++) {
                        Stack[g.Offset + i] = g.InitialValue[i];
                    }
                }
                SP += g.VariableType.NumValues;
            }
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
				catch (Exception) {
					Reset ();
					throw;
				}
			}
		}
    }
}
