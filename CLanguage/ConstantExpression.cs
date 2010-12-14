using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class ConstantExpression : Expression
    {
        public object Value { get; private set; }
        public CType ConstantType { get; private set; }

        public readonly static ConstantExpression Zero = new ConstantExpression(0);

        public ConstantExpression(object val, CType type)
        {
            Value = val;
            ConstantType = type;
        }

        public ConstantExpression(object val)
        {
            Value = val;

            if (Value is string)
            {
                ConstantType = CPointerType.PointerToConstChar;
            }
            else if (Value is byte)
            {
                ConstantType = CBasicType.UnsignedChar;
            }
            else if (Value is char)
            {
                ConstantType = CBasicType.SignedChar;
            }
            else if (Value is ushort)
            {
                ConstantType = CBasicType.UnsignedShortInt;
            }
            else if (Value is short)
            {
                ConstantType = CBasicType.SignedShortInt;
            }
            else if (Value is uint)
            {
                ConstantType = CBasicType.UnsignedInt;
            }
            else if (Value is int)
            {
                ConstantType = CBasicType.SignedInt;
            }
            else if (Value is ulong)
            {
                ConstantType = CBasicType.UnsignedLongLongInt;
            }
            else if (Value is long)
            {
                ConstantType = CBasicType.SignedLongLongInt;
            }
            else if (Value is float)
            {
                ConstantType = CBasicType.Float;
            }
            else if (Value is double)
            {
                ConstantType = CBasicType.Double;
            }
        }

        public override CType ExpressionType
        {
            get { return ConstantType; }
        }

        protected override Expression DoResolve(ResolveContext rc)
        {
            return this;
        }

        protected override void DoEmit(EmitContext ec)
        {
            ec.EmitConstant(Value, ConstantType);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
