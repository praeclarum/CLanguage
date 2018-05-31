using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ValueType = System.Int32;

namespace CLanguage
{
    public class ConstantExpression : Expression
    {
        public object Value { get; private set; }
		public ValueType EmitValue { get; private set; }
        public CType ConstantType { get; private set; }

        public readonly static ConstantExpression Zero = new ConstantExpression(0);

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
				EmitValue = (ValueType)(ulong)Value;
			}
            else if (Value is long)
            {
                ConstantType = CBasicType.SignedLongLongInt;
				EmitValue = (ValueType)(long)Value;
			}
            else if (Value is float)
            {
                ConstantType = CBasicType.Float;
				EmitValue = (ValueType)(float)Value;
			}
            else if (Value is double)
            {
                ConstantType = CBasicType.Double;
				EmitValue = (ValueType)(double)Value;
			}
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return ConstantType;
        }

        protected override void DoEmit (EmitContext ec)
        {
			var basicType = ConstantType as CBasicType;

			if (basicType != null) {

				if (basicType.IsIntegral) {
					var size = basicType.GetSize (ec);
					if (basicType.Signedness == Signedness.Signed) {
						switch (size) {
						case 1:
							ec.Emit (OpCode.LoadValue, (ValueType)(sbyte)EmitValue);
							break;
						case 2:
							ec.Emit (OpCode.LoadValue, (ValueType)(short)EmitValue);
							break;
						case 4:
							ec.Emit (OpCode.LoadValue, (ValueType)(int)EmitValue);
							break;
						default:
							throw new NotSupportedException ("Constants with type '" + ConstantType + "'");
						}
					}
					else {
						switch (size) {
						case 1:
							ec.Emit (OpCode.LoadValue, (ValueType)(byte)EmitValue);
							break;
						case 2:
							ec.Emit (OpCode.LoadValue, (ValueType)(ushort)EmitValue);
							break;
						case 4:
							ec.Emit (OpCode.LoadValue, (ValueType)(uint)EmitValue);
							break;
						default:
							throw new NotSupportedException ("Constants with type '" + ConstantType + "'");
						}
					}
				}
				else {
					throw new NotSupportedException ("Constants with type '" + ConstantType + "'");
				}
			} else {
				throw new NotSupportedException ("Constants with type '" + ConstantType + "'");
			}
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
