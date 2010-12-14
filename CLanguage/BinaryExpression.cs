using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public enum Binop
    {
        None,
        Equals,
        NotEquals,
        LogicalAnd,
        LogicalOr,
        BinaryAnd,
        BinaryOr,
        BinaryXor,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Add,
        Subtract,
        Multiply,
        Divide,
        Mod,
        ShiftLeft,
        ShiftRight,
    }

    public class BinaryExpression : Expression
    {
        public Expression Left { get; private set; }
        public Binop Op { get; private set; }
        public Expression Right { get; private set; }

        public BinaryExpression(Expression left, Binop op, Expression right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        protected override Expression DoResolve(ResolveContext rc)
        {
            var l = Left.Resolve(rc);
            var r = Right.Resolve(rc);

            if (l is ConstantExpression && r is ConstantExpression)
            {
                throw new NotImplementedException();
            }
            else
            {
                return new BinaryExpression(l, Op, r);
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            Left.Emit(ec);
            Right.Emit(ec);
            ec.EmitBinop(Op);
        }

        public override CType ExpressionType
        {
            get
            {
                if (Op == Binop.Equals || Op == Binop.NotEquals || 
                    Op == Binop.GreaterThan || Op == Binop.GreaterThanOrEqual ||
                    Op == Binop.LessThan || Op == Binop.LessThanOrEqual ||
                    Op == Binop.LogicalAnd || Op == Binop.LogicalOr)
                {
                    return CBasicType.SignedInt;
                }
                else
                {
                    return Left.ExpressionType;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", Left, Op, Right);
        }
    }
}
