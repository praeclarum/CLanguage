using System;
using CLanguage.Interpreter;

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
    }
}
