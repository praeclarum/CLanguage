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
			if (ConstantType is CIntType intType) {
				return PromoteIntConstant (intType, ec);
			}
			return ConstantType;
        }

		/// <summary>
		/// C11 §6.4.4.1: Integer constants get the first type in the
		/// promotion sequence that can represent the value.
		/// Signed: int → long → long long
		/// Unsigned: unsigned int → unsigned long → unsigned long long
		/// </summary>
		CIntType PromoteIntConstant (CIntType intType, EmitContext ec)
		{
			var mi = ec.MachineInfo;
			var curSize = intType.GetByteSize (mi);

			if (intType.Signedness == Signedness.Signed) {
				long val;
				try { val = Convert.ToInt64 (Value); }
				catch (Exception ex) when (ex is OverflowException || ex is InvalidCastException || ex is FormatException) { return intType; }

				if (FitsInSignedBytes (val, curSize))
					return intType;

				if (mi.LongIntSize > curSize && FitsInSignedBytes (val, mi.LongIntSize))
					return CBasicType.SignedLongInt;

				if (mi.LongLongIntSize > curSize && FitsInSignedBytes (val, mi.LongLongIntSize))
					return CBasicType.SignedLongLongInt;
			}
			else {
				ulong val;
				try { val = Convert.ToUInt64 (Value); }
				catch (Exception ex) when (ex is OverflowException || ex is InvalidCastException || ex is FormatException) { return intType; }

				if (FitsInUnsignedBytes (val, curSize))
					return intType;

				if (mi.LongIntSize > curSize && FitsInUnsignedBytes (val, mi.LongIntSize))
					return CBasicType.UnsignedLongInt;

				if (mi.LongLongIntSize > curSize && FitsInUnsignedBytes (val, mi.LongLongIntSize))
					return CBasicType.UnsignedLongLongInt;
			}

			return intType;
		}

		static bool FitsInSignedBytes (long val, int byteSize)
		{
			switch (byteSize) {
				case 1: return val >= sbyte.MinValue && val <= sbyte.MaxValue;
				case 2: return val >= short.MinValue && val <= short.MaxValue;
				case 4: return val >= int.MinValue && val <= int.MaxValue;
				case 8: return true;
				default: return false;
			}
		}

		static bool FitsInUnsignedBytes (ulong val, int byteSize)
		{
			switch (byteSize) {
				case 1: return val <= byte.MaxValue;
				case 2: return val <= ushort.MaxValue;
				case 4: return val <= uint.MaxValue;
				case 8: return true;
				default: return false;
			}
		}

        protected override void DoEmit (EmitContext ec)
        {
            var cval = EvalConstant (ec);
            ec.Emit (OpCode.LoadConstant, cval);
        }

        public override string ToString()
        {
            return Value.ToString() ?? "";
        }

        public override Value EvalConstant (EmitContext ec)
        {
            var evalType = GetEvaluatedCType (ec);
            if (evalType is CIntType intType) {
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
