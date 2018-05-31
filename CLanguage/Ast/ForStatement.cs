using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage.Ast
{
    public class ForStatement : Statement
    {
        public Block InitBlock { get; private set; }
        public Expression ContinueExpression { get; private set; }
        public Expression NextExpression { get; private set; }
        public Statement LoopBody { get; private set; }

        public ForStatement(Block decls, Expression continueExpr, Statement body)
        {
            InitBlock = decls;
            ContinueExpression = continueExpr;
            LoopBody = body;
        }

        public ForStatement(Block decls, Expression continueExpr, Expression nextExpr, Statement body)
        {
            InitBlock = decls;
            ContinueExpression = continueExpr;
            NextExpression = nextExpr;
            LoopBody = body;
        }

	    public ForStatement(ExpressionStatement initStatement, Expression continueExpr, Statement body, Block parent, Location startLoc, Location endLoc)
        {
            InitBlock = new Block(parent, startLoc)
            {
                EndLocation = endLoc
            };
			InitBlock.Statements.Add (initStatement);
            ContinueExpression = continueExpr;
            LoopBody = body;
            if (LoopBody is Block)
            {
                ((Block)LoopBody).Parent = InitBlock;
            }
        }

		public ForStatement(ExpressionStatement initStatement, Expression continueExpr, Expression nextExpr, Statement body, Block parent, Location startLoc, Location endLoc)
        {
            InitBlock = new Block(parent, startLoc)
            {
                EndLocation = endLoc
            };
			InitBlock.Statements.Add (initStatement);
            ContinueExpression = continueExpr;
            NextExpression = nextExpr;
            LoopBody = body;
            if (LoopBody is Block)
            {
                ((Block)LoopBody).Parent = InitBlock;
            }
        }

        protected override void DoEmit (EmitContext ec)
        {
			InitBlock.Emit (ec);

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
        }

		public override bool AlwaysReturns {
			get {
				return false;
			}
		}
    }
}
