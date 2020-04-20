using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Interpreter
{
    public class CInterpreter
    {
		Executable exe;
        BaseFunction? entrypoint;

        public Executable Executable => exe;

        public readonly Value[] Stack;
        public int SP;
        readonly ExecutionFrame[] Frames;
        int FI;
        public int YieldedValue { get; private set; }
        public int SleepTime { get; set; }
        public int RemainingTime { get; set; }
		
        public int CpuSpeed = 1000;

        public ExecutionFrame? ActiveFrame { get { return (0 <= FI && FI < Frames.Length) ? Frames[FI] : null; } }

        static readonly BaseFunction unusedStackFrameFunction = new InternalFunction ("unused", "", Types.CFunctionType.VoidProcedure);


        public CInterpreter (Executable exe, int maxStack = 1024, int maxFrames = 24)
        {
            this.exe = exe;
            Stack = new Value[maxStack];
            Frames = (from i in Enumerable.Range (0, maxFrames) select new ExecutionFrame (unusedStackFrameFunction)).ToArray ();
        }

        public Value ReadMemory (int address)
        {
            return Stack[address];
        }

        public string ReadStringWithEncoding (int address, Encoding encoding)
        {
            var b = (byte)Stack[address];
            var bytes = new List<byte> ();
            while (b != 0) {
                bytes.Add (b);
                address++;
                b = (byte)Stack[address];
            }
            var ar = bytes.ToArray ();
            return encoding.GetString (ar, 0, ar.Length);
        }

        public string ReadString (int address)
        {
            return ReadStringWithEncoding (address, Encoding.UTF8);
        }

        public Value ReadArg (int index)
        {
            var frame = ActiveFrame;
            if (frame == null)
                return 0;
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
            var address = frame.FP + frameOffset;
            return Stack[address];
        }

        public void Call (Value functionAddress)
        {
            Call (exe.Functions[functionAddress.PointerValue]);
        }

        public void Call (BaseFunction function)
        {
            if (FI + 1 >= Frames.Length) {
                var name = function.Name;
                var cname = ActiveFrame?.Function.Name ?? "?";
                Reset ();
                throw new ExecutionException ("Stack overflow while calling '" + name + "' from '" + cname + "'");
            }

            FI++;
            var frame = Frames[FI];

            frame.Function = function;
            frame.FP = SP;
            frame.IP = 0;

            function.Init (this);
        }

        public void Push (Value value)
        {
            Stack[SP++] = value;
        }

        public void Yield (int yieldedValue)
        {
            YieldedValue = yieldedValue;
        }

        public void Return ()
        {
            //
            // Pop the stack
            //
            var frame = ActiveFrame;
            if (frame == null)
                throw new InvalidOperationException ($"Cannot call Return with no ActiveFrame");
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
                        Stack[g.StackOffset + i] = g.InitialValue[i];
                    }
                }
                SP += g.VariableType.NumValues;
            }
            SleepTime = 0;
			if (entrypoint != null) {
				Call (entrypoint);
			}
		}

        public void Run ()
        {
            Step (1000000);
        }

        public static void Run (string code)
        {
            var exe = Compiler.CCompiler.Compile (code);
            var interpreter = new CInterpreter (exe);
            interpreter.Reset ("main");
            interpreter.Run ();
        }

        [Obsolete]
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
                    var a = ActiveFrame;
					while (a != null && RemainingTime > 0) {
                        RemainingTime -= CpuSpeed;
						a.Function.Step (this, a);
                        a = ActiveFrame;
                        if (YieldedValue != 0)
                            break;
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
