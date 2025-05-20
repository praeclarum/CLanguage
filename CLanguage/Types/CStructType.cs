using System;
using CLanguage.Compiler;
using System.Collections.Generic;
using System.Linq; // Added for List.ToList() and other LINQ operations
using CLanguage.Interpreter; // Added for CompiledFunction

namespace CLanguage.Types
{
    public class CStructType : CType
    {
        public string Name { get; set; }
        public List<CStructMember> Members { get; set; } = new List<CStructMember> ();
        public CStructType BaseClass { get; set; }
        public List<CompiledFunction> VTable { get; private set; }

        public CStructType (string name)
        {
            Name = name;
            // VTable will be initialized later, perhaps by a call to a method like FinalizeLayout()
            // or lazily. For now, let's initialize it after members and base class are set.
            // We'll call FinalizeLayout() explicitly after construction and member population.
            VTable = new List<CompiledFunction>(); // Initialize to empty, will be populated by FinalizeLayout
        }

        // Call this method after Members and BaseClass have been fully set.
        public void FinalizeLayout(EmitContext c)
        {
            // Potentially recalculate byte size and offsets if needed, though current logic seems okay.
            // Then, build the VTable.
            VTable = GetVTable(c);
        }

        public List<CompiledFunction> GetVTable(EmitContext c)
        {
            var vtable = new List<CompiledFunction>();

            // If there's a base class, copy its VTable
            if (BaseClass != null)
            {
                // Ensure base class VTable is also finalized if not already
                // This recursive call might be an issue if not handled carefully (e.g. cyclic dependency)
                // Assuming BaseClass.VTable is already correctly populated or GetVTable handles it.
                // Prefer using the already populated VTable from the base class.
                // FinalizeLayout should be called in order from base to derived.
                if (BaseClass.VTable == null) {
                    // This case should ideally be handled by ensuring FinalizeLayout is called in the correct order.
                    // For safety, we could force finalization, but it's better if the caller ensures this.
                    // For now, let's assume it's populated. If not, it will be an empty list or throw.
                    // Consider throwing an InvalidOperationException if BaseClass.VTable is null and required.
                    // For this iteration, we'll rely on BaseClass.VTable being ready.
                }
                vtable.AddRange(BaseClass.VTable);
            }

            // Iterate through the struct's own members to find methods
            foreach (var member in Members)
            {
                if (member is CStructMethod methodMember && methodMember.MemberType is CFunctionType functionType)
                {
                    // Assumption: Create a CompiledFunction representation for the method.
                    // The actual body (instructions) of this CompiledFunction would be set by the compiler.
                    // For vtable purposes, name and signature are key.
                    // We use 'this.Name' as the NameContext for the function.
                    var currentMethodFunction = new CompiledFunction(methodMember.Name, this.Name, functionType, null);

                    bool replaced = false;
                    for (int i = 0; i < vtable.Count; i++)
                    {
                        // Override based on name and function type (signature)
                        // CFunctionType.Equals compares return type and parameter types.
                        if (vtable[i].Name == currentMethodFunction.Name &&
                            vtable[i].FunctionType.Equals(currentMethodFunction.FunctionType))
                        {
                            vtable[i] = currentMethodFunction; // Replace with the overriding method
                            replaced = true;
                            break;
                        }
                    }

                    if (!replaced)
                    {
                        vtable.Add(currentMethodFunction); // Add as a new virtual method
                    }
                }
            }
            return vtable;
        }

        public override string ToString ()
        {
            return string.IsNullOrEmpty(Name) ? "struct" : Name;
        }

        public override int NumValues {
            get {
                var s = 0;
                foreach (var m in Members) {
                    s += m.MemberType.NumValues;
                }
                return s;
            }
        }

        public override int GetByteSize (EmitContext c)
        {
            var s = 0;
            if (BaseClass != null) {
                s += BaseClass.GetByteSize (c);
            }
            foreach (var m in Members) {
                s += m.MemberType.GetByteSize (c);
            }
            return s;
        }

        public int GetFieldValueOffset (CStructMember member, EmitContext c)
        {
            // Check members of the current struct first
            var localOffset = 0;
            foreach (var m in Members) {
                if (ReferenceEquals (m, member)) {
                    var baseSize = 0;
                    if (BaseClass != null) {
                        baseSize = BaseClass.GetByteSize (c);
                    }
                    return baseSize + localOffset;
                }
                localOffset += m.MemberType.NumValues; // Assuming NumValues is the size for offset calculation
            }

            // If not found in current struct, check the base class
            if (BaseClass != null) {
                // The offset from the base class is already calculated from the start of the memory layout
                // of the base class. No need to add anything further here.
                return BaseClass.GetFieldValueOffset (member, c);
            }
            
            // If not found in current struct and no base class, or not found in the entire hierarchy
            throw new Exception ($"Member '{member.Name}' not found in struct {Name} or its base classes.");
        }
    }
}
