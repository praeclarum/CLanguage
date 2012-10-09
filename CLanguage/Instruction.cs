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

	public abstract class Instruction
	{
		public Instruction ()
		{
		}
	}

	public class PushInstruction : Instruction
	{
		object value;

		public PushInstruction (object value, CType type)
		{
			this.value = value;
		}

		public override string ToString ()
		{
			return string.Format ("PUSH {0}", value);
		}
	}

	public class LoadArgInstruction : Instruction
	{
		int index;
		public LoadArgInstruction (int index)
		{
			this.index = index;
		}
		public override string ToString ()
		{
			return string.Format ("LOADARG {0}", index);
		}
	}

	public class BranchIfFalseInstruction : Instruction
	{
		Label label;

		public BranchIfFalseInstruction (Label label)
		{
			this.label = label;
		}
		public override string ToString ()
		{
			return string.Format ("BRFALSE {0}", label);
		}
	}

	public class JumpInstruction : Instruction
	{
		Label label;

		public JumpInstruction (Label label)
		{
			this.label = label;
		}
		public override string ToString ()
		{
			return string.Format ("JUMP {0}", label);
		}
	}

	public class ReturnInstruction : Instruction
	{
		public override string ToString ()
		{
			return string.Format ("RETURN");
		}
	}

	public class BinopInstruction : Instruction
	{
		Binop binop;
		public BinopInstruction (Binop binop)
		{
			this.binop = binop;
		}
		public override string ToString ()
		{
			return binop.ToString ();
		}
	}
}

