using System;

namespace CLanguage.Interpreter
{
	public class Label
	{
		public int Index { get; set; }

		public override string ToString ()
		{
			return "L_" + Index;
		}
	}

	public class Instruction
	{
		public OpCode Op;
		public Value X;
		public Label? Label;

		public Instruction (OpCode op, Value x)
		{
			Op = op;
			X = x;
		}

		public Instruction (OpCode op, Label label)
		{
			Op = op;
			Label = label;
		}

		public override string ToString ()
		{
			if (Label != null) {
				return string.Format ("{0} {1}", Op, Label);
			} else {
				return string.Format ("{0} {1}", Op, X);
			}
		}
	}

    public enum OpCode
    {
        Nop,
        Pop,
        Dup,

        #region Control

        Jump,
        BranchIfFalse,
        Call,
        Return,

        #endregion

        #region Memory

        LoadConstant,
        LoadFramePointer,

        LoadArg,
        LoadLocal,
        LoadGlobal,

        LoadPointer,

        StoreArg,
        StoreLocal,
        StoreGlobal,

        StorePointer,

        #endregion

        #region Arithmetic

        OffsetPointer,

        AddInt8,
        AddUInt8,
        AddInt16,
        AddUInt16,
        AddInt32,
        AddUInt32,
        AddInt64,
        AddUInt64,
        AddFloat32,
        AddFloat64,

        SubtractInt8,
        SubtractUInt8,
        SubtractInt16,
        SubtractUInt16,
        SubtractInt32,
        SubtractUInt32,
        SubtractInt64,
        SubtractUInt64,
        SubtractFloat32,
        SubtractFloat64,

        MultiplyInt8,
        MultiplyUInt8,
        MultiplyInt16,
        MultiplyUInt16,
        MultiplyInt32,
        MultiplyUInt32,
        MultiplyInt64,
        MultiplyUInt64,
        MultiplyFloat32,
        MultiplyFloat64,

        DivideInt8,
        DivideUInt8,
        DivideInt16,
        DivideUInt16,
        DivideInt32,
        DivideUInt32,
        DivideInt64,
        DivideUInt64,
        DivideFloat32,
        DivideFloat64,

        ShiftLeftInt8,
        ShiftLeftUInt8,
        ShiftLeftInt16,
        ShiftLeftUInt16,
        ShiftLeftInt32,
        ShiftLeftUInt32,
        ShiftLeftInt64,
        ShiftLeftUInt64,
        ShiftLeftFloat32,
        ShiftLeftFloat64,

        ShiftRightInt8,
        ShiftRightUInt8,
        ShiftRightInt16,
        ShiftRightUInt16,
        ShiftRightInt32,
        ShiftRightUInt32,
        ShiftRightInt64,
        ShiftRightUInt64,
        ShiftRightFloat32,
        ShiftRightFloat64,

        ModuloInt8,
        ModuloUInt8,
        ModuloInt16,
        ModuloUInt16,
        ModuloInt32,
        ModuloUInt32,
        ModuloInt64,
        ModuloUInt64,
        ModuloFloat32,
        ModuloFloat64,

        #endregion

        #region Relational

        EqualToInt8,
        EqualToUInt8,
        EqualToInt16,
        EqualToUInt16,
        EqualToInt32,
        EqualToUInt32,
        EqualToInt64,
        EqualToUInt64,
        EqualToFloat32,
        EqualToFloat64,

        LessThanInt8,
        LessThanUInt8,
        LessThanInt16,
        LessThanUInt16,
        LessThanInt32,
        LessThanUInt32,
        LessThanInt64,
        LessThanUInt64,
        LessThanFloat32,
        LessThanFloat64,

        GreaterThanInt8,
        GreaterThanUInt8,
        GreaterThanInt16,
        GreaterThanUInt16,
        GreaterThanInt32,
        GreaterThanUInt32,
        GreaterThanInt64,
        GreaterThanUInt64,
        GreaterThanFloat32,
        GreaterThanFloat64,

        #endregion

        #region Bitwise

        BinaryAndInt8,
        BinaryAndUInt8,
        BinaryAndInt16,
        BinaryAndUInt16,
        BinaryAndInt32,
        BinaryAndUInt32,
        BinaryAndInt64,
        BinaryAndUInt64,
        BinaryAndFloat32,
        BinaryAndFloat64,

        BinaryOrInt8,
        BinaryOrUInt8,
        BinaryOrInt16,
        BinaryOrUInt16,
        BinaryOrInt32,
        BinaryOrUInt32,
        BinaryOrInt64,
        BinaryOrUInt64,
        BinaryOrFloat32,
        BinaryOrFloat64,

        BinaryXorInt8,
        BinaryXorUInt8,
        BinaryXorInt16,
        BinaryXorUInt16,
        BinaryXorInt32,
        BinaryXorUInt32,
        BinaryXorInt64,
        BinaryXorUInt64,
        BinaryXorFloat32,
        BinaryXorFloat64,

        #endregion

        #region Unary

        NotInt8,
        NotUInt8,
        NotInt16,
        NotUInt16,
        NotInt32,
        NotUInt32,
        NotInt64,
        NotUInt64,
        NotFloat32,
        NotFloat64,

        BinaryNotInt8,
        BinaryNotUInt8,
        BinaryNotInt16,
        BinaryNotUInt16,
        BinaryNotInt32,
        BinaryNotUInt32,
        BinaryNotInt64,
        BinaryNotUInt64,
        BinaryNotFloat32,
        BinaryNotFloat64,

        NegateInt8,
        NegateUInt8,
        NegateInt16,
        NegateUInt16,
        NegateInt32,
        NegateUInt32,
        NegateInt64,
        NegateUInt64,
        NegateFloat32,
        NegateFloat64,

        #endregion

        #region Boolean

        LogicalAnd,
        LogicalOr,

        #endregion

        #region Conversion

        ConvertInt8Int8,
        ConvertInt8UInt8,
        ConvertInt8Int16,
        ConvertInt8UInt16,
        ConvertInt8Int32,
        ConvertInt8UInt32,
        ConvertInt8Int64,
        ConvertInt8UInt64,
        ConvertInt8Float32,
        ConvertInt8Float64,

        ConvertUInt8Int8,
        ConvertUInt8UInt8,
        ConvertUInt8Int16,
        ConvertUInt8UInt16,
        ConvertUInt8Int32,
        ConvertUInt8UInt32,
        ConvertUInt8Int64,
        ConvertUInt8UInt64,
        ConvertUInt8Float32,
        ConvertUInt8Float64,

        ConvertInt16Int8,
        ConvertInt16UInt8,
        ConvertInt16Int16,
        ConvertInt16UInt16,
        ConvertInt16Int32,
        ConvertInt16UInt32,
        ConvertInt16Int64,
        ConvertInt16UInt64,
        ConvertInt16Float32,
        ConvertInt16Float64,

        ConvertUInt16Int8,
        ConvertUInt16UInt8,
        ConvertUInt16Int16,
        ConvertUInt16UInt16,
        ConvertUInt16Int32,
        ConvertUInt16UInt32,
        ConvertUInt16Int64,
        ConvertUInt16UInt64,
        ConvertUInt16Float32,
        ConvertUInt16Float64,

        ConvertInt32Int8,
        ConvertInt32UInt8,
        ConvertInt32Int16,
        ConvertInt32UInt16,
        ConvertInt32Int32,
        ConvertInt32UInt32,
        ConvertInt32Int64,
        ConvertInt32UInt64,
        ConvertInt32Float32,
        ConvertInt32Float64,

        ConvertUInt32Int8,
        ConvertUInt32UInt8,
        ConvertUInt32Int16,
        ConvertUInt32UInt16,
        ConvertUInt32Int32,
        ConvertUInt32UInt32,
        ConvertUInt32Int64,
        ConvertUInt32UInt64,
        ConvertUInt32Float32,
        ConvertUInt32Float64,

        ConvertInt64Int8,
        ConvertInt64UInt8,
        ConvertInt64Int16,
        ConvertInt64UInt16,
        ConvertInt64Int32,
        ConvertInt64UInt32,
        ConvertInt64Int64,
        ConvertInt64UInt64,
        ConvertInt64Float32,
        ConvertInt64Float64,

        ConvertUInt64Int8,
        ConvertUInt64UInt8,
        ConvertUInt64Int16,
        ConvertUInt64UInt16,
        ConvertUInt64Int32,
        ConvertUInt64UInt32,
        ConvertUInt64Int64,
        ConvertUInt64UInt64,
        ConvertUInt64Float32,
        ConvertUInt64Float64,

        ConvertFloat32Int8,
        ConvertFloat32UInt8,
        ConvertFloat32Int16,
        ConvertFloat32UInt16,
        ConvertFloat32Int32,
        ConvertFloat32UInt32,
        ConvertFloat32Int64,
        ConvertFloat32UInt64,
        ConvertFloat32Float32,
        ConvertFloat32Float64,

        ConvertFloat64Int8,
        ConvertFloat64UInt8,
        ConvertFloat64Int16,
        ConvertFloat64UInt16,
        ConvertFloat64Int32,
        ConvertFloat64UInt32,
        ConvertFloat64Int64,
        ConvertFloat64UInt64,
        ConvertFloat64Float32,
        ConvertFloat64Float64,

        ConvertPointerInt8,
        ConvertPointerUInt8,
        ConvertPointerInt16,
        ConvertPointerUInt16,
        ConvertPointerInt32,
        ConvertPointerUInt32,
        ConvertPointerInt64,
        ConvertPointerUInt64,
        ConvertPointerFloat32,
        ConvertPointerFloat64,

        #endregion
    }
}

