using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Compiler;
using CLanguage.Types;

namespace CLanguage.Syntax
{
    public abstract class Expression
    {
        public Location Location { get; protected set; }
        public Location EndLocation { get; protected set; }

        public bool HasError { get; set; }

        public void Emit (EmitContext ec)
        {
            DoEmit(ec);
        }

		public virtual bool CanEmitPointer => false;
        public void EmitPointer (EmitContext ec)
        {
            DoEmitPointer (ec);
        }

		public abstract CType GetEvaluatedCType (EmitContext ec);

        protected abstract void DoEmit(EmitContext ec);
        protected virtual void DoEmitPointer (EmitContext ec) =>
            throw new NotSupportedException ($"Cannot get address of {this.GetType().Name} `{this}`");

		protected static CType GetPromotedType (Expression expr, string op, EmitContext ec)
		{
			var leftType = expr.GetEvaluatedCType (ec);

			var leftBasicType = leftType as CBasicType;

			if (leftBasicType != null) {
				return leftBasicType.IntegerPromote (ec);
			}
			else if (leftType is CArrayType laType) {
				return laType.ElementType.Pointer;
			}
			else if (leftType is CPointerType) {
				return leftType;
			}
			else {
				ec.Report.Error (19, "'" + op + "' cannot be applied to operand of type '" + leftType + "'");
				return CBasicType.SignedInt;
			}
		}

		protected static CType GetArithmeticType (Expression leftExpr, Expression rightExpr, string op, EmitContext ec)
		{
			var leftType = leftExpr.GetEvaluatedCType (ec);
			var rightType = rightExpr.GetEvaluatedCType (ec);

			var leftBasicType = leftType as CBasicType;
			var rightBasicType = rightType as CBasicType;

			if (leftBasicType != null && rightBasicType != null) {
				return leftBasicType.ArithmeticConvert (rightBasicType, ec);
			}
			else if (leftType is CPointerType lpType && rightBasicType != null) {
				return lpType;
			}
			else if (leftType is CArrayType laType && rightBasicType != null) {
				return laType.ElementType.Pointer;
			}
			else if (rightType is CPointerType rpType && leftBasicType != null) {
				return rpType;
			}
			else if (rightType is CArrayType raType && leftBasicType != null) {
				return raType.ElementType.Pointer;
			}
			else {
				ec.Report.Error (19, "'" + op + "' cannot be applied to operands of type '" + leftType + "' and '" + rightType + "'");
				return CBasicType.SignedInt;
			}
		}

        public virtual Value EvalConstant (EmitContext ec)
        {
            ec.Report.Error (133, $"'{this}' not constant");
            return 0;
        }
    }
}
