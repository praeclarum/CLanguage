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

        public int GetFieldOffset (CStructMember member, EmitContext c)
        {
            var offset = 0;
            foreach (var m in Members) {
                if (ReferenceEquals (m, member))
                    return offset;
                offset += m.MemberType.GetByteSize (c);
            }
            return offset;
        }
    }
}
