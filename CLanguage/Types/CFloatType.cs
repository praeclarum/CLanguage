using System;
using CLanguage.Compiler;

namespace CLanguage.Types
{
    public class CFloatType : CBasicType
    {
        public int Bits { get; }

        public CFloatType (string name, int bits)
            : base (name, Signedness.Signed, "")
        {
            Bits = bits;
        }

        public override int NumValues => 1;

        public override int GetByteSize (EmitContext c)
        {
            return Bits / 8;
        }

        public override int ScoreCastTo (CType otherType)
        {
            if (Equals (otherType)) return 1000;
            if (otherType is CFloatType ft) {
                return 900;
            }
            else {
                return 0;
            }
        }

    }
}
