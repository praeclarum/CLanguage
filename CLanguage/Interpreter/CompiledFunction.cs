using System;
using System.Collections.Generic;
using System.IO;
using CLanguage.Syntax;
using CLanguage.Types;
using System.Diagnostics;

namespace CLanguage.Interpreter
{
    public class CompiledFunction : BaseFunction
    {
        public Block? Body { get; }

        public List<CompiledVariable> LocalVariables { get; }
        public List<Instruction> Instructions { get; }

        public CompiledFunction (string name, string nameContext, CFunctionType functionType, Block? body)
        {
            Name = name;
            NameContext = nameContext;
            FunctionType = functionType;
            Body = body;
            LocalVariables = new List<CompiledVariable> ();
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

        public override void Init (CInterpreter state)
        {
            var last = LocalVariables.Count == 0 ? null : LocalVariables[LocalVariables.Count - 1];
            if (last != null) {
                state.SP += last.StackOffset + last.VariableType.NumValues;
            }
        }

        public override void Step (CInterpreter state, ExecutionFrame frame)
        {
            var ip = frame.IP;

            var done = false;

            Value a = new Value ();
            Value b = new Value ();

            while (!done && ip < Instructions.Count && state.RemainingTime > 0) {

                var i = Instructions[ip];

                //Debug.WriteLine (new string(' ', 4*state.CallStackDepth) + i + " ;" + state.ActiveFrame?.Function.Name);

                if (state.SP < frame.FP)
                    throw new Exception ($"{(ip - 1 >= 0 ? Instructions[ip - 1] : null)} {this.Name}@{ip - 1} stack underflow");

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
                        if (i.Label != null)
                            ip = i.Label.Index;
                        else
                            throw new InvalidOperationException ($"Jump label not set");
                        break;
                    case OpCode.BranchIfFalse:
                        a = state.Stack[state.SP - 1];
                        state.SP--;
                        if (a.UInt8Value == 0) {
                            if (i.Label != null)
                                ip = i.Label.Index;
                            else
                                throw new InvalidOperationException ($"BranchIfFalse label not set");
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
                    case OpCode.LoadConstant:
                        state.Stack[state.SP] = i.X;
                        state.SP++;
                        ip++;
                        break;
                    case OpCode.LoadFramePointer:
                        state.Stack[state.SP] = frame.FP;
                        state.SP++;
                        ip++;
                        break;
                    case OpCode.LoadPointer:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = state.Stack[a.PointerValue];
                        ip++;
                        break;
                    case OpCode.StorePointer:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[b.PointerValue] = a;
                        state.SP -= 2;
                        ip++;
                        break;
                    case OpCode.OffsetPointer:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2].PointerValue = a.PointerValue + b.Int32Value;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.LoadGlobal:
                        state.Stack[state.SP] = state.Stack[i.X.Int32Value];
                        state.SP++;
                        ip++;
                        break;
                    case OpCode.StoreGlobal:
                        state.Stack[i.X.Int32Value] = state.Stack[state.SP - 1];
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.LoadArg:
                        state.Stack[state.SP] = state.Stack[frame.FP + i.X.Int32Value];
                        state.SP++;
                        ip++;
                        break;
                    case OpCode.StoreArg:
                        state.Stack[frame.FP + i.X.Int32Value] = state.Stack[state.SP - 1];
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.LoadLocal:
                        state.Stack[state.SP] = state.Stack[frame.FP + i.X.Int32Value];
                        state.SP++;
                        ip++;
                        break;
                    case OpCode.StoreLocal:
                        state.Stack[frame.FP + i.X.Int32Value] = state.Stack[state.SP - 1];
                        state.SP--;
                        ip++;
                        break;

                    case OpCode.AddInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (sbyte)((sbyte)a + (sbyte)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.AddUInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (byte)((byte)a + (byte)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.AddInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (short)((short)a + (short)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.AddUInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ushort)((ushort)a + (ushort)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.AddInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (int)a + (int)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.AddUInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (uint)a + (uint)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.AddInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)a + (long)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.AddUInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ulong)a + (ulong)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.AddFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (float)a + (float)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.AddFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (double)a + (double)b;
                        state.SP--;
                        ip++;
                        break;

                    case OpCode.SubtractInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (sbyte)((sbyte)a - (sbyte)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.SubtractUInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (byte)((byte)a - (byte)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.SubtractInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (short)((short)a - (short)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.SubtractUInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ushort)((ushort)a - (ushort)b);
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
                        state.Stack[state.SP - 2] = (uint)((uint)a - (uint)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.SubtractInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)a - (long)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.SubtractUInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ulong)a - (ulong)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.SubtractFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (float)a - (float)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.SubtractFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (double)a - (double)b;
                        state.SP--;
                        ip++;
                        break;

                    case OpCode.MultiplyInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (sbyte)((sbyte)a * (sbyte)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.MultiplyUInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (byte)((byte)a * (byte)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.MultiplyInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (short)((short)a * (short)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.MultiplyUInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ushort)((ushort)a * (ushort)b);
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
                        state.Stack[state.SP - 2] = (uint)((uint)a * (uint)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.MultiplyInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)((long)a * (long)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.MultiplyUInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ulong)((ulong)a * (ulong)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.MultiplyFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (float)a * (float)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.MultiplyFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (double)a * (double)b;
                        state.SP--;
                        ip++;
                        break;

                    case OpCode.DivideInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (sbyte)((sbyte)a / (sbyte)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.DivideUInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (byte)((byte)a / (byte)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.DivideInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (short)((short)a / (short)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.DivideUInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ushort)((ushort)a / (ushort)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.DivideInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (int)a / (int)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.DivideUInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (uint)((uint)a / (uint)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.DivideInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)((long)a / (long)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.DivideUInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ulong)((ulong)a / (ulong)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.DivideFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (float)a / (float)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.DivideFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (double)a / (double)b;
                        state.SP--;
                        ip++;
                        break;

                    case OpCode.ShiftLeftInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (sbyte)((sbyte)a << (sbyte)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftLeftUInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (byte)((byte)a << (byte)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftLeftInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (short)((short)a << (short)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftLeftUInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ushort)((ushort)a << (ushort)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftLeftInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (int)a << (int)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftLeftUInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (uint)((uint)a << (int)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftLeftInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)((long)a << (int)(long)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftLeftUInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ulong)((ulong)a << (int)(ulong)b);
                        state.SP--;
                        ip++;
                        break;
                        // invalid instruction in C
                    case OpCode.ShiftLeftFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((int)(float)a << (int)(float)b);
                        state.SP--;
                        ip++;
                        break;
                    // invalid instruction in C
                    case OpCode.ShiftLeftFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((long)(double)a << (int)(double)b);
                        state.SP--;
                        ip++;
                        break;

                    case OpCode.ShiftRightInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (sbyte)a >> (sbyte)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftRightUInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (byte)a >> (byte)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftRightInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((short)a >> (short)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftRightUInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((ushort)a >> (ushort)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftRightInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((int)a >> (int)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftRightUInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (Value)((uint)a >> (int)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftRightInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)((long)a >> (int)(long)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ShiftRightUInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ulong)((ulong)a >> (int)(ulong)b);
                        state.SP--;
                        ip++;
                        break;
                    // invalid instruction in C
                    case OpCode.ShiftRightFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((int)(float)a >> (int)(float)b);
                        state.SP--;
                        ip++;
                        break;
                    // invalid instruction in C
                    case OpCode.ShiftRightFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((long)(double)a >> (int)(double)b);
                        state.SP--;
                        ip++;
                        break;

                    case OpCode.ModuloInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((short)a % (short)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ModuloUInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((ushort)a % (ushort)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ModuloInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((int)a % (int)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ModuloUInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (Value)((uint)a % (uint)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ModuloInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)((long)a % (long)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ModuloUInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ulong)((ulong)a % (ulong)b);
                        state.SP--;
                        ip++;
                        break;
                    // invalid instruction in C
                    case OpCode.ModuloFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((float)a % (float)b);
                        state.SP--;
                        ip++;
                        break;
                    // invalid instruction in C
                    case OpCode.ModuloFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((double)a % (double)b);
                        state.SP--;
                        ip++;
                        break;

                    case OpCode.BinaryAndInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (sbyte)a & (sbyte)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryAndUInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (byte)a & (byte)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryAndInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (short)a & (short)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryAndUInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ushort)a & (ushort)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryAndInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (int)a & (int)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryAndUInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (uint)a & (uint)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryAndInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)a & (long)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryAndUInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ulong)a & (ulong)b;
                        state.SP--;
                        ip++;
                        break;
                    // invalid instruction in C
                    case OpCode.BinaryAndFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)(float)a & (long)(float)b;
                        state.SP--;
                        ip++;
                        break;
                    // invalid instruction in C
                    case OpCode.BinaryAndFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)(double)a & (long)(double)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryOrInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (sbyte)a | (sbyte)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryOrUInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (byte)a | (byte)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryOrInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (short)a | (short)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryOrUInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ushort)a | (ushort)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryOrInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (int)a | (int)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryOrUInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (uint)a | (uint)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryOrInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)a | (long)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryOrUInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ulong)a | (ulong)b;
                        state.SP--;
                        ip++;
                        break;
                    // invalid instruction in C
                    case OpCode.BinaryOrFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)(float)a | (long)(float)b;
                        state.SP--;
                        ip++;
                        break;
                    // invalid instruction in C
                    case OpCode.BinaryOrFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)(double)a | (long)(double)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryXorInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (sbyte)a ^ (sbyte)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryXorUInt8:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (byte)a ^ (byte)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryXorInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (short)a ^ (short)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryXorUInt16:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ushort)a ^ (ushort)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryXorInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (int)a ^ (int)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryXorUInt32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (uint)a ^ (uint)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryXorInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)a ^ (long)b;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.BinaryXorUInt64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (ulong)a ^ (ulong)b;
                        state.SP--;
                        ip++;
                        break;
                    // invalid instruction in C
                    case OpCode.BinaryXorFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)(float)a ^ (long)(float)b;
                        state.SP--;
                        ip++;
                        break;
                    // invalid instruction in C
                    case OpCode.BinaryXorFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (long)(double)a ^ (long)(double)b;
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
                    case OpCode.EqualToFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((float)a == (float)b) ? 1 : 0;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.EqualToFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((double)a == (double)b) ? 1 : 0;
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
                    case OpCode.LessThanFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((float)a < (float)b) ? 1 : 0;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.LessThanFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((double)a < (double)b) ? 1 : 0;
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
                    case OpCode.GreaterThanFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((float)a > (float)b) ? 1 : 0;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.GreaterThanFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((double)a > (double)b) ? 1 : 0;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.NotInt8:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (sbyte)a == 0 ? 1 : 0;
                        ip++;
                        break;
                    case OpCode.NotUInt8:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (byte)a == 0 ? 1 : 0;
                        ip++;
                        break;
                    case OpCode.NotInt16:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (short)a == 0 ? 1 : 0;
                        ip++;
                        break;
                    case OpCode.NotUInt16:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (ushort)a == 0 ? 1 : 0;
                        ip++;
                        break;
                    case OpCode.NotInt32:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (int)a == 0 ? 1 : 0;
                        ip++;
                        break;
                    case OpCode.NotUInt32:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (uint)a == 0 ? 1 : 0;
                        ip++;
                        break;
                    case OpCode.NotInt64:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (long)a == 0 ? 1 : 0;
                        ip++;
                        break;
                    case OpCode.NotUInt64:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (ulong)a == 0 ? 1 : 0;
                        ip++;
                        break;
                    case OpCode.NotFloat32:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (float)a == 0.0f ? 1 : 0;
                        ip++;
                        break;
                    case OpCode.NotFloat64:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (double)a == 0.0 ? 1 : 0;
                        ip++;
                        break;
                    case OpCode.BinaryNotInt8:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = ~(sbyte)a;
                        ip++;
                        break;
                    case OpCode.BinaryNotUInt8:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = ~(byte)a;
                        ip++;
                        break;
                    case OpCode.BinaryNotInt16:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = ~(short)a;
                        ip++;
                        break;
                    case OpCode.BinaryNotUInt16:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = ~(ushort)a;
                        ip++;
                        break;
                    case OpCode.BinaryNotInt32:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = ~(int)a;
                        ip++;
                        break;
                    case OpCode.BinaryNotUInt32:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = ~(uint)a;
                        ip++;
                        break;
                    case OpCode.BinaryNotInt64:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = ~(long)a;
                        ip++;
                        break;
                    case OpCode.BinaryNotUInt64:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = ~(ulong)a;
                        ip++;
                        break;
                    case OpCode.BinaryNotFloat32:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = ~(int)(float)a;
                        ip++;
                        break;
                    case OpCode.BinaryNotFloat64:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = ~(int)(double)a;
                        ip++;
                        break;
                    case OpCode.NegateInt8:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = -(sbyte)a;
                        ip++;
                        break;
                    case OpCode.NegateUInt8:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = -(byte)a;
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
                        state.Stack[state.SP - 1] = (Value)(-(uint)a);
                        ip++;
                        break;
                    case OpCode.NegateInt64:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = -(long)a;
                        ip++;
                        break;
                    case OpCode.NegateUInt64:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (Value)((ulong)-(long)a);
                        ip++;
                        break;
                    case OpCode.NegateFloat32:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (Value)(-(float)a);
                        ip++;
                        break;
                    case OpCode.NegateFloat64:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = (Value)(-(double)a);
                        ip++;
                        break;
                    case OpCode.LogicalAnd:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((a.Int32Value != 0) && (b.Int32Value != 0)) ? 1 : 0;
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.LogicalOr:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((a.Int32Value != 0) || (b.Int32Value != 0)) ? 1 : 0;
                        state.SP--;
                        ip++;
                        break;
                    default:
                        a = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 1] = Convert (a, i.Op);
                        ip++;
                        break;
                }

                state.RemainingTime -= state.CpuSpeed;
            }

            frame.IP = ip;

            if (ip >= Instructions.Count) {
                throw new ExecutionException ("Function '" + Name + "' never returned.");
            }
        }

        Value Convert (Value x, OpCode op)
        {
            switch (op) {
                case OpCode.ConvertInt8Int8: return (sbyte)(sbyte)x;
                case OpCode.ConvertInt8UInt8: return (byte)(sbyte)x;
                case OpCode.ConvertInt8Int16: return (short)(sbyte)x;
                case OpCode.ConvertInt8UInt16: return (ushort)(sbyte)x;
                case OpCode.ConvertInt8Int32: return (int)(sbyte)x;
                case OpCode.ConvertInt8UInt32: return (uint)(sbyte)x;
                case OpCode.ConvertInt8Int64: return (long)(sbyte)x;
                case OpCode.ConvertInt8UInt64: return (ulong)(sbyte)x;
                case OpCode.ConvertInt8Float32: return (float)(sbyte)x;
                case OpCode.ConvertInt8Float64: return (double)(sbyte)x;

                case OpCode.ConvertUInt8Int8: return (sbyte)(byte)x;
                case OpCode.ConvertUInt8UInt8: return (byte)(byte)x;
                case OpCode.ConvertUInt8Int16: return (short)(byte)x;
                case OpCode.ConvertUInt8UInt16: return (ushort)(byte)x;
                case OpCode.ConvertUInt8Int32: return (int)(byte)x;
                case OpCode.ConvertUInt8UInt32: return (uint)(byte)x;
                case OpCode.ConvertUInt8Int64: return (long)(byte)x;
                case OpCode.ConvertUInt8UInt64: return (ulong)(byte)x;
                case OpCode.ConvertUInt8Float32: return (float)(byte)x;
                case OpCode.ConvertUInt8Float64: return (double)(byte)x;

                case OpCode.ConvertInt16Int8: return (sbyte)(short)x;
                case OpCode.ConvertInt16UInt8: return (byte)(short)x;
                case OpCode.ConvertInt16Int16: return (short)(short)x;
                case OpCode.ConvertInt16UInt16: return (ushort)(short)x;
                case OpCode.ConvertInt16Int32: return (int)(short)x;
                case OpCode.ConvertInt16UInt32: return (uint)(short)x;
                case OpCode.ConvertInt16Int64: return (long)(short)x;
                case OpCode.ConvertInt16UInt64: return (ulong)(short)x;
                case OpCode.ConvertInt16Float32: return (float)(short)x;
                case OpCode.ConvertInt16Float64: return (double)(short)x;

                case OpCode.ConvertUInt16Int8: return (sbyte)(ushort)x;
                case OpCode.ConvertUInt16UInt8: return (byte)(ushort)x;
                case OpCode.ConvertUInt16Int16: return (short)(ushort)x;
                case OpCode.ConvertUInt16UInt16: return (ushort)(ushort)x;
                case OpCode.ConvertUInt16Int32: return (int)(ushort)x;
                case OpCode.ConvertUInt16UInt32: return (uint)(ushort)x;
                case OpCode.ConvertUInt16Int64: return (long)(ushort)x;
                case OpCode.ConvertUInt16UInt64: return (ulong)(ushort)x;
                case OpCode.ConvertUInt16Float32: return (float)(ushort)x;
                case OpCode.ConvertUInt16Float64: return (double)(ushort)x;

                case OpCode.ConvertInt32Int8: return (sbyte)(int)x;
                case OpCode.ConvertInt32UInt8: return (byte)(int)x;
                case OpCode.ConvertInt32Int16: return (short)(int)x;
                case OpCode.ConvertInt32UInt16: return (ushort)(int)x;
                case OpCode.ConvertInt32Int32: return (int)(int)x;
                case OpCode.ConvertInt32UInt32: return (uint)(int)x;
                case OpCode.ConvertInt32Int64: return (long)(int)x;
                case OpCode.ConvertInt32UInt64: return (ulong)(int)x;
                case OpCode.ConvertInt32Float32: return (float)(int)x;
                case OpCode.ConvertInt32Float64: return (double)(int)x;

                case OpCode.ConvertUInt32Int8: return (sbyte)(uint)x;
                case OpCode.ConvertUInt32UInt8: return (byte)(uint)x;
                case OpCode.ConvertUInt32Int16: return (short)(uint)x;
                case OpCode.ConvertUInt32UInt16: return (ushort)(uint)x;
                case OpCode.ConvertUInt32Int32: return (int)(uint)x;
                case OpCode.ConvertUInt32UInt32: return (uint)(uint)x;
                case OpCode.ConvertUInt32Int64: return (long)(uint)x;
                case OpCode.ConvertUInt32UInt64: return (ulong)(uint)x;
                case OpCode.ConvertUInt32Float32: return (float)(uint)x;
                case OpCode.ConvertUInt32Float64: return (double)(uint)x;

                case OpCode.ConvertInt64Int8: return (sbyte)(long)x;
                case OpCode.ConvertInt64UInt8: return (byte)(long)x;
                case OpCode.ConvertInt64Int16: return (short)(long)x;
                case OpCode.ConvertInt64UInt16: return (ushort)(long)x;
                case OpCode.ConvertInt64Int32: return (int)(long)x;
                case OpCode.ConvertInt64UInt32: return (uint)(long)x;
                case OpCode.ConvertInt64Int64: return (long)(long)x;
                case OpCode.ConvertInt64UInt64: return (ulong)(long)x;
                case OpCode.ConvertInt64Float32: return (float)(long)x;
                case OpCode.ConvertInt64Float64: return (double)(long)x;

                case OpCode.ConvertUInt64Int8: return (sbyte)(ulong)x;
                case OpCode.ConvertUInt64UInt8: return (byte)(ulong)x;
                case OpCode.ConvertUInt64Int16: return (short)(ulong)x;
                case OpCode.ConvertUInt64UInt16: return (ushort)(ulong)x;
                case OpCode.ConvertUInt64Int32: return (int)(ulong)x;
                case OpCode.ConvertUInt64UInt32: return (uint)(ulong)x;
                case OpCode.ConvertUInt64Int64: return (long)(ulong)x;
                case OpCode.ConvertUInt64UInt64: return (ulong)(ulong)x;
                case OpCode.ConvertUInt64Float32: return (float)(ulong)x;
                case OpCode.ConvertUInt64Float64: return (double)(ulong)x;

                case OpCode.ConvertFloat32Int8: return (sbyte)(float)x;
                case OpCode.ConvertFloat32UInt8: return (byte)(float)x;
                case OpCode.ConvertFloat32Int16: return (short)(float)x;
                case OpCode.ConvertFloat32UInt16: return (ushort)(float)x;
                case OpCode.ConvertFloat32Int32: return (int)(float)x;
                case OpCode.ConvertFloat32UInt32: return (uint)(float)x;
                case OpCode.ConvertFloat32Int64: return (long)(float)x;
                case OpCode.ConvertFloat32UInt64: return (ulong)(float)x;
                case OpCode.ConvertFloat32Float32: return (float)(float)x;
                case OpCode.ConvertFloat32Float64: return (double)(float)x;

                case OpCode.ConvertFloat64Int8: return (sbyte)(double)x;
                case OpCode.ConvertFloat64UInt8: return (byte)(double)x;
                case OpCode.ConvertFloat64Int16: return (short)(double)x;
                case OpCode.ConvertFloat64UInt16: return (ushort)(double)x;
                case OpCode.ConvertFloat64Int32: return (int)(double)x;
                case OpCode.ConvertFloat64UInt32: return (uint)(double)x;
                case OpCode.ConvertFloat64Int64: return (long)(double)x;
                case OpCode.ConvertFloat64UInt64: return (ulong)(double)x;
                case OpCode.ConvertFloat64Float32: return (float)(double)x;
                case OpCode.ConvertFloat64Float64: return (double)(double)x;

                default:
                    throw new NotSupportedException ($"Op code '{op}' is not supported");
            }
        }
    }
}

