using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class EmitContext : CompilerContext
    {
		public enum VariableScope
		{
			Global,
			Local,
			Arg,
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

        public virtual void DeclareVariable(VariableDeclaration v) { }
        public virtual void DeclareFunction(FunctionDeclaration f) { }

        public virtual void BeginBlock(Block b) { }
        public virtual void EndBlock() { }

        public virtual Label DefineLabel()
		{
			return new Label ();
		}

        public virtual void EmitLabel(Label l)
        {
        }

        public virtual void EmitJump(Label l)
        {
        }
        
		public virtual void EmitBranchIfFalse(Label l)
        {
        }

		public virtual void EmitReturn ()
		{
		}

        public virtual void EmitBinop(Binop op)
        {
        }

        public virtual void EmitUnop(Unop op)
        {
        }
        
        public virtual void EmitCall(CFunctionType type)
        {
        }

        public virtual void EmitConstant(object value, CType type)
        {
        }

        public virtual void EmitAssign(Expression left)
        {
        }

        public virtual void EmitVariable(ResolvedVariable variable)
        {
        }

        public virtual void EmitPop()
        {
        }
    }
}
