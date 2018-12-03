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
        public Block Parent { get; set; }
        public List<Statement> Statements { get; private set; } = new List<Statement> ();

        public List<CompiledVariable> Variables { get; private set; } = new List<CompiledVariable> ();
        public List<CompiledFunction> Functions { get; private set; } = new List<CompiledFunction> ();
        public Dictionary<string, CType> Typedefs { get; private set; } = new Dictionary<string, CType> ();
        public List<Statement> InitStatements { get; private set; } = new List<Statement> ();
        public Dictionary<string, CStructType> Structures { get; private set; } = new Dictionary<string, CStructType> ();

        public Block (IEnumerable<Statement> statements)
        {
            AddStatements (statements);
        }

        public Block ()
        {
        }

        public void AddStatement (Statement stmt)
        {
            Statements.Add (stmt);

            if (stmt is Block block) {
                block.Parent = this;
            }
        }

        public void AddStatements (IEnumerable<Statement> stmts)
        {
            foreach (var s in stmts) {
                AddStatement (s);
            }
        }

        protected override void DoEmit (EmitContext ec)
        {
            ec.BeginBlock (this);
            foreach (var s in InitStatements) {
                s.Emit (ec);
            }
            foreach (var s in Statements) {
                s.Emit (ec);
            }
            ec.EndBlock ();
        }

        public override bool AlwaysReturns {
            get {
                return Statements.Any (s => s.AlwaysReturns);
            }
        }

        public void AddVariable (string name, CType ctype)
        {
            Variables.Add (new CompiledVariable (name, 0, ctype));
        }

        public CType LookupTypename (string name)
        {
            return Typedefs.TryGetValue (name, out var t) ? t : Parent?.LookupTypename (name);
        }
    }
}
