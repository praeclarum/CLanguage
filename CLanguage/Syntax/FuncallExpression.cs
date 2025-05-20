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
				Arguments[i].Emit (ec);
                ec.EmitCast (argTypes[i], type.Parameters[i].ParameterType);
			}
            for (var i = Arguments.Count; i < type.Parameters.Count; i++) {
                var v = type.Parameters[i].DefaultValue ?? (Value)0;
                ec.Emit (OpCode.LoadConstant, v);
            }

            function.Emit (ec);

            ec.Emit (OpCode.Call, type.Parameters.Count);

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

                    var methods = new List<CStructMethod> ();
                    foreach (var mem in structType.Members) {
                        if (mem is CStructMethod meth && meth.Name == memr.MemberName)
                            methods.Add(meth);
                    }

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
                        else { // A non-error bestMatch was found
                            var matchedMethod = bestMatch.Method;
                            var methodType = matchedMethod.MemberType as CFunctionType;

                            if (methodType == null) { // Should not happen if it's a CStructMethod
                                ec.Report.Error(2000, memr.Location, $"Method '{matchedMethod.Name}' in struct '{structType.Name}' does not have a valid function type.");
                                return Overload.Error;
                            }

                            // --- BEGIN VIRTUAL CALL MODIFICATION ---
                            int vtableIndex = -1;
                            if (structType.VTable != null) {
                                for (int i = 0; i < structType.VTable.Count; i++) {
                                    var vfunc = structType.VTable[i];
                                    if (vfunc.Name == matchedMethod.Name && vfunc.FunctionType.Equals(methodType)) {
                                        vtableIndex = i;
                                        break;
                                    }
                                }
                            }

                            if (vtableIndex != -1) { // It's a virtual call!
                                return new Overload(
                                    methodType, // The signature of the method
                                    nec => {
                                        // 1. Emit 'this' pointer (object address)
                                        memr.Left.EmitPointer(nec); // obj_addr is now on stack

                                        // If CFunctionType.IsInstance is true, OpCode.Call will expect 'this'
                                        // The 'this' pointer is memr.Left.
                                        // The call sequence for instance methods usually involves:
                                        // PUSH this_ptr
                                        // PUSH arg1
                                        // ...
                                        // PUSH func_ptr_from_vtable
                                        // CALL num_args_declared_in_function_type

                                        // We need func_ptr_from_vtable on stack before CALL
                                        // And this_ptr must be on stack before arguments if IsInstance is true for the methodType.
                                        // Let's assume IsInstance methods will have 'this' pushed by the caller of 'function.Emit(ec)'
                                        // So, this Emit action should leave func_ptr on stack.
                                        // Arguments will be pushed by FuncallExpression.DoEmit after this Overload.Emit is called.
                                        
                                        // Stack before this Overload.Emit: [...]
                                        // Stack after memr.Left.EmitPointer(nec): [..., obj_addr]
                                        // This obj_addr will be the 'this' pointer.
                                        // Now, we need to get the function pointer from the vtable.

                                        // To get func_ptr from vtable:
                                        // 1. Get vtable address (__vptr is at offset 0 of obj_addr)
                                        //    We already have obj_addr on stack. Dup it to use for vptr lookup.
                                        nec.Emit(OpCode.Dup); // Stack: [..., obj_addr, obj_addr]
                                        nec.Emit(OpCode.LoadConstant, Value.Pointer(0)); // Offset for __vptr
                                        nec.Emit(OpCode.OffsetPointer); // Stack: [..., obj_addr, &__vptr] (effectively)
                                        nec.Emit(OpCode.LoadPointer);   // Stack: [..., obj_addr, vtable_address]
                                        
                                        // 2. Get method address from vtable
                                        // nec.MachineInfo.PointerSize should be used if VTable stores raw pointers.
                                        // Here, VTable is List<CompiledFunction>. The "pointer" might be an index or direct ref.
                                        // Let's assume for now the interpreter's CALL can handle a direct CompiledFunction object
                                        // or that VTable stores callable function pointers/IDs.
                                        // If VTable stores direct function pointers (Value.Pointer type):
                                        // nec.Emit(OpCode.LoadConstant, Value.Pointer(vtableIndex * nec.MachineInfo.PointerSize));
                                        // nec.Emit(OpCode.OffsetPointer); // Stack: [..., obj_addr, &(vtable[vtableIndex])]
                                        // nec.Emit(OpCode.LoadPointer);   // Stack: [..., obj_addr, function_pointer_from_vtable]
                                        // ---- NEW LOGIC based on VTable ID and Index ---
                                        // Stack is currently: [..., obj_addr (this_ptr), vtable_id_value_loaded_from_object]
                                        nec.Emit(OpCode.LoadConstant, Value.Int(vtableIndex)); // Push vtableIndex
                                        // Stack: [..., obj_addr (this_ptr), vtable_id_value, vtableIndex_value]
                                        nec.Emit(OpCode.ResolveVirtualFunction); // New OpCode: pops vtable_id, vtableIndex; looks up Executable.VTables[id][index]; pushes CompiledFunction Value
                                        // Stack: [..., obj_addr (this_ptr), actual_CompiledFunction_Value_from_vtable]
                                        // ---- END NEW LOGIC ---
                                        
                                        // Now stack has [this_ptr, func_ptr_from_vtable]
                                        // FuncallExpression.DoEmit will then push args and then call OpCode.Call.
                                        // OpCode.Call needs to be aware of instance calls to correctly handle 'this_ptr'.
                                        // It would typically expect [func_ptr, this_ptr, arg1, ..., argN] then SP-- for args & this, then SP-- for func_ptr.
                                        // Or [this_ptr, arg1, ..., argN, func_ptr]
                                        // The current FuncallExpression.DoEmit does:
                                        //   args.Emit()
                                        //   function.Emit(ec) // our overload, leaves func_ptr on stack
                                        //   OpCode.Call
                                        // This means stack before OpCode.Call is [arg1..argN, func_ptr]
                                        // For instance calls, we need to insert 'this_ptr' before func_ptr or ensure OpCode.Call can find it.

                                        // Let's adjust: this Overload.Emit should leave func_ptr on stack.
                                        // FuncallExpression.DoEmit needs to be aware if it's an instance call.
                                        // If methodType.IsInstance, then memr.Left.EmitPointer(nec) should be called by FuncallExpression.DoEmit
                                        // *before* it pushes arguments.

                                        // For now, this Overload.Emit will:
                                        // 1. memr.Left.EmitPointer(nec) ; to get obj_addr for vtable lookup
                                        // 2. ... lookup ... results in function_pointer_from_vtable on stack.
                                        // This means this_ptr is NOT explicitly on stack from this Overload.Emit alone for the OpCode.Call later.
                                        // This will be handled by the modified DoEmit.
                                    });
                            }
                            else { // Not a virtual call, use existing direct dispatch logic
                                var res = ec.ResolveMethodFunction (structType, matchedMethod);
                                if (res != null) {
                                    return new Overload (
                                        methodType,
                                        nec => {
                                            // For non-virtual instance methods, 'this' pointer is still needed.
                                            // memr.Left.EmitPointer(nec); // this_ptr
                                            // nec.Emit(OpCode.LoadConstant, Value.Pointer(res.Address)); // func_ptr
                                            // This structure is similar to the virtual one, just direct address.
                                            // FuncallExpression.DoEmit will handle pushing 'this' if it's an instance call.
                                            // So, this just needs to leave the function pointer on the stack.
                                            nec.Emit(OpCode.LoadConstant, Value.Pointer(res.Address));
                                        });                                
                                }
                                else {
                                    ec.Report.Error(2001, memr.Location, $"Could not resolve address for non-virtual method '{matchedMethod.Name}'.");
                                    return Overload.Error;
                                }
                            }
                            // --- END VIRTUAL CALL MODIFICATION ---
                        }
                    }
                }
                else if (targetType is CPointerType pointerTargetType && pointerTargetType.InnerType is CStructType pointedStructType) {
                    // Handling calls like ptr->Method() where ptr is CPointerType to CStructType
                    // This logic largely mirrors the CStructType case above.
                    var methods = new List<CStructMethod>();
                    foreach (var mem in pointedStructType.Members) {
                        if (mem is CStructMethod meth && meth.Name == memr.MemberName)
                            methods.Add(meth);
                    }

                    if (methods.Count == 0) {
                        ec.Report.Error(1061, memr.Location, $"'{memr.MemberName}' not found in struct '{pointedStructType.Name}'.");
                        return Overload.Error;
                    }

                    var scoredMethods = new List<ScoredMethod>();
                    foreach (var m in methods) {
                        if (m.MemberType is CFunctionType mt) {
                            var score = mt.ScoreParameterTypeMatches(argTypes);
                            if (score > 0) {
                                scoredMethods.Add(new ScoredMethod(m, score));
                            }
                        }
                    }
                    scoredMethods.Sort((a, b) => b.Score - a.Score);
                    var bestMatch = scoredMethods.Count > 0 ? scoredMethods[0] : null;

                    if (bestMatch == null) {
                        ec.Report.Error(1503, memr.Location, $"Argument type mismatch for method '{memr.MemberName}' in struct '{pointedStructType.Name}'.");
                        return Overload.Error;
                    }
                    
                    var matchedMethod = bestMatch.Method;
                    var methodType = matchedMethod.MemberType as CFunctionType;
                    if (methodType == null) {
                         ec.Report.Error(2000, memr.Location, $"Method '{matchedMethod.Name}' in struct '{pointedStructType.Name}' does not have a valid function type.");
                         return Overload.Error;
                    }

                    int vtableIndex = -1;
                    if (pointedStructType.VTable != null) {
                        for (int i = 0; i < pointedStructType.VTable.Count; i++) {
                            var vfunc = pointedStructType.VTable[i];
                            if (vfunc.Name == matchedMethod.Name && vfunc.FunctionType.Equals(methodType)) {
                                vtableIndex = i;
                                break;
                            }
                        }
                    }

                    if (vtableIndex != -1) { // Virtual call via pointer
                        return new Overload(
                            methodType,
                            nec => {
                                // 1. Emit pointer to object (which is memr.Left itself as it's already a pointer)
                                memr.Left.Emit(nec); // ptr_value is now on stack (this is obj_addr)
                                
                                // Vtable lookup (same as non-pointer case, but Left.Emit instead of Left.EmitPointer)
                                nec.Emit(OpCode.Dup); // obj_addr_val, obj_addr_val_copy (this is the pointer to struct)
                                nec.Emit(OpCode.LoadConstant, Value.Pointer(0)); 
                                nec.Emit(OpCode.OffsetPointer); 
                                nec.Emit(OpCode.LoadPointer);   // obj_addr_val, vtable_id_value
                                
                                // nec.Emit(OpCode.LoadConstant, Value.Pointer(vtableIndex * nec.MachineInfo.PointerSize));
                                // nec.Emit(OpCode.OffsetPointer); 
                                // nec.Emit(OpCode.LoadPointer);   
                                // ---- NEW LOGIC based on VTable ID and Index ---
                                nec.Emit(OpCode.LoadConstant, Value.Int(vtableIndex)); // Push vtableIndex
                                // Stack: [..., obj_addr_val (this_ptr), vtable_id_value, vtableIndex_value]
                                nec.Emit(OpCode.ResolveVirtualFunction); // New OpCode
                                // Stack: [..., obj_addr_val (this_ptr), actual_CompiledFunction_Value_from_vtable]
                                // ---- END NEW LOGIC ---
                            });
                    } else { // Non-virtual call via pointer
                        var res = ec.ResolveMethodFunction(pointedStructType, matchedMethod);
                        if (res != null) {
                            return new Overload(
                                methodType,
                                nec => {
                                    // memr.Left.Emit(nec); // For 'this' pointer, pushed by DoEmit if instance
                                    nec.Emit(OpCode.LoadConstant, Value.Pointer(res.Address)); // Direct function address
                                });
                        } else {
                            ec.Report.Error(2001, memr.Location, $"Could not resolve address for non-virtual method '{matchedMethod.Name}'.");
                            return Overload.Error;
                        }
                    }
                }
                else {
                    ec.Report.Error (119, memr.Location, $"'{memr.Left}' ({targetType?.ToString()}) is not a struct or pointer to struct, cannot call method '{memr.MemberName}'.");
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

            public static readonly Action<EmitContext> NoEmit = _ => { };
            public static readonly Overload Error = new Overload (CBasicType.SignedInt, NoEmit);

            public Overload (CType? type, Action<EmitContext> emit)
            {
                CType = type;
                Emit = emit ?? throw new ArgumentNullException (nameof (emit));
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
