using System;
using CLanguage.Interpreter;

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

        public override int GetSize (EmitContext c)
        {
            return Bits / 8;
        }
    }
}
