using System;

using ValueType = System.Int32;

namespace CLanguage
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
		public ValueType X;
		public Label Label;

		public Instruction (OpCode op, ValueType x)
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

		LoadFunction,
		LoadValue,

		LoadArg,
		LoadLocal,
		LoadMemory,
		StoreArg,
		StoreLocal,
		StoreMemory,

		#endregion

		#region Arithmetic

		AddInt16,
		AddUInt16,
		AddInt32,
		AddUInt32,

		SubtractInt16,
		SubtractUInt16,
		SubtractInt32,
		SubtractUInt32,

		MultiplyInt16,
		MultiplyUInt16,
		MultiplyInt32,
		MultiplyUInt32,

		DivideInt16,
		DivideUInt16,
		DivideInt32,
		DivideUInt32,

		ModuloInt16,
		ModuloUInt16,
		ModuloInt32,
		ModuloUInt32,

		#endregion

		#region Relational

		EqualToInt16,
		EqualToUInt16,
		EqualToInt32,
		EqualToUInt32,

		LessThanInt16,
		LessThanUInt16,
		LessThanInt32,
		LessThanUInt32,

		GreaterThanInt16,
		GreaterThanUInt16,
		GreaterThanInt32,
		GreaterThanUInt32,

		#endregion

		#region Bitwise

		#endregion

		#region Unary

		NotInt16,
		NotUInt16,
		NotInt32,
		NotUInt32,

		NegateInt16,
		NegateUInt16,
		NegateInt32,
		NegateUInt32,

		#endregion

		#region Boolean

		LogicalAnd,
		LogicalOr,
		LogicalNot,

		#endregion
	}	
}

