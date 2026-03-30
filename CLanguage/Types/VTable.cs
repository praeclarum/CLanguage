using System.Collections.Generic;

namespace CLanguage.Types
{
    /// <summary>
    /// Represents the virtual method table (vtable) for a polymorphic type.
    /// Each polymorphic <see cref="CStructType"/> has exactly one <see cref="VTable"/>
    /// containing the ordered list of virtual method slots. At runtime, objects of that
    /// type carry a vptr (1 Value slot at offset 0) that points to this table.
    /// This type is designed to be extended with RTTI and other metadata in the future.
    /// </summary>
    public class VTable
    {
        /// <summary>
        /// The ordered list of virtual method slots. Indices in this list correspond
        /// to the <see cref="VTableEntry.SlotIndex"/> values.
        /// </summary>
        public List<VTableEntry> Entries { get; } = new List<VTableEntry> ();

        /// <summary>
        /// Gets the number of slots in this vtable.
        /// </summary>
        public int Count => Entries.Count;

        /// <summary>
        /// Gets the vtable entry at the specified slot index.
        /// </summary>
        public VTableEntry this[int index] => Entries[index];
    }
}
