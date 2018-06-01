using System;
using System.Collections.Generic;
using System.IO;
using CLanguage.Syntax;
using CLanguage.Types;
using StackValue = System.Int32;

namespace CLanguage.Interpreter
{
	public class CompiledFunction : BaseFunction
	{
        public Block Body { get; private set; }

        public List<VariableDeclaration> LocalVariables { get; private set; }
        public List<Instruction> Instructions { get; private set; }

        public CompiledFunction (string name, CFunctionType functionType, Block body = null)
		{
			Name = name;
			FunctionType = functionType;
            Body = body;
			LocalVariables = new List<VariableDeclaration> ();
			Instructions = new List<Instruction> ();
		}

		public override string ToString ()
		{
			return Name;
		}

		public string Assembler {
			get {
				var w = new StringWriter ();
				for (var i = 0; i < Instructions.Count; i++) {
					w.WriteLine ("{0}: {1}", i, Instructions[i]);
				}
				return w.ToString ();
			}
		}

		public override void Init (ExecutionState state)
		{
			state.ActiveFrame.AllocateLocals (LocalVariables.Count);
		}

		public override void Step (ExecutionState state)
		{
			var frame = state.ActiveFrame;
			var ip = frame.IP;
			var locals = frame.Locals;
			var args = frame.Args;

			var done = false;

			var a = 0;
			var b = 0;

			while (!done && ip < Instructions.Count && state.RemainingTime > 0) {

				var i = Instructions[ip];

				switch (i.Op) {
				case OpCode.Dup:
					state.Stack[state.SP] = state.Stack[state.SP - 1];
					state.SP++;
					ip++;
					break;
				case OpCode.Pop:
					state.SP--;
					ip++;
					break;
				case OpCode.Jump:
					ip = i.Label.Index;
					break;
				case OpCode.BranchIfFalse:
					a = state.Stack[state.SP - 1];
					state.SP--;
					if (a == 0) {
						ip = i.Label.Index;
					}
					else {
						ip++;
					}
					break;
				case OpCode.Call:
					a = state.Stack[state.SP - 1];
					state.SP--;
					ip++;
					state.Call (a);
					done = true;
					break;
				case OpCode.Return:
					state.Return ();
					done = true;
					break;
				case OpCode.LoadFunction:
					state.Stack[state.SP] = i.X;
					state.SP++;
					ip++;
					break;
				case OpCode.LoadValue:
					state.Stack[state.SP] = i.X;
					state.SP++;
					ip++;
					break;
				case OpCode.LoadMemory:
					state.Stack[state.SP] = state.Stack [i.X];
					state.SP++;
					ip++;
					break;
				case OpCode.StoreMemory:
					state.Stack [i.X] = state.Stack [state.SP - 1];
					state.SP--;
					ip++;
					break;
				case OpCode.LoadArg:
					state.Stack[state.SP] = args [i.X];
					state.SP++;
					ip++;
					break;
				case OpCode.StoreArg:
					args [i.X] = state.Stack [state.SP - 1];
					state.SP--;
					ip++;
					break;
				case OpCode.LoadLocal:
					state.Stack[state.SP] = locals [i.X];
					state.SP++;
					ip++;
					break;
				case OpCode.StoreLocal:
					locals[i.X] = state.Stack [state.SP - 1];
					state.SP--;
					ip++;
					break;
				case OpCode.AddInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((short)a + (short)b);
					state.SP--;
					ip++;
					break;
				case OpCode.AddUInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((ushort)a + (ushort)b);
					state.SP--;
					ip++;
					break;
				case OpCode.AddInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((int)a + (int)b);
					state.SP--;
					ip++;
					break;
				case OpCode.AddUInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = (StackValue)((uint)a + (uint)b);
					state.SP--;
					ip++;
					break;
				case OpCode.SubtractInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((short)a - (short)b);
					state.SP--;
					ip++;
					break;
				case OpCode.SubtractUInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((ushort)a - (ushort)b);
					state.SP--;
					ip++;
					break;
				case OpCode.SubtractInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((int)a - (int)b);
					state.SP--;
					ip++;
					break;
				case OpCode.SubtractUInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = (StackValue)((uint)a - (uint)b);
					state.SP--;
					ip++;
					break;
				case OpCode.MultiplyInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((short)a * (short)b);
					state.SP--;
					ip++;
					break;
				case OpCode.MultiplyUInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((ushort)a * (ushort)b);
					state.SP--;
					ip++;
					break;
				case OpCode.MultiplyInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((int)a * (int)b);
					state.SP--;
					ip++;
					break;
				case OpCode.MultiplyUInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = (StackValue)((uint)a * (uint)b);
					state.SP--;
					ip++;
					break;
				case OpCode.DivideInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((short)a / (short)b);
					state.SP--;
					ip++;
					break;
				case OpCode.DivideUInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((ushort)a / (ushort)b);
					state.SP--;
					ip++;
					break;
				case OpCode.DivideInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((int)a / (int)b);
					state.SP--;
					ip++;
					break;
				case OpCode.DivideUInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = (StackValue)((uint)a / (uint)b);
					state.SP--;
					ip++;
					break;
				case OpCode.EqualToInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((short)a == (short)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.EqualToUInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((ushort)a == (ushort)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.EqualToInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((int)a == (int)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.EqualToUInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((uint)a == (uint)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.LessThanInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((short)a < (short)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.LessThanUInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((ushort)a < (ushort)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.LessThanInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((int)a < (int)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.LessThanUInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((uint)a < (uint)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.GreaterThanInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((short)a > (short)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.GreaterThanUInt16:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((ushort)a > (ushort)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.GreaterThanInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((int)a > (int)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.GreaterThanUInt32:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((uint)a > (uint)b) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.NegateInt16:
					a = state.Stack[state.SP - 1];
					state.Stack[state.SP - 1] = -(short)a;
					ip++;
					break;
				case OpCode.NegateUInt16:
					a = state.Stack[state.SP - 1];
					state.Stack[state.SP - 1] = -(ushort)a;
					ip++;
					break;
				case OpCode.NegateInt32:
					a = state.Stack[state.SP - 1];
					state.Stack[state.SP - 1] = -(int)a;
					ip++;
					break;
				case OpCode.NegateUInt32:
					a = state.Stack[state.SP - 1];
					state.Stack[state.SP - 1] = (StackValue)(-(uint)a);
					ip++;
					break;
				case OpCode.LogicalNot:
					a = state.Stack[state.SP - 1];
					state.Stack[state.SP - 1] = (a == 0) ? 1 : 0;
					ip++;
					break;
				case OpCode.LogicalAnd:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((a != 0) && (b != 0)) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				case OpCode.LogicalOr:
					a = state.Stack[state.SP - 2];
					b = state.Stack[state.SP - 1];
					state.Stack[state.SP - 2] = ((a != 0) || (b != 0)) ? 1 : 0;
					state.SP--;
					ip++;
					break;
				default:
					throw new NotImplementedException (i.Op + " has not been implemented yet.");
				}

				state.RemainingTime -= state.CpuSpeed;
			}

			frame.IP = ip;

			if (ip >= Instructions.Count) {
				throw new ExecutionException ("Function '" + Name + "' never returned.");
			}
		}
	}
}

