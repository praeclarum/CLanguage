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

        public override int GetSize(CompilerContext c)
        {
            c.Report.Error(2070, "'void': illegal sizeof operand");
            return 0;
        }

        public override string ToString()
        {
            return "void";
        }
    }


}
