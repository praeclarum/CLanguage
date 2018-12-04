using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Interpreter;
using CLanguage.Types;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class StructureExpression : Expression
    {        
        public class Item
        {
            public int Index;
            public string Field;
            public Expression Expression;
        }

        public List<Item> Items { get; private set; }

        public StructureExpression()
        {
            Items = new List<Item>();
        }

        public override string ToString ()
        {
            return "{ " + string.Join (", ", Items.Select (x => x.Expression.ToString ())) + " }";
        }

        public override CType GetEvaluatedCType (EmitContext ec)
		{
			return CType.Void;
        }

        protected override void DoEmit(EmitContext ec)
        {
            throw new NotImplementedException();
        }
    }
}
