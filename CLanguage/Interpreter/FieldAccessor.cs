using System;

namespace CLanguage.Interpreter
{
    /// <summary>
    /// Pre-computed accessor for a single field within a C struct.
    /// Created by <see cref="StructLayout.Field(string)"/> and intended
    /// to be cached for the lifetime of the struct type.
    /// At runtime, Get/Set compile down to a single array access with an
    /// integer add — identical cost to hardcoded <c>stack[ptr + N]</c>.
    /// </summary>
    public readonly struct FieldAccessor
    {
        /// <summary>
        /// Pre-computed offset in Value slots from the struct base pointer.
        /// </summary>
        public readonly int Offset;

        /// <summary>
        /// Number of Value slots this field occupies.
        /// 1 for scalar types (int, float, pointer, etc.),
        /// N for struct or array fields.
        /// </summary>
        public readonly int NumValues;

        public FieldAccessor (int offset, int numValues)
        {
            Offset = offset;
            NumValues = numValues;
        }

        /// <summary>
        /// Read this field's value from the stack at the given struct base pointer.
        /// Only valid for scalar fields (NumValues == 1).
        /// </summary>
        public Value Get (Value[] stack, int basePtr) => stack[basePtr + Offset];

        /// <summary>
        /// Write a value to this field on the stack at the given struct base pointer.
        /// Only valid for scalar fields (NumValues == 1).
        /// </summary>
        public void Set (Value[] stack, int basePtr, Value value)
        {
            stack[basePtr + Offset] = value;
        }

        /// <summary>
        /// Returns the absolute stack address of this field.
        /// Useful for struct or array fields where you need to pass
        /// a pointer to the field's data.
        /// </summary>
        public int GetAddress (int basePtr) => basePtr + Offset;
    }
}
