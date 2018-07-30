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
		public Label Label;

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

		Jump,
		BranchIfFalse,
		Call,
		Return,

		#region Conversion

		#endregion

		#region Memory

		LoadValue,

		LoadArg,
		LoadLocal,
		LoadGlobal,
        LoadMemoryIndirect,
		StoreArg,
		StoreLocal,
		StoreMemory,

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
		LogicalNot,

		#endregion
	}	
}

