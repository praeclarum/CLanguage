using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class EmitContext : ResolveContext
    {
        int _labelId = 1;

        public class Label
        {
            public int Id { get; private set; }
            public Label(int id)
            {
                Id = id;
            }
            public override string ToString()
            {
                return Id.ToString();
            }
        }

        public EmitContext(CompilerContext c, MachineInfo m) 
            : base(c, m)
        {
        }

        public virtual void DeclareVariable(VariableDeclaration v) { }
        public virtual void DeclareFunction(FunctionDeclaration f) { }

        public virtual void BeginBlock(Block b) { }
        public virtual void EndBlock() { }

        public virtual void BeginCatchBlock(CType exceptionType) { }
        public virtual void BeginExceptFilterBlock() { }
        public virtual Label BeginExceptionBlock() { var l = DefineLabel(); EmitLabel(l); return l; }
        public virtual void BeginFaultBlock() { }
        public virtual void BeginFinallyBlock() { }
        public virtual void EndExceptionBlock() { }
        public virtual void ThrowException(Type excType) { }

        public virtual Label DefineLabel() { return new Label(_labelId++); }
        public virtual void EmitLabel(Label l)
        {
        }
        public virtual void EmitJump(Label l)
        {
        }
        public virtual void EmitBranchIfTrue(Label l)
        {
        }
        public virtual void EmitBranchIfFalse(Label l)
        {
        }

        public virtual void EmitBinop(Binop op)
        {
        }

        public virtual void EmitUnop(Unop op)
        {
        }
        
        public virtual void EmitCall(CFunctionType type, int argsCount)
        {
        }

        public virtual void EmitConstant(object value, CType type)
        {
        }

        public virtual void EmitAssign(Expression left)
        {
        }

        public virtual void EmitLoadLocal(int index)
        {
        }

        public virtual void EmitPop()
        {
        }

        /*public virtual void Emit(OpCode opcode) { }
        public virtual void Emit(OpCode opcode, sbyte arg) { }
        public virtual void Emit(OpCode opcode, byte arg) { }
        public virtual void Emit(OpCode opcode, double arg) { }
        public virtual void Emit(OpCode opcode, float arg) { }
        public virtual void Emit(OpCode opcode, int arg) { }
        public virtual void Emit(OpCode opcode, Label label) { }
        public virtual void Emit(OpCode opcode, Label[] labels) { }
        public virtual void Emit(OpCode opcode, long arg) { }
        public virtual void Emit(OpCode opcode, short arg) { }
        public virtual void Emit(OpCode opcode, string str) { }*/
    }
}
