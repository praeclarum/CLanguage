namespace CLanguage.Types
{
    /// <summary>
    /// Represents a single slot in a virtual method table. Each entry maps
    /// a slot index to the method that currently fills that slot for a given type.
    /// </summary>
    public class VTableEntry
    {
        /// <summary>
        /// Zero-based position of this method in the vtable array.
        /// </summary>
        public int SlotIndex { get; set; }

        /// <summary>
        /// Name of the virtual method. Used for override matching and debugging.
        /// </summary>
        public string MethodName { get; set; } = "";

        /// <summary>
        /// The function signature expected at this vtable slot.
        /// </summary>
        public CFunctionType Signature { get; set; }

        /// <summary>
        /// The type that originally introduced this virtual method slot.
        /// When a derived class overrides a method, <see cref="DeclaringType"/>
        /// is updated to the overriding type, while <see cref="SlotIndex"/>
        /// remains the same as in the base class vtable.
        /// </summary>
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
