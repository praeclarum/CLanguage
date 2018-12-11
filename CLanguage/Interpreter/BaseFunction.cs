using System;
using CLanguage.Compiler;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
	public abstract class BaseFunction
	{
        public string Name { get; protected set; } = "";
        public string NameContext { get; protected set; } = "";
        public CFunctionType FunctionType { get; protected set; } = CFunctionType.VoidProcedure;

		public virtual void Init (CInterpreter state) {}
		public abstract void Step (CInterpreter state, ExecutionFrame frame);

        public override string ToString () => string.IsNullOrEmpty (NameContext) ? Name : NameContext + "::" + Name;
    }
}

