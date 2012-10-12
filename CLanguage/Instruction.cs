using System;

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
		OpCode op;
		Int32 x;

		Instruction (OpCode op, int x)
		{
			this.op = op;
			this.x = x;
		}

		public static Instruction LoadValue (Int32 value)
		{
			return new Instruction (OpCode.LoadValueInt32, value);
		}

		public override string ToString ()
		{
			return string.Format ("{0} {1}", op, x);
		}
	}

	public enum OpCode
	{
		Nop,

		Jump,
		BranchIfFalse,
		Call,
		Return,

		#region Memory

		LoadValueInt8,
		LoadValueUInt8,
		LoadValueInt16,
		LoadValueUInt16,
		LoadValueInt32,
		LoadValueUInt32,

		LoadArgInt8,
		LoadArgUInt8,
		LoadArgInt16,
		LoadArgUInt16,
		LoadArgInt32,
		LoadArgUInt32,

		LoadLocalInt8,
		LoadLocalUInt8,
		LoadLocalInt16,
		LoadLocalUInt16,
		LoadLocalInt32,
		LoadLocalUInt32,

		LoadMemoryInt8,
		LoadMemoryUInt8,
		LoadMemoryInt16,
		LoadMemoryUInt16,
		LoadMemoryInt32,
		LoadMemoryUInt32,

		StoreArgInt8,
		StoreArgUInt8,
		StoreArgInt16,
		StoreArgUInt16,
		StoreArgInt32,
		StoreArgUInt32,

		StoreLocalInt8,
		StoreLocalUInt8,
		StoreLocalInt16,
		StoreLocalUInt16,
		StoreLocalInt32,
		StoreLocalUInt32,

		StoreMemoryInt8,
		StoreMemoryUInt8,
		StoreMemoryInt16,
		StoreMemoryUInt16,
		StoreMemoryInt32,
		StoreMemoryUInt32,

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

		MultiplyInt8,
		MultiplyUInt8,
		MultiplyInt16,
		MultiplyUInt16,
		MultiplyInt32,
		MultiplyUInt32,

		DivideInt8,
		DivideUInt8,
		DivideInt16,
		DivideUInt16,
		DivideInt32,
		DivideUInt32,

		ModuloInt8,
		ModuloUInt8,
		ModuloInt16,
		ModuloUInt16,
		ModuloInt32,
		ModuloUInt32,

		#endregion

		#region Comparison

		EqualToInt8,
		EqualToUInt8,
		EqualToInt16,
		EqualToUInt16,
		EqualToInt32,
		EqualToUInt32,

		LessThanInt8,
		LessThanUInt8,
		LessThanInt16,
		LessThanUInt16,
		LessThanInt32,
		LessThanUInt32,

		GreaterThanInt8,
		GreaterThanUInt8,
		GreaterThanInt16,
		GreaterThanUInt16,
		GreaterThanInt32,
		GreaterThanUInt32,

		#endregion

		#region Boolean

		LogicalNotInt16,
		LogicalOrInt16,
		LogicalAndInt16,

		#endregion

		#region Bitwise

		#endregion
	}	
}

