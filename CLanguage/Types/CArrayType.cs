using System;
using CLanguage.Ast;

namespace CLanguage.Types
{
    public class CArrayType : CType
    {
        public CType ElementType { get; private set; }
        public Expression LengthExpression { get; set; }

        public CArrayType(CType elementType, Expression lengthExpression)
        {
            ElementType = elementType;
            LengthExpression = lengthExpression;
        }

        public override int GetSize(EmitContext c)
        {
            var innerSize = ElementType.GetSize(c);
            if (LengthExpression == null)
            {
                c.Report.Error(2133, Location, "unknown size");
                return 0;
            }
            var lexp = LengthExpression;
            if (lexp is ConstantExpression)
            {
                var cexp = (ConstantExpression)lexp;

                if (cexp.ConstantType.IsIntegral)
                {
                    var length = Convert.ToInt32(cexp.Value);
                    return length * innerSize;
                }
                else
                {
                    c.Report.Error(2058, LengthExpression.Location, "constant expression is not integral");
                    return 0;
                }
            }
            else
            {
                c.Report.Error(2057, LengthExpression.Location, "expected constant expression");
                return 0;
            }
        }

        public override string ToString()
        {
            return string.Format("(Array {0} {1})", ElementType, LengthExpression);
        }
    }


}
