using System;

namespace CLanguage
{
	public interface IFunction
	{
		string Name { get; }
		CFunctionType FunctionType { get; }

		void Step (ExecutionState state);
	}
}

