using System;
using CLanguage.Compiler;
using System.Collections.Generic;

namespace CLanguage.Types
{
    public class CEnumType : CType
    {
        public string Name { get; set; }
        public List<CEnumMember> Members { get; set; } = new List<CEnumMember> ();

        public int NextValue => Members.Count > 0 ? Members[Members.Count - 1].Value + 1 : 0;

        public override int NumValues => 1;

        public override bool IsIntegral => true;

        public override int GetByteSize (EmitContext c)
        {
            return CBasicType.SignedInt.GetByteSize (c);
        }
        public CEnumType (string name)
        {
            Name = name;
        }
    }
}
