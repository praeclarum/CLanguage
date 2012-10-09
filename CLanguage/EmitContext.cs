using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class EmitContext
    {
		public enum VariableScope
		{
			Global,
			Local,
			Arg
		}

		public class ResolvedVariable
		{
			public VariableScope Scope { get; private set; }
			public int Index { get; private set; }

			public ResolvedVariable (VariableScope scope, int index)
			{
				Scope = scope;
				Index = index;
			}
		}

		public FunctionDeclaration FunctionDecl { get; private set; }

		public MachineInfo MachineInfo { get; private set; }
		
		public Report Report { get; private set; }

        public EmitContext (FunctionDeclaration fdecl, Report report)
        {
			FunctionDecl = fdecl;
			MachineInfo = MachineInfo.WindowsX86;
			Report = report;
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
        
        public virtual void EmitCall(CFunctionType type, int argsCount)
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
