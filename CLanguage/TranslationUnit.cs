using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class TranslationUnit : Block
    {
        public TranslationUnit()
            : base(null, Location.Null)
        {
			LocalSymbolName = "_";
        }
		
		public ObjectFile Compile (ReportPrinter printer)
		{
			var o = new ObjectFile ();
			
			var ec = new EmitContext (printer, o);
			
			Emit (ec);
			
			return o;
		}
    }
}
