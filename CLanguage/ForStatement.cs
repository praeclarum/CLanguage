using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
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

        public ForStatement(Expression initExpr, Expression continueExpr, Statement body, Block parent, Location startLoc, Location endLoc)
        {
            InitBlock = new Block(parent, startLoc)
            {
                EndLocation = endLoc
            };
            ContinueExpression = continueExpr;
            LoopBody = body;
            if (LoopBody is Block)
            {
                ((Block)LoopBody).Parent = InitBlock;
            }
        }

        public ForStatement(Expression initExpr, Expression continueExpr, Expression nextExpr, Statement body, Block parent, Location startLoc, Location endLoc)
        {
            InitBlock = new Block(parent, startLoc)
            {
                EndLocation = endLoc
            };
            ContinueExpression = continueExpr;
            NextExpression = nextExpr;
            LoopBody = body;
            if (LoopBody is Block)
            {
                ((Block)LoopBody).Parent = InitBlock;
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            throw new NotImplementedException();
        }

		public override bool AlwaysReturns {
			get {
				return false;
			}
		}
    }
}
