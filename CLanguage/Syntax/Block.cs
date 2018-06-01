using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Types;
using CLanguage.Interpreter;

namespace CLanguage.Syntax
{
    public class Block : Statement
    {
        public Location StartLocation { get; private set; } = Location.Null;
        public List<Statement> Statements { get; private set; } = new List<Statement> ();
        public Location EndLocation { get; set; } = Location.Null;

        public List<VariableDeclaration> Variables { get; private set; } = new List<VariableDeclaration> ();
        public List<CompiledFunction> Functions { get; private set; } = new List<CompiledFunction> ();
        public Dictionary<string, CType> Typedefs { get; private set; } = new Dictionary<string, CType> ();

        public Block ()
        {
        }

        public Block (Location startLoc, List<Statement> statements, Location endLoc)
        {
            StartLocation = startLoc;
            Statements.AddRange (statements);
            EndLocation = endLoc;
        }

        public Block (Location startLoc, Location endLoc)
        {
            StartLocation = startLoc;
            EndLocation = endLoc;
        }

        public Block(Block parent, Location startLoc)
        {
            StartLocation = startLoc;
            EndLocation = Location.Null;
        }

        public void AddStatement(Statement stmt)
        {
            Statements.Add(stmt);
        }

        protected override void DoEmit(EmitContext ec)
        {
            ec.BeginBlock(this);
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
