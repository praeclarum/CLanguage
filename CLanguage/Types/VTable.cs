using System.Collections.Generic;

namespace CLanguage.Types
{
    /// <summary>
    /// Represents the virtual method table (vtable) for a polymorphic type.
    /// Each polymorphic <see cref="CStructType"/> has exactly one <see cref="VTable"/>
    /// containing the ordered list of virtual method slots. At runtime, objects of that
    /// type carry a vptr (1 Value slot at offset 0) that points to this table.
    /// 
    /// Runtime vtable layout:
    ///   [0]  = type_id (integer, for RTTI)
    ///   [1]  = first virtual method pointer
    ///   [2]  = second virtual method pointer
    ///   ...
    /// </summary>
    public class VTable
    {
        /// <summary>
        /// Unique type identifier assigned at compile time. Used as the RTTI
        /// foundation for future <c>dynamic_cast</c> and <c>typeid</c> support.
        /// Stored at slot 0 of the runtime vtable array.
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// The ordered list of virtual method slots. Indices in this list correspond
        /// to the <see cref="VTableEntry.SlotIndex"/> values.
        /// At runtime, method pointers start at index 1 (after the type_id slot).
        /// </summary>
        public List<VTableEntry> Entries { get; } = new List<VTableEntry> ();

        /// <summary>
        /// Gets the number of method slots in this vtable.
        /// </summary>
        public int Count => Entries.Count;

        /// <summary>
        /// The total number of runtime slots including the type_id slot.
        /// </summary>
        public int RuntimeSlotCount => 1 + Entries.Count;

        /// <summary>
        /// Gets the vtable entry at the specified slot index.
        /// </summary>
        public VTableEntry this[int index] => Entries[index];
    }
}
