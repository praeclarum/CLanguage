using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
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

        public override CType ExpressionType
        {
            get { return CType.Void; }
        }

        protected override Expression DoResolve(ResolveContext rc)
        {
            throw new NotImplementedException();
        }

        protected override void DoEmit(EmitContext ec)
        {
            throw new NotImplementedException();
        }
    }
}
