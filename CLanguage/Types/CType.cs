using System.Linq;
using System.Text;

namespace CLanguage.Types
{
    public abstract class CType
    {
        public TypeQualifiers TypeQualifiers { get; set; }

        public abstract int GetSize (EmitContext c);

        public static readonly CVoidType Void = new CVoidType ();

        public virtual bool IsIntegral {
            get {
                return false;
            }
        }

        public CType ()
        {
        }

        public virtual bool IsVoid {
            get {
                return false;
            }
        }
    }
}
