using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Types;
using CLanguage.Interpreter;

namespace CLanguage.Syntax
{
    public class ConstantExpression : Expression
    {
        public object Value { get; private set; }
		public Value EmitValue { get; private set; }
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
			EmitValue = 0;

            if (Value is string)
            {
                ConstantType = CPointerType.PointerToConstChar;
            }
            else if (Value is bool) {
                ConstantType = CBasicType.Bool;
                EmitValue = (byte)((bool)Value ? 1 : 0);
            }
            else if (Value is byte)
            {
                ConstantType = CBasicType.UnsignedChar;
				EmitValue = (byte)Value;
            }
            else if (Value is char)
            {
                ConstantType = CBasicType.SignedChar;
				EmitValue = (char)Value;
            }
            else if (Value is ushort)
            {
                ConstantType = CBasicType.UnsignedShortInt;
				EmitValue = (ushort)Value;
			}
            else if (Value is short)
            {
                ConstantType = CBasicType.SignedShortInt;
				EmitValue = (short)Value;
			}
            else if (Value is uint)
            {
                ConstantType = CBasicType.UnsignedInt;
				EmitValue = (int)(uint)Value;
			}
            else if (Value is int)
            {
                ConstantType = CBasicType.SignedInt;
				EmitValue = (int)Value;
			}
            else if (Value is ulong)
            {
                ConstantType = CBasicType.UnsignedLongLongInt;
				EmitValue = (Value)(ulong)Value;
			}
            else if (Value is long)
            {
                ConstantType = CBasicType.SignedLongLongInt;
				EmitValue = (Value)(long)Value;
			}
            else if (Value is float)
            {
                ConstantType = CBasicType.Float;
				EmitValue = (Value)(float)Value;
			}
            else if (Value is double)
            {
                ConstantType = CBasicType.Double;
				EmitValue = (Value)(double)Value;
			}
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return ConstantType;
        }

        protected override void DoEmit (EmitContext ec)
        {
            if (ConstantType is CIntType intType) {
                var size = intType.GetByteSize (ec);
                if (intType.Signedness == Signedness.Signed) {
                    switch (size) {
                        case 1:
                            ec.Emit (OpCode.LoadConstant, (Value)Convert.ToSByte (Value));
                            break;
                        case 2:
                            ec.Emit (OpCode.LoadConstant, (Value)Convert.ToInt16 (Value));
                            break;
                        case 4:
                            ec.Emit (OpCode.LoadConstant, (Value)Convert.ToInt32 (Value));
                            break;
                        case 8:
                            ec.Emit (OpCode.LoadConstant, (Value)Convert.ToInt64 (Value));
                            break;
                        default:
                            throw new NotSupportedException ("Signed integral constants with type '" + ConstantType + "'");
                    }
                }
                else {
                    switch (size) {
                        case 1:
                            ec.Emit (OpCode.LoadConstant, (Value)Convert.ToByte (Value));
                            break;
                        case 2:
                            ec.Emit (OpCode.LoadConstant, (Value)Convert.ToUInt16 (Value));
                            break;
                        case 4:
                            ec.Emit (OpCode.LoadConstant, (Value)Convert.ToUInt32 (Value));
                            break;
                        case 8:
                            ec.Emit (OpCode.LoadConstant, (Value)Convert.ToUInt64 (Value));
                            break;
                        default:
                            throw new NotSupportedException ("Unsigned integral constants with type '" + ConstantType + "'");
                    }
                }
            }
            else if (ConstantType is CBoolType boolType) {
                ec.Emit (OpCode.LoadConstant, EmitValue);
            }
            else if (ConstantType is CFloatType floatType) {
                ec.Emit (OpCode.LoadConstant, EmitValue);
            }
            else if (Value is string vs) {
                ec.Emit (OpCode.LoadConstant, ec.GetConstantMemory (vs));
            }
            else {
				throw new NotSupportedException ("Non-basic constants with type '" + ConstantType + "'");
			}
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
