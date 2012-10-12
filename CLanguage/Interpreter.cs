using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class Interpreter
    {
		Executable exe;

        public Interpreter (Executable exe)
        {
			this.exe = exe;
        }

		public void Start (string entrypoint)
		{
			var f = exe.Functions.First (x => x.Name == entrypoint);
		}
    }
}
