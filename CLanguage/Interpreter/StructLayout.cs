using System;
using System.Collections.Generic;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
    /// <summary>
    /// Pre-computed layout of a C struct type, providing named field access
    /// for use in <see cref="InternalFunction"/> callbacks.
    /// <para>
    /// Create once per struct type (e.g. in your MachineInfo constructor),
    /// cache the instance, then use <see cref="Field(string)"/> to get
    /// <see cref="FieldAccessor"/> values that can read/write struct members
    /// at zero additional cost compared to hardcoded offsets.
    /// </para>
    /// </summary>
    public class StructLayout
    {
        /// <summary>
        /// The name of the struct type.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Total number of Value slots this struct occupies on the stack.
        /// </summary>
        public int NumValues { get; }

        readonly Dictionary<string, FieldAccessor> fields;
        readonly Dictionary<string, CType> fieldTypes;

        /// <summary>
        /// Create a StructLayout from a <see cref="CStructType"/>.
        /// Walks the type's members (including base type fields) exactly once
        /// and pre-computes all field offsets.
        /// </summary>
        public StructLayout (CStructType structType)
        {
            if (structType == null)
                throw new ArgumentNullException (nameof (structType));

            Name = structType.Name;
            NumValues = structType.NumValues;
            fields = new Dictionary<string, FieldAccessor> ();
            fieldTypes = new Dictionary<string, CType> ();
            BuildFieldMap (structType);
        }

        /// <summary>
        /// Get a <see cref="FieldAccessor"/> for the named field.
        /// Call once and cache the result for maximum performance.
        /// </summary>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no field with the given name exists in this struct.
        /// </exception>
        public FieldAccessor Field (string name)
        {
            if (fields.TryGetValue (name, out var accessor))
                return accessor;
            throw new KeyNotFoundException ($"Field '{name}' not found in struct '{Name}'");
        }

        /// <summary>
        /// Get a <see cref="StructLayout"/> for a nested struct-typed field.
        /// This creates a new StructLayout each time — cache the result.
        /// </summary>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no field with the given name exists in this struct.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the named field is not a struct type.
        /// </exception>
        public StructLayout FieldLayout (string name)
        {
            if (!fieldTypes.TryGetValue (name, out var memberType))
                throw new KeyNotFoundException ($"Field '{name}' not found in struct '{Name}'");

            if (memberType is CStructType nestedStruct)
                return new StructLayout (nestedStruct);

            throw new InvalidOperationException ($"Field '{name}' in struct '{Name}' is not a struct type (it is {memberType})");
        }

        void BuildFieldMap (CStructType structType)
        {
            if (!structType.IsPolymorphic && structType.BaseType == null) {
                var offset = 0;
                foreach (var m in structType.Members) {
                    if (m is CStructField) {
                        fields[m.Name] = new FieldAccessor (offset, m.MemberType.NumValues);
                        fieldTypes[m.Name] = m.MemberType;
                        offset += m.MemberType.NumValues;
                    }
                }
            }
            else {
                // Polymorphic or has base type: only CStructField members
                // contribute to layout; vptr is at offset 0 if polymorphic.
                var offset = structType.IsPolymorphic ? 1 : 0;

                if (structType.BaseType != null)
                    offset = AddBaseFields (structType.BaseType, offset);

                foreach (var m in structType.Members) {
                    if (m is CStructField) {
                        fields[m.Name] = new FieldAccessor (offset, m.MemberType.NumValues);
                        fieldTypes[m.Name] = m.MemberType;
                        offset += m.MemberType.NumValues;
                    }
                }
            }
        }

        int AddBaseFields (CStructType type, int offset)
        {
            // Recursively add base type fields first
            if (type.BaseType != null)
                offset = AddBaseFields (type.BaseType, offset);

            // Add own fields of this base type
            foreach (var m in type.Members) {
                if (m is CStructField) {
                    fields[m.Name] = new FieldAccessor (offset, m.MemberType.NumValues);
                    fieldTypes[m.Name] = m.MemberType;
                    offset += m.MemberType.NumValues;
                }
            }
            return offset;
        }
    }
}
