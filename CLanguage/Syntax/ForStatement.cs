using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Interpreter;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class ForStatement : Statement
    {
        public Block InitBlock { get; private set; }
        public Expression ContinueExpression { get; private set; }
        public Expression? NextExpression { get; private set; }
        public Block LoopBody { get; private set; }

        public ForStatement (Statement initStatement, Expression continueExpr, Block body)
        {
            InitBlock = new Block (VariableScope.Local);
            if (initStatement != null) {
                InitBlock.AddStatement (initStatement);
            }
            ContinueExpression = continueExpr;
            LoopBody = body;
        }

        public ForStatement (Statement initStatement, Expression continueExpr, Expression nextExpr, Block body)
        {
            InitBlock = new Block (VariableScope.Local);
            if (initStatement != null) {
                InitBlock.AddStatement (initStatement);
            }
            ContinueExpression = continueExpr;
            NextExpression = nextExpr;
            LoopBody = body;
        }

        public override string ToString ()
        {
            return $"for ({InitBlock}; {ContinueExpression}; {NextExpression}) {LoopBody}";
        }

        protected override void DoEmit (EmitContext initialContext)
        {
            initialContext.BeginBlock (InitBlock);
            foreach (var s in InitBlock.InitStatements) {
                s.Emit (initialContext);
            }
            foreach (var s in InitBlock.Statements) {
                s.Emit (initialContext);
            }

            var nextLabel = initialContext.DefineLabel ();
            var endLabel = initialContext.DefineLabel ();

            var ec = initialContext.PushLoop (breakLabel: endLabel, continueLabel: nextLabel);

            //
            // Test the condition:
            //   If succeeded then execute the loop body
            //   If failed, goto END
            //
			var conditionLabel = ec.DefineLabel ();
			ec.EmitLabel (conditionLabel);
			ContinueExpression.Emit (ec);
			ec.EmitCastToBoolean (ContinueExpression.GetEvaluatedCType (ec));
			ec.Emit (OpCode.BranchIfFalse, endLabel);

            //
            // Fall through to the loop body
            //
            LoopBody.Emit (ec);

            //
            // Fall through to the NEXT label
            //   Run the incrementer
            //   Loop back to the condition
            //
            ec.EmitLabel (nextLabel);
            if (NextExpression != null) {
                NextExpression.Emit (ec);
                ec.Emit (OpCode.Pop);
            }
			ec.Emit (OpCode.Jump, conditionLabel);

            //
            // Arrive here when the condition fails
            //
			ec.EmitLabel (endLabel);

            ec.EndBlock ();
        }

        public override void AddDeclarationToBlock (BlockContext context)
        {
            InitBlock.AddDeclarationToBlock (context);
            LoopBody.AddDeclarationToBlock (context);
        }

        public override bool AlwaysReturns => false;
    }
}
