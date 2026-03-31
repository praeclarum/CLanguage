using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Types;
using CLanguage.Interpreter;
using CLanguage.Compiler;

namespace CLanguage.Syntax
{
    public class AssignExpression : Expression
    {
        public Expression Left { get; private set; }
        public Expression Right { get; private set; }

        public AssignExpression(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return Left.GetEvaluatedCType (ec);
        }

        void DoEmitStructureAssignment (StructureExpression sexpr, EmitContext ec)
        {
            var type = GetEvaluatedCType (ec);

            if (type is CArrayType arrayType) {
                EmitArrayStructuredInit (sexpr, arrayType, 0, ec);
                Left.EmitPointer (ec);
            }
            else {
                throw new NotSupportedException ($"Structured assignment of '{GetEvaluatedCType (ec)}' not supported");
            }
        }

        void EmitArrayStructuredInit (StructureExpression sexpr, CArrayType arrayType, int baseOffset, EmitContext ec)
        {
            var elementType = arrayType.ElementType;
            var numItemValues = elementType.NumValues;

            var count = sexpr.Items.Count;
            for (int i = 0; i < count; i++) {
                var item = sexpr.Items[i];
                var itemOffset = baseOffset + i * numItemValues;

                if (item.Expression is StructureExpression subExpr && elementType is CArrayType subArrayType) {
                    EmitArrayStructuredInit (subExpr, subArrayType, itemOffset, ec);
                }
                else {
                    item.Expression.Emit (ec);
                    ec.EmitCast (item.Expression.GetEvaluatedCType (ec), elementType);
                    Left.EmitPointer (ec);
                    ec.Emit (OpCode.LoadConstant, itemOffset);
                    ec.Emit (OpCode.OffsetPointer);
                    ec.Emit (OpCode.StorePointer);
                }
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            if (Right is StructureExpression sexpr) {
                DoEmitStructureAssignment (sexpr, ec);
                return;
            }

            Right.Emit (ec);

            if (Left is VariableExpression variable) {

                var v = ec.ResolveVariable (variable, null);

                if (v.VariableType is CReferenceType refType) {
                    // Assigning to a reference: write through the pointer
                    ec.EmitCast (Right.GetEvaluatedCType (ec), refType.InnerType);
                    ec.Emit (OpCode.Dup);
                    // Load the pointer that the reference holds
                    VariableExpression.EmitLoadReferenceSlot (ec, v);
                    ec.Emit (OpCode.StorePointer);
                }
                else if (v.VariableType is CStructType structType) {
                    ec.EmitCast (Right.GetEvaluatedCType (ec), Left.GetEvaluatedCType (ec));
                    var numValues = structType.NumValues;
                    // Store N values in reverse order (top of stack = last field)
                    for (int i = numValues - 1; i >= 0; i--) {
                        if (v.Scope == VariableScope.Global) {
                            ec.Emit (OpCode.StoreGlobal, v.Address + i);
                        }
                        else if (v.Scope == VariableScope.Local) {
                            ec.Emit (OpCode.StoreLocal, v.Address + i);
                        }
                        else if (v.Scope == VariableScope.Arg) {
                            ec.Emit (OpCode.StoreArg, v.Address + i);
                        }
                        else {
                            throw new NotSupportedException ("Assigning struct to scope '" + v.Scope + "'");
                        }
                    }
                    // Reload all values as the expression result
                    for (int i = 0; i < numValues; i++) {
                        if (v.Scope == VariableScope.Global) {
                            ec.Emit (OpCode.LoadGlobal, v.Address + i);
                        }
                        else if (v.Scope == VariableScope.Local) {
                            ec.Emit (OpCode.LoadLocal, v.Address + i);
                        }
                        else if (v.Scope == VariableScope.Arg) {
                            ec.Emit (OpCode.LoadArg, v.Address + i);
                        }
                        else {
                            throw new NotSupportedException ("Loading struct from scope '" + v.Scope + "'");
                        }
                    }
                }
                else {
                    ec.EmitCast (Right.GetEvaluatedCType (ec), Left.GetEvaluatedCType (ec));
                    ec.Emit (OpCode.Dup);

                    if (v.Scope == VariableScope.Global) {
                        ec.Emit (OpCode.StoreGlobal, v.Address);
                    }
                    else if (v.Scope == VariableScope.Local) {
                        ec.Emit (OpCode.StoreLocal, v.Address);
                    }
                    else if (v.Scope == VariableScope.Arg) {
                        ec.Emit (OpCode.StoreArg, v.Address);
                    }
                    else if (v.Scope == VariableScope.Function) {
                        ec.Emit (OpCode.Pop);
                        ec.Report.Error (1656, $"Cannot assign to `{variable.VariableName}` because it is a function");
                    }
                    else {
                        throw new NotSupportedException ("Assigning to scope '" + v.Scope + "'");
                    }
                }
            }
            else if (Left.CanEmitPointer) {
                ec.EmitCast (Right.GetEvaluatedCType (ec), Left.GetEvaluatedCType (ec));
                ec.Emit (OpCode.Dup);
                Left.EmitPointer (ec);
                ec.Emit (OpCode.StorePointer);
            }
            else {
                ec.Report.Error (131, "The left-hand side of an assignment must be a variable or an addressable memory location");
            }
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Left, Right);
        }
    }
}
