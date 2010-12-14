using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; set; }

        public ExpressionStatement(Expression expr)
        {
            Expression = expr;
        }

        protected override void DoEmit(EmitContext ec)
        {
            if (Expression != null)
            {
                Expression.Emit(ec);
                if (!(Expression is AssignExpression))
                {
                    ec.EmitPop();
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0};", Expression);
        }
    }
}
