namespace CLanguage.Types
{
    /// <summary>
    /// Represents an entry in the compile-time type hierarchy table.
    /// Each polymorphic type gets one entry. This table enables future
    /// <c>dynamic_cast</c> and <c>typeid</c> implementations by encoding
    /// the inheritance tree as a searchable array.
    /// </summary>
    public class TypeHierarchyEntry
    {
        /// <summary>
        /// Unique compile-time identifier for this type.
        /// Matches the <see cref="VTable.TypeId"/> stored at runtime vtable slot 0.
        /// </summary>
        public int TypeId { get; }

        /// <summary>
        /// Type ID of the base class, or -1 if this type has no polymorphic base.
        /// </summary>
        public int BaseTypeId { get; }

        /// <summary>
        /// The name of this type (e.g., "Base", "Derived").
        /// </summary>
        public string TypeName { get; }

        public TypeHierarchyEntry (int typeId, int baseTypeId, string typeName)
        {
            TypeId = typeId;
            BaseTypeId = baseTypeId;
            TypeName = typeName;
        }

        public override string ToString () => $"TypeId={TypeId} Base={BaseTypeId} Name={TypeName}";
    }
}
