using System;
using CLanguage.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace CLanguage.Types
{
    public class CStructType : CType
    {
        public string Name { get; set; }
        public List<CStructMember> Members { get; set; } = new List<CStructMember> ();

        public CStructType? BaseType { get; set; }
        public List<VTableEntry>? VTable { get; set; }

        public bool HasVTable => VTable != null && VTable.Count > 0;
        public bool IsPolymorphic => HasVTable || (BaseType?.IsPolymorphic ?? false);

        public CStructType (string name)
        {
            Name = name;
        }

        public override string ToString ()
        {
            return string.IsNullOrEmpty(Name) ? "struct" : Name;
        }

        public int GetOwnFieldsNumValues ()
        {
            var s = 0;
            foreach (var m in Members) {
                if (m is CStructField)
                    s += m.MemberType.NumValues;
            }
            return s;
        }

        public int GetOwnFieldsByteSize (EmitContext c)
        {
            var s = 0;
            foreach (var m in Members) {
                if (m is CStructField)
                    s += m.MemberType.GetByteSize (c);
            }
            return s;
        }

        public override int NumValues {
            get {
                if (!IsPolymorphic && BaseType == null) {
                    // Non-polymorphic, no base: original behavior
                    var s = 0;
                    foreach (var m in Members) {
                        s += m.MemberType.NumValues;
                    }
                    return s;
                }
                var total = IsPolymorphic ? 1 : 0; // vptr
                if (BaseType != null)
                    total += BaseType.GetOwnFieldsNumValues ();
                total += GetOwnFieldsNumValues ();
                return total;
            }
        }

        public override int GetByteSize (EmitContext c)
        {
            if (!IsPolymorphic && BaseType == null) {
                // Non-polymorphic, no base: original behavior
                var s = 0;
                foreach (var m in Members) {
                    s += m.MemberType.GetByteSize (c);
                }
                return s;
            }
            var total = IsPolymorphic ? c.MachineInfo.PointerSize : 0; // vptr
            if (BaseType != null)
                total += BaseType.GetOwnFieldsByteSize (c);
            total += GetOwnFieldsByteSize (c);
            return total;
        }

        public int GetFieldValueOffset (CStructMember member, EmitContext c)
        {
            if (!IsPolymorphic && BaseType == null) {
                // Non-polymorphic, no base: original behavior
                var offset = 0;
                foreach (var m in Members) {
                    if (ReferenceEquals (m, member))
                        return offset;
                    offset += m.MemberType.NumValues;
                }
                throw new Exception ($"Member '{member.Name}' not found");
            }
            return GetFieldValueOffsetPolymorphic (member, c);
        }

        int GetFieldValueOffsetPolymorphic (CStructMember member, EmitContext c)
        {
            var offset = IsPolymorphic ? 1 : 0; // skip vptr if present

            // Base class fields first
            if (BaseType != null) {
                var baseOffset = BaseType.FindFieldValueOffset (member, c);
                if (baseOffset >= 0)
                    return offset + baseOffset;
                offset += BaseType.GetOwnFieldsNumValues ();
            }

            // Own fields
            foreach (var m in Members) {
                if (m is CStructField) {
                    if (ReferenceEquals (m, member))
                        return offset;
                    offset += m.MemberType.NumValues;
                }
            }
            throw new Exception ($"Member '{member.Name}' not found");
        }

        int FindFieldValueOffset (CStructMember member, EmitContext c)
        {
            var offset = 0;
            // Search base class fields first
            if (BaseType != null) {
                var baseOffset = BaseType.FindFieldValueOffset (member, c);
                if (baseOffset >= 0)
                    return offset + baseOffset;
                offset += BaseType.GetOwnFieldsNumValues ();
            }
            // Search own fields
            foreach (var m in Members) {
                if (m is CStructField) {
                    if (ReferenceEquals (m, member))
                        return offset;
                    offset += m.MemberType.NumValues;
                }
            }
            return -1; // not found
        }

        /// <summary>
        /// Builds the vtable for this type by inheriting base vtable entries,
        /// replacing overridden slots, and appending new virtual methods.
        /// </summary>
        public void BuildVTable ()
        {
            var entries = new List<VTableEntry> ();

            // Copy base vtable entries
            if (BaseType?.VTable != null) {
                foreach (var baseEntry in BaseType.VTable) {
                    entries.Add (new VTableEntry (
                        baseEntry.SlotIndex,
                        baseEntry.MethodName,
                        baseEntry.Signature,
                        baseEntry.DeclaringType));
                }
            }

            // Process virtual and override methods
            foreach (var m in Members) {
                if (m is CStructMethod method && method.MemberType is CFunctionType sig) {
                    if (method.IsOverride) {
                        // Must match an existing base slot
                        var slot = entries.FindIndex (e => e.MethodName == method.Name && e.Signature.ParameterTypesEqual (sig));
                        if (slot < 0)
                            throw new InvalidOperationException ($"Override method '{method.Name}' does not match any base virtual method in '{Name}'");
                        entries[slot] = new VTableEntry (slot, method.Name, sig, this);
                        method.VTableSlotIndex = slot;
                    }
                    else if (method.IsVirtual) {
                        // Check if it overrides an existing slot (implicit override)
                        var slot = entries.FindIndex (e => e.MethodName == method.Name && e.Signature.ParameterTypesEqual (sig));
                        if (slot >= 0) {
                            entries[slot] = new VTableEntry (slot, method.Name, sig, this);
                            method.VTableSlotIndex = slot;
                        }
                        else {
                            // New virtual method — append a new slot
                            var newSlot = entries.Count;
                            entries.Add (new VTableEntry (newSlot, method.Name, sig, this));
                            method.VTableSlotIndex = newSlot;
                        }
                    }
                }
            }

            VTable = entries.Count > 0 ? entries : null;
        }
    }
}
