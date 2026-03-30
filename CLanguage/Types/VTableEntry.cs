namespace CLanguage.Types
{
    public class VTableEntry
    {
        public int SlotIndex { get; set; }
        public string MethodName { get; set; } = "";
        public CFunctionType Signature { get; set; }
        public CStructType DeclaringType { get; set; }

        public VTableEntry (int slotIndex, string methodName, CFunctionType signature, CStructType declaringType)
        {
            SlotIndex = slotIndex;
            MethodName = methodName;
            Signature = signature;
            DeclaringType = declaringType;
        }

        public override string ToString () => $"vtable[{SlotIndex}] {MethodName} (from {DeclaringType})";
    }
}
