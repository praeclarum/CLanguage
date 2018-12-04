using System;
using CLanguage.Compiler;

namespace CLanguage.Types
{
    public class CBoolType : CBasicType
    {
        public override bool IsIntegral => true;

        public CBoolType ()
            : base ("bool", Signedness.Unsigned, "")
        {
        }

        public override int NumValues => 1;

        public override int GetByteSize (EmitContext c) => c.MachineInfo.CharSize;

        public override string ToString () => "bool";
    }
}
