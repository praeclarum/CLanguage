using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Types;

namespace CLanguage.Syntax
{
    public class MemberFromReferenceExpression : Expression
    {
        public Expression Left { get; private set; }
        public string MemberName { get; private set; }

        public MemberFromReferenceExpression(Expression left, string memberName)
        {
            Left = left;
            MemberName = memberName;
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
            var targetType = Left.GetEvaluatedCType (ec);

            throw new NotImplementedException("Member access on " + targetType.GetType ());
        }

        protected override void DoEmit(EmitContext ec)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Left, MemberName);
        }
    }
}
