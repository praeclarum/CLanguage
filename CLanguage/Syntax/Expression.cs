using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Compiler;
using CLanguage.Interpreter;
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

        #region Operator Overload Helpers

        protected static string? BinopToOperatorName (Binop op) => op switch {
            Binop.Add => "operator+",
            Binop.Subtract => "operator-",
            Binop.Multiply => "operator*",
            Binop.Divide => "operator/",
            Binop.Mod => "operator%",
            Binop.ShiftLeft => "operator<<",
            Binop.ShiftRight => "operator>>",
            Binop.BinaryAnd => "operator&",
            Binop.BinaryOr => "operator|",
            Binop.BinaryXor => "operator^",
            _ => null,
        };

        protected static string? RelOpToOperatorName (RelationalOp op) => op switch {
            RelationalOp.Equals => "operator==",
            RelationalOp.NotEquals => "operator!=",
            RelationalOp.LessThan => "operator<",
            RelationalOp.LessThanOrEqual => "operator<=",
            RelationalOp.GreaterThan => "operator>",
            RelationalOp.GreaterThanOrEqual => "operator>=",
            _ => null,
        };

        protected static string? UnopToOperatorName (Unop op) => op switch {
            Unop.Negate => "operator-",
            Unop.Not => "operator!",
            Unop.BinaryComplement => "operator~",
            _ => null,
        };

        protected static CStructMethod? FindBestOperatorMethod (CStructType structType, string operatorName, CType[] argTypes)
        {
            var methods = structType.FindMethods (operatorName);
            CStructMethod? best = null;
            var bestScore = 0;
            foreach (var m in methods) {
                if (m.MemberType is CFunctionType mt) {
                    var score = mt.ScoreParameterTypeMatches (argTypes);
                    if (score > bestScore) {
                        bestScore = score;
                        best = m;
                    }
                }
            }
            return best;
        }

        protected static CFunctionType? TryResolveBinaryOperatorType (EmitContext ec, CType leftType, CType rightType, string? operatorName)
        {
            if (operatorName == null)
                return null;
            if (leftType is CStructType st) {
                // Check struct members first (for operators declared in struct body)
                var m = FindBestOperatorMethod (st, operatorName, new[] { rightType });
                if (m?.MemberType is CFunctionType ft)
                    return ft;
                // Check externally defined member operators
                var resolved = ec.TryResolveOperatorFunction (st.Name, operatorName, new[] { rightType });
                if (resolved?.VariableType is CFunctionType rft)
                    return rft;
            }
            // Only check free-standing operators if at least one operand is a struct type
            if (leftType is CStructType || rightType is CStructType) {
                var res = ec.TryResolveVariable (operatorName, new[] { leftType, rightType });
                if (res?.VariableType is CFunctionType fft)
                    return fft;
            }
            return null;
        }

        protected static CFunctionType? TryResolveUnaryOperatorType (EmitContext ec, CType operandType, string? operatorName)
        {
            if (operatorName == null)
                return null;
            if (operandType is CStructType st) {
                var m = FindBestOperatorMethod (st, operatorName, Array.Empty<CType> ());
                if (m?.MemberType is CFunctionType ft)
                    return ft;
                var resolved = ec.TryResolveOperatorFunction (st.Name, operatorName, Array.Empty<CType> ());
                if (resolved?.VariableType is CFunctionType rft)
                    return rft;
                // Check free-standing unary operator
                var res = ec.TryResolveVariable (operatorName, new[] { operandType });
                if (res?.VariableType is CFunctionType fft)
                    return fft;
            }
            return null;
        }

        protected static bool TryEmitBinaryOperatorCall (EmitContext ec, CType leftType, CType rightType, Expression left, Expression right, string? operatorName)
        {
            if (operatorName == null)
                return false;

            var argTypes = new[] { rightType };
            var args = new[] { right };

            if (leftType is CStructType structType) {
                // Check struct members first (for operators declared in struct body)
                var method = FindBestOperatorMethod (structType, operatorName, argTypes);
                if (method != null) {
                    var funcType = (CFunctionType)method.MemberType;
                    var resolved = ec.ResolveMethodFunction (structType, method);
                    EmitMemberOperatorCall (ec, structType, resolved, funcType, left, args, argTypes);
                    return true;
                }
                // Check externally defined member operators via executable function list
                var resolvedOp = ec.TryResolveOperatorFunction (structType.Name, operatorName, argTypes);
                if (resolvedOp?.VariableType is CFunctionType rFuncType) {
                    EmitMemberOperatorCall (ec, structType, resolvedOp, rFuncType, left, args, argTypes);
                    return true;
                }
            }

            // Only check free-standing operators if at least one operand is a struct type
            if (leftType is CStructType || rightType is CStructType) {
                var freeArgTypes = new[] { leftType, rightType };
                var res = ec.TryResolveVariable (operatorName, freeArgTypes);
                if (res?.VariableType is CFunctionType) {
                    EmitFreeStandingOperatorCall (ec, res, new[] { left, right }, freeArgTypes);
                    return true;
                }
            }

            return false;
        }

        protected static bool TryEmitUnaryOperatorCall (EmitContext ec, CType operandType, Expression operand, string? operatorName)
        {
            if (operatorName == null)
                return false;

            if (operandType is CStructType structType) {
                var emptyTypes = Array.Empty<CType> ();
                var emptyArgs = Array.Empty<Expression> ();

                var method = FindBestOperatorMethod (structType, operatorName, emptyTypes);
                if (method != null) {
                    var funcType = (CFunctionType)method.MemberType;
                    var resolved = ec.ResolveMethodFunction (structType, method);
                    EmitMemberOperatorCall (ec, structType, resolved, funcType, operand, emptyArgs, emptyTypes);
                    return true;
                }
                var resolvedOp = ec.TryResolveOperatorFunction (structType.Name, operatorName, emptyTypes);
                if (resolvedOp?.VariableType is CFunctionType rFuncType) {
                    EmitMemberOperatorCall (ec, structType, resolvedOp, rFuncType, operand, emptyArgs, emptyTypes);
                    return true;
                }
                // Check free-standing unary operator
                var argTypes = new[] { operandType };
                var res = ec.TryResolveVariable (operatorName, argTypes);
                if (res?.VariableType is CFunctionType) {
                    EmitFreeStandingOperatorCall (ec, res, new[] { operand }, argTypes);
                    return true;
                }
            }

            return false;
        }

        static void EmitMemberOperatorCall (EmitContext ec, CStructType structType, ResolvedVariable resolved, CFunctionType funcType, Expression thisExpr, Expression[] args, CType[] argTypes)
        {
            var tempThisOffset = -1;

            // If thisExpr is an rvalue (e.g., intermediate result from chained operators),
            // evaluate it first and store to a temp so we can take its address.
            if (!thisExpr.CanEmitPointer) {
                tempThisOffset = ec.AllocateTemp (structType);
                thisExpr.Emit (ec);
                var numValues = structType.NumValues;
                for (var i = numValues - 1; i >= 0; i--)
                    ec.Emit (OpCode.StoreLocal, tempThisOffset + i);
            }

            // Push arguments
            for (var i = 0; i < args.Length; i++) {
                var paramType = funcType.Parameters[i].ParameterType;
                EmitOperatorArgument (ec, args[i], argTypes[i], paramType);
            }

            // Push this pointer
            if (tempThisOffset >= 0) {
                ec.Emit (OpCode.LoadConstant, Value.Pointer (tempThisOffset));
                ec.Emit (OpCode.LoadFramePointer);
                ec.Emit (OpCode.OffsetPointer);
            }
            else {
                thisExpr.EmitPointer (ec);
            }

            // Push function address and call
            ec.Emit (OpCode.LoadConstant, Value.Pointer (resolved.Address));
            ec.Emit (OpCode.Call, funcType.Parameters.Count);

            if (funcType.ReturnType.IsVoid)
                ec.Emit (OpCode.LoadConstant, 0);
        }

        static void EmitFreeStandingOperatorCall (EmitContext ec, ResolvedVariable resolvedFunc, Expression[] args, CType[] argTypes)
        {
            var funcType = (CFunctionType)resolvedFunc.VariableType;

            for (var i = 0; i < args.Length; i++) {
                var paramType = funcType.Parameters[i].ParameterType;
                EmitOperatorArgument (ec, args[i], argTypes[i], paramType);
            }

            resolvedFunc.Emit (ec);
            ec.Emit (OpCode.Call, funcType.Parameters.Count);

            if (funcType.ReturnType.IsVoid)
                ec.Emit (OpCode.LoadConstant, 0);
        }

        static void EmitOperatorArgument (EmitContext ec, Expression arg, CType argType, CType paramType)
        {
            if (paramType is CReferenceType refType) {
                if (arg.CanEmitPointer) {
                    arg.EmitPointer (ec);
                }
                else {
                    var innerType = refType.InnerType;
                    var tempOffset = ec.AllocateTemp (innerType);
                    arg.Emit (ec);
                    ec.EmitCast (argType, innerType);
                    var numValues = innerType.NumValues;
                    for (var i = numValues - 1; i >= 0; i--)
                        ec.Emit (OpCode.StoreLocal, tempOffset + i);
                    ec.Emit (OpCode.LoadConstant, Value.Pointer (tempOffset));
                    ec.Emit (OpCode.LoadFramePointer);
                    ec.Emit (OpCode.OffsetPointer);
                }
            }
            else {
                arg.Emit (ec);
                ec.EmitCast (argType, paramType);
            }
        }

        #endregion
    }
}
