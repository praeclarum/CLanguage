using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public abstract class Op
    {
    }

	public class PopOp : Op
	{
		public override string ToString ()
		{
			return "POP";
		}
	}
}
