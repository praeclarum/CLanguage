using System;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
	public abstract class BaseFunction
	{
		public string Name { get; protected set; }
		public CFunctionType FunctionType { get; protected set; }

		public virtual void Init (ExecutionState state) {}
		public abstract void Step (ExecutionState state);
	}
}

