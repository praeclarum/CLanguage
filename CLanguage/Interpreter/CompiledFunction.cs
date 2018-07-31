using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CLanguage.Syntax;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
    public class CompiledFunction : BaseFunction
    {
        public Block Body { get; private set; }

        public List<CompiledVariable> LocalVariables { get; private set; }
        public List<Instruction> Instructions { get; private set; }

        public CompiledFunction (string name, CFunctionType functionType, Block body = null)
        {
            Name = name;
            FunctionType = functionType;
            Body = body;
            LocalVariables = new List<CompiledVariable> ();
            Instructions = new List<Instruction> ();
        }

        public CompiledFunction (string name, string nameContext, CFunctionType functionType, Block body = null)
            : this (name, functionType, body)
        {
            NameContext = nameContext;
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
            state.ActiveFrame.AllocateLocals (LocalVariables.Count);
        }

        public override void Step (CInterpreter state)
        {
            var frame = state.ActiveFrame;
            var ip = frame.IP;
            var locals = frame.Locals;
            var args = frame.Args;

            var done = false;

            Value a = new Value ();
            Value b = new Value ();

            while (!done && ip < Instructions.Count && state.RemainingTime > 0) {

                var i = Instructions[ip];

                //Debug.WriteLine (i);

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
                    case OpCode.LoadConstant:
                        state.Stack[state.SP] = i.X;
                        state.SP++;
                        ip++;
                        break;
                    case OpCode.LoadGlobal:
                        state.Stack[state.SP] = state.Stack[i.X];
                        state.SP++;
                        ip++;
                        break;
                    case OpCode.LoadPointer: {
                            var p = state.Stack[state.SP - 1];
                            if (!p.IsPointer)
                                throw new InvalidOperationException ($"Cannot dereference {p.Type}");
                            state.Stack[state.SP - 1] = state.Stack[p.PointerValue.Index];
                            ip++;
                        }
                        break;
                    case OpCode.StoreGlobal:
                        state.Stack[i.X] = state.Stack[state.SP - 1];
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.LoadArg:
                        state.Stack[state.SP] = args[i.X];
                        state.SP++;
                        ip++;
                        break;
                    case OpCode.StoreArg:
                        args[i.X] = state.Stack[state.SP - 1];
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.LoadLocal:
                        state.Stack[state.SP] = locals[i.X];
                        state.SP++;
                        ip++;
                        break;
                    case OpCode.StoreLocal:
                        locals[i.X] = state.Stack[state.SP - 1];
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.OffsetPointer:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = a.OffsetPointer (b);
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
                        state.Stack[state.SP - 2] = (Value)((uint)a + (uint)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.AddFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (Value)((float)a + (float)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.AddFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (Value)((double)a + (double)b);
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
                        state.Stack[state.SP - 2] = (Value)((uint)a - (uint)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.SubtractFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (Value)((float)a - (float)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.SubtractFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = (Value)((double)a - (double)b);
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
                        state.Stack[state.SP - 2] = (Value)((uint)a * (uint)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.MultiplyFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((float)a * (float)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.MultiplyFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((double)a * (double)b);
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
                        state.Stack[state.SP - 2] = (Value)((uint)a / (uint)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.DivideFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((float)a / (float)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.DivideFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((double)a / (double)b);
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
                    case OpCode.ModuloFloat32:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((float)a % (float)b);
                        state.SP--;
                        ip++;
                        break;
                    case OpCode.ModuloFloat64:
                        a = state.Stack[state.SP - 2];
                        b = state.Stack[state.SP - 1];
                        state.Stack[state.SP - 2] = ((double)a % (double)b);
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

