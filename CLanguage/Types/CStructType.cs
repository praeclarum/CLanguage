using System;
using CLanguage.Compiler;
using System.Collections.Generic;

namespace CLanguage.Types
{
    public class CStructType : CType
    {
        public string Name { get; set; }
        public List<CStructMember> Members { get; set; } = new List<CStructMember> ();

        public CStructType (string name)
        {
            Name = name;
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
            foreach (var m in Members) {
                s += m.MemberType.GetByteSize (c);
            }
            return s;
        }

        public int GetFieldValueOffset (CStructMember member, EmitContext c)
        {
            var offset = 0;
            foreach (var m in Members) {
                if (ReferenceEquals (m, member))
                    return offset;
                offset += m.MemberType.NumValues;
            }
            throw new Exception ($"Member '{member.Name}' not found");
        }
    }
}
