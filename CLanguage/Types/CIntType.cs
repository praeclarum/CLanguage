using System;
using CLanguage.Compiler;

namespace CLanguage.Types
{
    public class CIntType : CBasicType
    {
        public override bool IsIntegral => true;

        public CIntType (string name, Signedness signedness, string size)
            : base (name, signedness, size)
        {
        }

        public override int NumValues => 1;

        public override int GetByteSize (EmitContext c)
        {
            if (Name == "char") {
                return c.MachineInfo.CharSize;
            }
            else if (Name == "int") {
                if (Size == "short") {
                    return c.MachineInfo.ShortIntSize;
                }
                else if (Size == "long") {
                    return c.MachineInfo.LongIntSize;
                }
                else if (Size == "long long") {
                    return c.MachineInfo.LongLongIntSize;
                }
                else {
                    return c.MachineInfo.IntSize;
                }
            }
            else {
                throw new NotSupportedException (this.ToString ());
            }
        }

        public override int ScoreCastTo (CType otherType)
        {
            if (Equals (otherType)) return 1000;
            if (otherType is CIntType it) {
                if (Size == it.Size) return 900;
                return 800;
            }
            else if (otherType is CFloatType ft) {
                if (ft.Bits == 64) return 400;
                return 300;
            }
            else if (otherType is CBoolType bt) {
                return 200;
            }
            else {
                return 0;
            }
        }
    }
}
