using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class AssignExpression : Expression
    {
        public Expression Left { get; private set; }
        public Binop Op { get; private set; }
        public Expression Right { get; private set; }

        public AssignExpression(Expression left, Expression right)
        {
            Left = left;
            Op = Binop.None;
            Right = right;
        }

        public AssignExpression(Expression left, Binop op, Expression right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        public override CType ExpressionType
        {
            get { return Left.ExpressionType; }
        }

        protected override Expression DoResolve(ResolveContext rc)
        {
            return new AssignExpression(Left.Resolve(rc), Right.Resolve(rc));
        }

        protected override void DoEmit(EmitContext ec)
        {
            if (Op == Binop.None)
            {
                Right.Emit(ec);
            }
            else
            {
                Left.Emit(ec);
                Right.Emit(ec);
                ec.EmitBinop(Op);
            }
            ec.EmitAssign(Left);
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Left, Right);
        }
    }
}
