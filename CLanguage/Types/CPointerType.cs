using CLanguage.Interpreter;

namespace CLanguage.Types
{
    public class CPointerType : CType
    {
        public CType InnerType { get; private set; }

        public CPointerType(CType innerType)
        {
            InnerType = innerType;
        }

        public static readonly CPointerType PointerToConstChar = new CPointerType(CBasicType.ConstChar);

        public override int GetSize(EmitContext c)
        {
            return c.MachineInfo.PointerSize;
        }

        public override string ToString()
        {
            return "(Pointer " + InnerType + ")";
        }
    }


}
