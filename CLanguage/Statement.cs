using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public abstract class Statement
    {
        public Location Location { get; protected set; }

        public void Emit(EmitContext ec)
        {
            DoEmit(ec);
        }

        protected abstract void DoEmit(EmitContext ec);
    }
}
