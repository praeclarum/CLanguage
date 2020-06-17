using System;
using System.Linq;
using System.Text;
using CLanguage.Interpreter;
using CLanguage.Syntax;
using CLanguage.Compiler;

namespace CLanguage.Types
{
    public abstract class CType
    {
        public TypeQualifiers TypeQualifiers { get; set; }

        public abstract int GetByteSize (EmitContext c);
        public abstract int NumValues { get; }

        public static readonly CVoidType Void = new CVoidType ();

        public virtual bool IsIntegral => false;

        public virtual bool IsVoid => false;

        readonly Lazy<CPointerType> pointer;

        public CPointerType Pointer => pointer.Value;

        public CType ()
        {
            pointer = new Lazy<CPointerType> (CreatePointerType);
        }

        protected virtual CPointerType CreatePointerType ()
        {
            return new CPointerType (this);
        }

        public virtual int ScoreCastTo (CType otherType)
        {
            return Equals (otherType) ? 1000 : 0;
        }

        public virtual object GetClrValue (Value[] values, MachineInfo machineInfo)
        {
            throw new NotSupportedException ($"Cannot get CLR type from {this}");
        }
    }
}
