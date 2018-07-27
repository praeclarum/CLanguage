using System;
using System.Linq;
using System.Text;
using CLanguage.Interpreter;

namespace CLanguage.Types
{
    public abstract class CType
    {
        public TypeQualifiers TypeQualifiers { get; set; }

        public abstract int GetSize (EmitContext c);

        public static readonly CVoidType Void = new CVoidType ();

        public virtual bool IsIntegral => false;

        public virtual bool IsVoid => false;

        readonly Lazy<CPointerType> pointer;

        public CPointerType Pointer => pointer.Value;

        public CType ()
        {
            pointer = new Lazy<CPointerType> (() => new CPointerType (this));
        }

        public virtual bool CanCastTo (CType otherType)
        {
            return false;
        }
    }
}
