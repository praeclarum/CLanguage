using CLanguage.Compiler;

namespace CLanguage.Types
{
    public class CVoidType : CType
    {
        public CVoidType()
        {
        }

        public override bool IsVoid
        {
            get
            {
                return true;
            }
        }

        public override int NumValues => 0;

        public override int GetByteSize(EmitContext c)
        {
            c.Report.Error(2070, "'void': illegal sizeof operand");
            return 0;
        }

        public override string ToString()
        {
            return "void";
        }

        public override bool Equals (object? obj)
        {
            return obj is CVoidType;
        }

        public override int GetHashCode ()
        {
            int hash = 17;
            return hash;
        }
    }
}
