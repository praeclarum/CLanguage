using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Interpreter;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class SwitchStatement : Statement
    {
        public Expression Value { get; private set; }
        public List<SwitchCase> Cases { get; private set; }

        public SwitchStatement(Expression value, List<SwitchCase> cases, Location loc)
        {
            if (value == null) throw new ArgumentNullException (nameof (value));
            if (cases == null) throw new ArgumentNullException (nameof (cases));
            Value = value;
            Cases = cases;
            Location = loc;
        }

        protected override void DoEmit (EmitContext ec)
        {
            Value.Emit(ec);

            if (Cases.Count == 0) {
                ec.Emit(OpCode.Pop);
                return;
            }

            var caseLabels = new List<Label>();

			throw new NotImplementedException();
        }

        public override string ToString()
        {
            return string.Format("switch ({0}) {1};", Value, String.Join("", Cases));
        }

        public override void AddDeclarationToBlock (BlockContext context)
        {
            foreach (var c in Cases) {
                foreach (var s in c.Statements) {
                    s.AddDeclarationToBlock (context);
                }
            }
        }

        public override bool AlwaysReturns {
			get {
				return false;
			}
		}
    }

    public class SwitchCase
    {
        public Expression? Value { get; private set; }
        public List<Statement> Statements { get; private set; }

        public SwitchCase(Expression? value, List<Statement> statements)
        {
            Value = value;
            Statements = statements;
        }
    }
}
