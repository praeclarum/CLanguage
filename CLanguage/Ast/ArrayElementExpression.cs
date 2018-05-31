using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class ArrayElementExpression : Expression
    {
        public Expression Array { get; private set; }
        public Expression ElementIndex { get; private set; }

        public ArrayElementExpression(Expression array, Expression elementIndex)
        {
            Array = array;
            ElementIndex = elementIndex;
        }

		public override CType GetEvaluatedCType (EmitContext ec)
        {
            var a = Array.GetEvaluatedCType (ec) as CArrayType;
            if (a != null)
            {
                return a.ElementType;
            }
            else
            {
                return CType.Void;
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", Array, ElementIndex);
        }
    }
}
