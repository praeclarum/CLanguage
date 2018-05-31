using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Types;

namespace CLanguage.Ast
{
    public class Block : Statement
    {
		public string LocalSymbolName { get; set; }
		
        public Block Parent { get; set; }
        public Location StartLocation { get; private set; }
        public Location EndLocation { get; set; }

        public List<Declaration> Declarations { get; private set; }
        public List<VariableDeclaration> Variables { get; private set; }
        public List<FunctionDeclaration> Functions { get; private set; }
        public Dictionary<string, CType> Typedefs { get; private set; }

        public List<Statement> Statements { get; private set; }        

        public Block(Block parent, Location startLoc)
        {
            Parent = parent;
            StartLocation = startLoc;
            EndLocation = Location.Null;

            Typedefs = new Dictionary<string, CType>();
            Declarations = new List<Declaration>();
            Variables = new List<VariableDeclaration>();
            Functions = new List<FunctionDeclaration>();
            Statements = new List<Statement>();
        }

        public void AddStatement(Statement stmt)
        {
            Statements.Add(stmt);
        }

        public void AddVariable(VariableDeclaration dec)
        {
            Declarations.Add(dec);
            Variables.Add(dec);
        }

        public void AddFunction(FunctionDeclaration dec)
        {
            Declarations.Add(dec);
            Functions.Add(dec);
        }

        protected override void DoEmit(EmitContext ec)
        {
            ec.BeginBlock(this);
            foreach (var dec in Declarations)
            {
                dec.Emit(ec);
            }
            foreach (var stmt in Statements)
            {
                stmt.Emit(ec);
            }
            ec.EndBlock();
        }

		public override bool AlwaysReturns {
			get {
				return Statements.Any (s => s.AlwaysReturns);
			}
		}
    }
}
