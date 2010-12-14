using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public enum Unop
    {
        None,
        Negate,
        BinaryComplement,
        Not,
        PreIncrement,
        PreDecrement,
        PostIncrement,
        PostDecrement
    }

    public class UnaryExpression : Expression
    {
        public Unop Op { get; private set; }
        public Expression Right { get; private set; }

        public UnaryExpression(Unop op, Expression right)
        {
            Op = op;
            Right = right;
        }

        protected override Expression DoResolve(ResolveContext rc)
        {
            var r = Right.Resolve(rc);

            if (r is ConstantExpression)
            {
                throw new NotImplementedException();
            }
            else
            {
                return new UnaryExpression(Op, r);
            }
        }

        public override CType ExpressionType
        {
            get
            {
                return Right.ExpressionType;
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("({0} {1})", Op, Right);
        }
    }
}
