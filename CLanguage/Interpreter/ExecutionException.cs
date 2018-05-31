using System;

namespace CLanguage.Interpreter
{
    public class ExecutionException : Exception
    {
        public ExecutionException (string message)
            : base (message)
        {
        }

        public ExecutionException (string message, Exception innerException)
            : base (message, innerException)
        {
        }
    }
}
