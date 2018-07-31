using System;
using CLanguage.Interpreter;

namespace CLanguage.Types
{
    public class CBoolType : CBasicType
    {
        public override bool IsIntegral => true;

        public CBoolType ()
            : base ("bool", Signedness.Unsigned, "")
        {
        }

        public override int GetByteSize (EmitContext c)
        {
            return c.MachineInfo.CharSize;
        }
    }
}
