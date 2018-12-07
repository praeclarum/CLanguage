using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Types;
using CLanguage.Interpreter;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class ConstantExpression : Expression
    {
        public object Value { get; private set; }
        public CType ConstantType { get; private set; }

        public readonly static ConstantExpression Zero = new ConstantExpression (0);
        public readonly static ConstantExpression One = new ConstantExpression (1);
        public readonly static ConstantExpression NegativeOne = new ConstantExpression (-1);
        public readonly static ConstantExpression True = new ConstantExpression (true);
        public readonly static ConstantExpression False = new ConstantExpression (false);

        public ConstantExpression(object val, CType type)
			: this (val)
        {
            ConstantType = type;
        }

        public ConstantExpression(object val)
        {
            Value = val;

            if (Value is string)
            {
                ConstantType = CPointerType.PointerToConstChar;
            }
            else if (Value is bool) {
                ConstantType = CBasicType.Bool;
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
                ConstantType = CBasicType.UnsignedLongInt;
			}
            else if (Value is long)
            {
                ConstantType = CBasicType.SignedLongInt;
			}
            else if (Value is float)
            {
                ConstantType = CBasicType.Float;
			}
            else if (Value is double)
            {
                ConstantType = CBasicType.Double;
			}
            else {
                ConstantType = CBasicType.SignedInt;
            }
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return ConstantType;
        }

        protected override void DoEmit (EmitContext ec)
        {
            var cval = EvalConstant (ec);
            ec.Emit (OpCode.LoadConstant, cval);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override Value EvalConstant (EmitContext ec)
        {
            if (ConstantType is CIntType intType) {
                var size = intType.GetByteSize (ec);
                if (intType.Signedness == Signedness.Signed) {
                    unchecked {
                        switch (size) {
                            case 1:
                                return (sbyte)Convert.ToInt64 (Value);
                            case 2:
                                return (short)Convert.ToInt64 (Value);
                            case 4:
                                return (int)Convert.ToInt64 (Value);
                            case 8:
                                return Convert.ToInt64 (Value);
                            default:
                                throw new NotSupportedException ("Signed integral constants with type '" + ConstantType + "'");
                        }
                    }
                }
                else {
                    unchecked {
                        switch (size) {
                            case 1:
                                return (byte)Convert.ToInt64 (Value);
                            case 2:
                                return (ushort)Convert.ToInt64 (Value);
                            case 4:
                                return (uint)Convert.ToInt64 (Value);
                            case 8:
                                return Convert.ToUInt64 (Value);
                            default:
                                throw new NotSupportedException ("Unsigned integral constants with type '" + ConstantType + "'");
                        }
                    }
                }
            }
            else if (ConstantType is CBoolType boolType) {
                return (byte)((bool)Value ? 1 : 0);
            }
            else if (ConstantType is CFloatType floatType) {
                return floatType.Bits == 64 ? (Value)Convert.ToDouble (Value) : (Value)Convert.ToSingle (Value);
            }
            else if (Value is string vs) {
                return ec.GetConstantMemory (vs);
            }
            else {
                throw new NotSupportedException ("Non-basic constants with type '" + ConstantType + "'");
            }
        }
    }
}
