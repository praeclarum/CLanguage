using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Interpreter;
using CLanguage.Types;

namespace CLanguage.Syntax
{
    public abstract class Expression
    {
        public Location Location { get; protected set; }

        public bool HasError { get; set; }

        public void Emit(EmitContext ec)
        {
            DoEmit(ec);
        }

        public void EmitPointer (EmitContext ec)
        {
            DoEmitPointer (ec);
        }

		public abstract CType GetEvaluatedCType (EmitContext ec);

        protected abstract void DoEmit(EmitContext ec);
        protected virtual void DoEmitPointer (EmitContext ec) =>
            throw new NotSupportedException ($"Cannot get address of {this.GetType().Name} `{this}`");

		protected static int GetInstructionOffset (CBasicType aType, EmitContext ec)
		{
			if (!aType.IsIntegral) {
				throw new NotSupportedException ("Arithmetic on type '" + aType + "'");
			}

			var ioff = 0;
			var size = aType.GetSize (ec);
			if (size == 2) {
				ioff = 0;
			}
			else if (size == 4) {
				ioff = 2;
			}
			else {
				throw new NotSupportedException ("Arithmetic on type '" + aType + "'");
			}

			if (aType.Signedness == Signedness.Unsigned) {
				ioff += 1;
			}

			return ioff;
		}

		protected static CBasicType GetPromotedType (Expression expr, string op, EmitContext ec)
		{
			var leftType = expr.GetEvaluatedCType (ec);

			var leftBasicType = leftType as CBasicType;

			if (leftBasicType == null) {
				ec.Report.Error (19, "Operator '" + op + "' cannot be applied to operand of type '" + leftType + "'");
				return CBasicType.SignedInt;
			} else {
				return leftBasicType.IntegerPromote (ec);
			}
		}

		protected static CBasicType GetArithmeticType (Expression leftExpr, Expression rightExpr, string op, EmitContext ec)
		{
			var leftType = leftExpr.GetEvaluatedCType (ec);
			var rightType = rightExpr.GetEvaluatedCType (ec);

			var leftBasicType = leftType as CBasicType;
			var rightBasicType = rightType as CBasicType;

			if (leftBasicType == null || rightBasicType == null) {
				ec.Report.Error (19, "Operator '" + op + "' cannot be applied to operands of type '" + leftType + "' and '" + rightType + "'");
				return CBasicType.SignedInt;
			} else {
				return leftBasicType.ArithmeticConvert (rightBasicType, ec);
			}
		}
    }
}
