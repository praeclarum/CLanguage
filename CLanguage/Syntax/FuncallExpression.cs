using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLanguage.Types;

using CLanguage.Compiler;
using CLanguage.Interpreter;
using System.Diagnostics;

namespace CLanguage.Syntax
{
    public class FuncallExpression : Expression
    {
        public Expression Function { get; }
        public List<Expression> Arguments { get; }

        public FuncallExpression(Expression fun)
        {
            Function = fun;
            Arguments = new List<Expression>();
        }

        public FuncallExpression(Expression fun, IEnumerable<Expression> args)
        {
            Function = fun;
            Arguments = new List<Expression>(args);
        }

		public override CType GetEvaluatedCType (EmitContext ec)
		{
            var argTypes = new CType[Arguments.Count];
            for (var i = 0; i < Arguments.Count; i++) {
                argTypes[i] = Arguments[i].GetEvaluatedCType (ec);
            }
            var function = ResolveOverload (Function, argTypes, ec);
            var ft = function.CType as CFunctionType;
            if (ft != null)
            {
                return ft.ReturnType;
            }
            else
            {
                return CBasicType.SignedInt;
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            var argTypes = new CType[Arguments.Count];
            for (var i = 0; i < Arguments.Count; i++) {
                argTypes[i] = Arguments[i].GetEvaluatedCType (ec);
            }
            var function = ResolveOverload (Function, argTypes, ec);

            var type = function.CType as CFunctionType;

            var numRequiredParameters = 0;
            if (type != null)
            {
                foreach (var p in type.Parameters) {
                    if (p.DefaultValue.HasValue)
                        break;
                    numRequiredParameters++;
                }
                if (Arguments.Count < numRequiredParameters) {
                    ec.Report.Error (1501, "'{0}' takes {1} arguments, {2} provided", Function, numRequiredParameters, Arguments.Count);
					return;
				}
            }
            else
            {
                //if (!Function.HasError)
                {
                    ec.Report.Error(2064, "'{0}' does not evaluate to a function taking {1} arguments", Function, Arguments.Count);
                }
				return;
            }
            for (var i = 0; i < Arguments.Count; i++)
			{
                var paramType = type.Parameters[i].ParameterType;
                if (paramType is CReferenceType refType) {
                    // Pass by reference: push address of the argument
                    if (Arguments[i].CanEmitPointer) {
                        Arguments[i].EmitPointer (ec);
                    }
                    else {
                        // For rvalues/temporaries: emit value to a temp local, then take its address
                        var tempOffset = ec.AllocateTemp (refType.InnerType);
                        Arguments[i].Emit (ec);
                        ec.EmitCast (argTypes[i], refType.InnerType);
                        // Store the value in the temp local
                        ec.Emit (OpCode.StoreLocal, tempOffset);
                        // Push the address of the temp local
                        ec.Emit (OpCode.LoadConstant, Value.Pointer (tempOffset));
                        ec.Emit (OpCode.LoadFramePointer);
                        ec.Emit (OpCode.OffsetPointer);
                    }
                }
                else {
                    Arguments[i].Emit (ec);
                    ec.EmitCast (argTypes[i], paramType);
                }
			}
            for (var i = Arguments.Count; i < type.Parameters.Count; i++) {
                var v = type.Parameters[i].DefaultValue ?? (Value)0;
                ec.Emit (OpCode.LoadConstant, v);
            }

            function.Emit (ec);

            if (function.VTableSlotIndex.HasValue) {
                ec.Emit (OpCode.CallVirtual, function.VTableSlotIndex.Value);
            }
            else {
                ec.Emit (OpCode.Call, type.Parameters.Count);
            }

			if (type.ReturnType.IsVoid) {
				ec.Emit (OpCode.LoadConstant, 0); // Expressions should leave something on the stack
			}
        }

        class ScoredMethod {
            public CStructMethod Method;
            public int Score;

            public ScoredMethod (CStructMethod method, int score)
            {
                Method = method;
                Score = score;
            }
        }

        Overload ResolveOverload (Expression function, CType[] argTypes, EmitContext ec)
        {
            if (function is MemberFromReferenceExpression memr) {
                var targetType = memr.Left.GetEvaluatedCType (ec);

                if (targetType is CStructType structType) {

                    var methods = structType.FindMethods (memr.MemberName);

                    if (methods.Count == 0) {
                        ec.Report.Error (1061, "'{1}' not found in '{0}'", structType.Name, memr.MemberName);
                        return Overload.Error;
                    }
                    else {
                        var scoredMethods = new List<ScoredMethod> ();
                        foreach (var m in methods) {
                            if (m.MemberType is CFunctionType mt) {
                                var score = mt.ScoreParameterTypeMatches (argTypes);
                                if (score > 0) {
                                    scoredMethods.Add (new ScoredMethod (m, score));
                                }
                            }
                        }
                        scoredMethods.Sort((a, b) => b.Score - a.Score);
                        var bestMatch = scoredMethods.Count > 0 ? scoredMethods[0] : null;

                        if (bestMatch == null) {
                            var fmethod = methods.FirstOrDefault ();
                            ec.Report.Error (1503, $"'{function}' argument type mismatch");
                            return Overload.Error;
                        }
                        else {
                            var method = bestMatch.Method;
                            if (method.VTableSlotIndex.HasValue && structType.VTableGlobalAddress.HasValue) {
                                // Virtual dispatch path
                                var functionType = method.MemberType as CFunctionType;
                                return new Overload (
                                    functionType,
                                    nec => {
                                        memr.Left.EmitPointer (nec);              // push &obj (this)
                                    },
                                    method.VTableSlotIndex.Value);
                            }
                            else {
                                // Non-virtual direct call path
                                var res = ec.ResolveMethodFunction (structType, method);
                                if (res != null) {
                                    return new Overload (
                                        res.Function?.FunctionType,
                                        nec => {
                                            memr.Left.EmitPointer (nec);
                                            nec.Emit (OpCode.LoadConstant, Value.Pointer (res.Address));
                                        });                                
                                }
                                else {
                                    return Overload.Error;
                                }
                            }
                        }
                    }
                }
                else {
                    ec.Report.Error (119, $"'{memr.Left}' is not valid in the given context");
                    return Overload.Error;
                }
            }
            else if (function is MemberFromPointerExpression memp) {
                var targetType = memp.Left.GetEvaluatedCType (ec);

                if (targetType is CPointerType pType && pType.InnerType is CStructType structType) {

                    var methods = structType.FindMethods (memp.MemberName);

                    if (methods.Count == 0) {
                        ec.Report.Error (1061, "'{1}' not found in '{0}'", structType.Name, memp.MemberName);
                        return Overload.Error;
                    }
                    else {
                        var scoredMethods = new List<ScoredMethod> ();
                        foreach (var m in methods) {
                            if (m.MemberType is CFunctionType mt) {
                                var score = mt.ScoreParameterTypeMatches (argTypes);
                                if (score > 0) {
                                    scoredMethods.Add (new ScoredMethod (m, score));
                                }
                            }
                        }
                        scoredMethods.Sort ((a, b) => b.Score - a.Score);
                        var bestMatch = scoredMethods.Count > 0 ? scoredMethods[0] : null;

                        if (bestMatch == null) {
                            ec.Report.Error (1503, $"'{function}' argument type mismatch");
                            return Overload.Error;
                        }
                        else {
                            var method = bestMatch.Method;
                            if (method.VTableSlotIndex.HasValue && structType.VTableGlobalAddress.HasValue) {
                                // Virtual dispatch path (via pointer)
                                var functionType = method.MemberType as CFunctionType;
                                return new Overload (
                                    functionType,
                                    nec => {
                                        memp.Left.Emit (nec);                      // push ptr (this)
                                    },
                                    method.VTableSlotIndex.Value);
                            }
                            else {
                                // Non-virtual direct call path (via pointer)
                                var res = ec.ResolveMethodFunction (structType, method);
                                if (res != null) {
                                    return new Overload (
                                        res.Function?.FunctionType,
                                        nec => {
                                            memp.Left.Emit (nec);
                                            nec.Emit (OpCode.LoadConstant, Value.Pointer (res.Address));
                                        });
                                }
                                else {
                                    return Overload.Error;
                                }
                            }
                        }
                    }
                }
                else {
                    ec.Report.Error (119, $"'{memp.Left}' is not valid for -> operator");
                    return Overload.Error;
                }
            }
            else if (function is ScopeResolutionExpression scopeExpr) {
                var res = ec.TryResolveQualifiedFunction (scopeExpr.TypeName, scopeExpr.MemberName, argTypes);
                if (res != null) {
                    return new Overload (
                        res.VariableType,
                        res.VariableType is CFunctionType ? (Action<EmitContext>)res.Emit : res.EmitPointer);
                }
                else {
                    ec.Report.Error (103, $"'{scopeExpr}' not found");
                    return Overload.Error;
                }
            }
            else if (function is VariableExpression v) {
                var res = ec.ResolveVariable (v, argTypes);
                if (res != null) {
                    return new Overload (
                        res.VariableType,
                        res.VariableType is CFunctionType ? (Action<EmitContext>)res.Emit : res.EmitPointer);
                }
                else {
                    return Overload.Error;
                }
            }
            else {
                return new Overload (
                    function?.GetEvaluatedCType (ec),
                    function != null ? function.Emit : Overload.NoEmit);
            }
        }

        class Overload
        {
            public readonly CType? CType;
            public readonly Action<EmitContext> Emit;
            public readonly int? VTableSlotIndex;

            public static readonly Action<EmitContext> NoEmit = _ => { };
            public static readonly Overload Error = new Overload (CBasicType.SignedInt, NoEmit);

            public Overload (CType? type, Action<EmitContext> emit, int? vTableSlotIndex = null)
            {
                CType = type;
                Emit = emit ?? throw new ArgumentNullException (nameof (emit));
                VTableSlotIndex = vTableSlotIndex;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Function.ToString());
            sb.Append("(");
            var head = "";
            foreach (var a in Arguments)
            {
                sb.Append(head);
                sb.Append(a.ToString());
                head = ", ";
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
