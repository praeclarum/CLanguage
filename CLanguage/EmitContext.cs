using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ValueType = System.Int32;

namespace CLanguage
{
    public class EmitContext : CompilerContext
    {
		public FunctionDeclaration FunctionDecl { get; private set; }

		public EmitContext (MachineInfo machineInfo, Report report, FunctionDeclaration fdecl = null)
			: base (machineInfo, report)
        {
			FunctionDecl = fdecl;
        }

		public virtual ResolvedVariable ResolveVariable (string name)
		{
			return null;
		}

        public virtual void DeclareVariable (VariableDeclaration v) { }
        public virtual void DeclareFunction (FunctionDeclaration f) { }

        public virtual void BeginBlock(Block b) { }
        public virtual void EndBlock() { }

        public virtual Label DefineLabel ()
		{
			return new Label ();
		}

        public virtual void EmitLabel (Label l)
        {
        }

		public void EmitCast (CType fromType, CType toType)
		{
			if (!fromType.Equals (toType)) {
				var fromBasicType = fromType as CBasicType;
				var toBasicType = toType as CBasicType;

				if (fromBasicType != null && fromBasicType.IsIntegral && toBasicType != null && toBasicType.IsIntegral) {
					// This conversion is implicit with how the evaluator stores its stuff
				}
				else {
					Report.Error (30, "Cannot convert type '" + fromType + "' to '" + toType + "'");
				}
			}
		}

		public void EmitCastToBoolean (CType fromType)
		{
			// Don't really need to do anything since 0 on that eval stack is false
		}

		public virtual void Emit (Instruction instruction)
		{
		}

		public void Emit (OpCode op, ValueType x)
		{
			Emit (new Instruction (op, x));
		}

		public void Emit (OpCode op, Label label)
		{
			Emit (new Instruction (op, label));
		}

		public void Emit (OpCode op)
		{
			Emit (op, 0);
		}
    }

	public enum VariableScope
	{
		Global,
		Arg,
		Local,
		Function,
	}

	public class ResolvedVariable
	{
		public VariableScope Scope { get; private set; }
		public int Index { get; private set; }
		public CType VariableType { get; private set; }
		public IFunction Function { get; private set; }

		public ResolvedVariable (VariableScope scope, int index, CType variableType)
		{
			Scope = scope;
			Index = index;
			VariableType = variableType;
		}

		public ResolvedVariable (IFunction function)
		{
			Scope = VariableScope.Function;
			Function = function;
			VariableType = Function.FunctionType;
		}
	}
}
