using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Interpreter;

namespace CLanguage.Syntax
{
    public class ForStatement : Statement
    {
        public Block InitBlock { get; private set; }
        public Expression ContinueExpression { get; private set; }
        public Expression NextExpression { get; private set; }
        public Block LoopBody { get; private set; }

        public ForStatement (Statement initStatement, Expression continueExpr, Block body)
        {
            InitBlock = new Block ();
            if (initStatement != null) {
                InitBlock.AddStatement (initStatement);
            }
            ContinueExpression = continueExpr;
            LoopBody = body;
        }

        public ForStatement (Statement initStatement, Expression continueExpr, Expression nextExpr, Block body)
        {
            InitBlock = new Block ();
            if (initStatement != null) {
                InitBlock.AddStatement (initStatement);
            }
            ContinueExpression = continueExpr;
            NextExpression = nextExpr;
            LoopBody = body;
        }

        protected override void DoEmit (EmitContext ec)
        {
            ec.BeginBlock (InitBlock);
            foreach (var s in InitBlock.InitStatements) {
                s.Emit (ec);
            }
            foreach (var s in InitBlock.Statements) {
                s.Emit (ec);
            }

			var endLabel = ec.DefineLabel ();

			var contLabel = ec.DefineLabel ();
			ec.EmitLabel (contLabel);
			ContinueExpression.Emit (ec);
			ec.EmitCastToBoolean (ContinueExpression.GetEvaluatedCType (ec));
			ec.Emit (OpCode.BranchIfFalse, endLabel);

            LoopBody.Emit (ec);

			NextExpression.Emit (ec);
			ec.Emit (OpCode.Pop);
			ec.Emit (OpCode.Jump, contLabel);

			ec.EmitLabel (endLabel);

            ec.EndBlock ();
        }

        public override bool AlwaysReturns => false;
    }
}
