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

        public int GetByteSize (MachineInfo c)
        {
            if (Name == "char") {
                return c.CharSize;
            }
            else if (Name == "int") {
                if (Size == "short") {
                    return c.ShortIntSize;
                }
                else if (Size == "long") {
                    return c.LongIntSize;
                }
                else if (Size == "long long") {
                    return c.LongLongIntSize;
                }
                else {
                    return c.IntSize;
                }
            }
            else {
                throw new NotSupportedException (this.ToString ());
            }
        }

        public override int GetByteSize (EmitContext c)
        {
            return GetByteSize (c.MachineInfo);
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

        public override object GetClrValue (Value[] values, MachineInfo machineInfo)
        {
            var byteSize = GetByteSize (machineInfo);
            if (this.Signedness == Signedness.Signed) {
                switch (byteSize) {
                    case 1:
                        return values[0].Int8Value;
                    case 2:
                        return values[0].Int16Value;
                    case 4:
                        return values[0].Int32Value;
                    default:
                        return values[0].Int64Value;
                }
            }
            else {
                switch (byteSize) {
                    case 1:
                        return values[0].UInt8Value;
                    case 2:
                        return values[0].UInt16Value;
                    case 4:
                        return values[0].UInt32Value;
                    default:
                        return values[0].UInt64Value;
                }
            }
        }
    }
}
