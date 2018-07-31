using System;
using CLanguage.Interpreter;
using System.Collections.Generic;

namespace CLanguage.Types
{
    public class CStructType : CType
    {
        public string Name { get; set; }
        public List<CStructMember> Members { get; set; } = new List<CStructMember> ();

        public CStructType ()
        {
        }

        public override int GetByteSize (EmitContext c)
        {
            var s = 0;
            foreach (var m in Members) {
                s += m.MemberType.GetByteSize (c);
            }
            return s;
        }
    }
}
