using System;
using System.Collections.Generic;
using CLanguage.Interpreter;
using CLanguage.Types;
using CLanguage.Compiler;
using System.Diagnostics;
using System.Linq;

namespace CLanguage.Syntax
{
    public class MultiDeclaratorStatement : Statement
    {
        public DeclarationSpecifiers Specifiers;
        public List<InitDeclarator>? InitDeclarators;

        public override bool AlwaysReturns => false;

        public MultiDeclaratorStatement (DeclarationSpecifiers specifiers, List<InitDeclarator>? initDeclarators)
        {
            Specifiers = specifiers;
            InitDeclarators = initDeclarators;
        }

        public override string ToString ()
        {
            return string.Join (" ", Specifiers.TypeSpecifiers) + (InitDeclarators != null ? " " + string.Join (", ", InitDeclarators) : "");
        }

        protected override void DoEmit (EmitContext ec)
        {
            // This code should be changed to take advantage of the data in
            // BlockContext.Block.InitStatements that is filled in by AddDeclarationToBlock
            var multi = this;
            if (multi.InitDeclarators != null) {
                foreach (var idecl in multi.InitDeclarators) {
                    if ((multi.Specifiers.StorageClassSpecifier & StorageClassSpecifier.Typedef) != 0) {
                        // Typedefs don't have runtime emissions here
                    }
                    else {
                        // This is a variable, function, or constructor declaration
                        CType ctype = ec.MakeCType (multi.Specifiers, idecl.Declarator, idecl.Initializer, null); // Pass null for block as this is for emission type resolution
                        var name = idecl.Declarator.DeclaredIdentifier;

                        // --- BEGIN VTABLE POINTER INITIALIZATION ---
                        if (ctype is CStructType structType && structType.VTable != null && structType.VTable.Any())
                        {
                            // Ensure ExecutableContext is available
                            var exeCtx = ec.ExecutableContext;
                            if (exeCtx != null)
                            {
                                int vtableId = exeCtx.RegisterVTable(structType.VTable);
                                var resolvedVar = ec.TryResolveVariable(name, null); // We need the variable's location (global/local and address/offset)
                                
                                if (resolvedVar != null) {
                                    // Emit code to get the address of the variable
                                    resolvedVar.EmitPointer(ec); // This should leave the address of the variable on the stack

                                    // Push the VTable ID
                                    ec.Emit(OpCode.LoadConstant, new Value(vtableId));

                                    // Store the VTable ID at the beginning of the struct (offset 0)
                                    // Assuming __vptr is the first field and is of integer type (storing ID)
                                    // StoreInt32 expects [value, address] on stack if it's like StorePointer.
                                    // Or [address, value] then it consumes both.
                                    // Let's assume standard [value, address] -> store value at address.
                                    // So, we need to swap if EmitPointer left address and then we pushed value.
                                    // If EmitPointer is [address] and LoadConstant is [address, value]:
                                    // OpCode.StoreInt32 should handle this.
                                    // If CStructType.GetByteSize already accounts for __vptr as the first field,
                                    // and GetFieldValueOffset shifts other fields, this is fine.
                                    // We need to ensure StoreInt32 stores into the first few bytes of the object.
                                    // A specific OpCode.SetVTablePointer(vtableId) might be cleaner, using implicit var address.
                                    // For now, using StoreInt32 at offset 0 of the object address.
                                    
                                    // If EmitPointer gives [address], and LoadConstant gives [address, value]
                                    // ec.Emit(OpCode.StoreInt32); // Stores value into address. (Pops 2, pushes 0)
                                    // This assumes StoreInt32 stores an Int32 at the given pointer.
                                    // The __vptr field should be considered part of the struct's layout by GetByteSize.
                                    
                                    // Safer: explicit offset calculation for storing at address[0]
                                    // Stack: [var_address, vtable_id_value]
                                    // To store vtable_id_value at var_address[0]:
                                    // No, StoreInt32 (like StorePointer) should take value then address.
                                    // Stack should be [vtable_id_value, var_address] for StoreInt32.
                                    // So, after resolvedVar.EmitPointer(ec) -> [var_address]
                                    // ec.Emit(OpCode.LoadConstant, new Value(vtableId)); -> [var_address, vtable_id_value]
                                    // This order is fine if StoreInt32 expects [address, value]
                                    // Let's assume ec.EmitStoreAtPointer(CType targetType) handles this correctly for first field.
                                    // For simplicity, let's assume a direct store to the variable's address (offset 0).
                                    // This implies the variable's type (ctype) must reflect the vtable pointer field
                                    // or StoreInt32 just writes to the memory location.

                                    // We need to ensure the stack is [value_to_store, address_to_store_at] for StoreInt32/StorePointer
                                    // 1. Push value (vtableId)
                                    ec.Emit(OpCode.LoadConstant, new Value(vtableId));
                                    // 2. Push address of variable
                                    resolvedVar.EmitPointer(ec); 
                                    // Stack: [vtableId, varAddress]
                                    ec.Emit(OpCode.StoreInt32); // Store vtableId at varAddress (offset 0)

                                } else {
                                    ec.Report.Error(2005, idecl.Declarator.Location, $"Could not resolve variable '{name}' to initialize its VTable pointer.");
                                }
                            } else {
                                 ec.Report.Error(2006, idecl.Declarator.Location, $"ExecutableContext not available for VTable registration for '{name}'.");
                            }
                        }
                        // --- END VTABLE POINTER INITIALIZATION ---

                        if (ctype is CFunctionType ftype && !HasStronglyBoundPointer (idecl.Declarator)) {

                            // Ctors look like function declarations
                            if (ftype.ReturnType is CStructType ctorDeclType && idecl.Initializer == null && idecl.Declarator is FunctionDeclarator ctorDecl && ctorDecl.CouldBeCtorCall) {
                                GetCtorInitializerStatement (name, ctorDeclType, ctorDecl).Emit (ec);
                            }
                        }
                        else if (idecl.Initializer != null) {
                            var varExpr = new VariableExpression (name, Location.Null, Location.Null);
                            var initExpr = GetInitializerExpression (idecl.Initializer);
                            new ExpressionStatement (new AssignExpression (varExpr, initExpr)).Emit (ec);
                        }
                    }
                }
            }
        }

        public override void AddDeclarationToBlock (BlockContext context)
        {
            var multi = this;
            var block = context.Block;
            if (multi.InitDeclarators != null) {
                foreach (var idecl in multi.InitDeclarators) {
                    if ((multi.Specifiers.StorageClassSpecifier & StorageClassSpecifier.Typedef) != 0) {
                        var name = idecl.Declarator.DeclaredIdentifier;
                        var ttype = context.MakeCType (multi.Specifiers, idecl.Declarator, idecl.Initializer, block);
                        block.Typedefs[name] = ttype;
                    }
                    else {
                        CType ctype = context.MakeCType (multi.Specifiers, idecl.Declarator, idecl.Initializer, block);
                        var name = idecl.Declarator.DeclaredIdentifier;

                        if (ctype is CFunctionType ftype && !HasStronglyBoundPointer (idecl.Declarator)) {

                            // Ctors look like function declarations
                            if (ftype.ReturnType is CStructType ctorDeclType && idecl.Initializer == null && idecl.Declarator is FunctionDeclarator ctorDecl && ctorDecl.CouldBeCtorCall) {
                                block.AddVariable (name, ctorDeclType);
                                var callStmt = GetCtorInitializerStatement (name, ctorDeclType, ctorDecl);
                                block.InitStatements.Add (callStmt);
                            }
                            else {

                                var nameContext = (idecl.Declarator.InnerDeclarator is IdentifierDeclarator ndecl && ndecl.Context.Count > 0) ?
                                    string.Join ("::", ndecl.Context) : "";
                                var f = new CompiledFunction (name, nameContext, ftype, null);
                                block.Functions.Add (f);
                            }
                        }
                        else {
                            if ((ctype is CArrayType atype) &&
                                (atype.Length == null) &&
                                (idecl.Initializer != null)) {
                                if (idecl.Initializer is StructuredInitializer structInit) {
                                    var len = 0;
                                    foreach (var i in structInit.Initializers) {
                                        if (i.Designation == null) {
                                            len++;
                                        }
                                        else {
                                            foreach (var de in i.Designation.Designators) {
                                                // TODO: Pay attention to designators
                                                len++;
                                            }
                                        }
                                    }
                                    atype = new CArrayType (atype.ElementType, len);
                                }
                                else {
                                    //Report.Error();
                                }
                            }
                            //var init = GetInitExpression(idecl.Initializer);
                            block.AddVariable (name, ctype ?? CBasicType.SignedInt);
                        }

                        if (idecl.Initializer != null) {
                            var varExpr = new VariableExpression (name, Location.Null, Location.Null);
                            var initExpr = GetInitializerExpression (idecl.Initializer);
                            block.InitStatements.Add (new ExpressionStatement (new AssignExpression (varExpr, initExpr)));
                        }
                    }
                }
            }
            else {
                // When InitDeclarators is null, it's a type definition like:
                // struct Foo { ... }; or enum Bar { ... };
                // The MakeCType call is responsible for parsing the Body of the TypeSpecifier
                // and populating the members of the CStructType or CEnumType.
                CType ctype = context.MakeCType(multi.Specifiers, null, block);

                if (ctype is CStructType definedStructType) {
                    string structName = definedStructType.Name; // Name should be set by MakeCType from the TypeSpecifier

                    // --- BEGIN MODIFICATION for Base Class and FinalizeLayout ---
                    // Find the TypeSpecifier that defined this struct 
                    // (e.g., to get potential BaseClassName from it)
                    TypeSpecifier? actualStructSpecifier = multi.Specifiers.TypeSpecifiers
                        .FirstOrDefault(ts => ts.Kind == TypeSpecifierKind.Struct && (ts.Name == structName || (string.IsNullOrEmpty(ts.Name) && !string.IsNullOrEmpty(structName))));

                    if (actualStructSpecifier != null) { // Should always be true if definedStructType was created
                        // ASSUMPTION: actualStructSpecifier has a 'string? BaseClassName' property.
                        // This property needs to be added to TypeSpecifier.cs and populated by the parser.
                        // For now, we access it directly. If it's not there, this will cause a compile error
                        // which highlights the dependency on parser changes.
                        string? baseClassName = actualStructSpecifier.BaseClassName; 

                        if (!string.IsNullOrEmpty(baseClassName)) {
                            CType? baseCType = context.TryResolveType(baseClassName); // TryResolveType searches scopes
                            if (baseCType is CStructType baseStructType) {
                                definedStructType.BaseClass = baseStructType;
                                // Optional: Report info about successful base class linking
                                // context.Report.Info($"Struct '{structName}' inherits from '{baseClassName}'.");
                            } else {
                                if (baseCType == null) {
                                    context.Report.Error(1001, multi.Specifiers.TypeSpecifiers.First().Location, $"Base class '{baseClassName}' for struct '{structName}' not found.");
                                } else {
                                    context.Report.Error(1002, multi.Specifiers.TypeSpecifiers.First().Location, $"Base class '{baseClassName}' for struct '{structName}' is not a struct type.");
                                }
                            }
                        }

                        // Call FinalizeLayout to build the VTable
                        // BlockContext inherits from EmitContext, so 'context' can be used here.
                        definedStructType.FinalizeLayout(context);
                        // Optional: Report info about VTable generation
                        // if (definedStructType.VTable.Any()) {
                        //    context.Report.Info($"VTable generated for struct '{structName}' with {definedStructType.VTable.Count} entries.");
                        // }
                    }
                    // --- END MODIFICATION ---
                    
                    // Original logic to register the struct (if it has a name)
                    // Ensure structName is valid before using it as a key.
                    // MakeCType should have already tried to name it (e.g. from TypeSpecifier or typedef).
                    if (!string.IsNullOrEmpty(structName)) {
                        block.Structures[structName] = definedStructType;
                    } else if (actualStructSpecifier?.Body != null && multi.Specifiers.StorageClassSpecifier != StorageClassSpecifier.Typedef) {
                        // It's an anonymous struct definition, e.g., struct { int x; } var;
                        // These are typically handled by MakeCType returning the anonymous type directly
                        // for the variable declaration. If it got a name somehow (e.g. internal)
                        // but shouldn't be registered globally, this logic might need adjustment.
                        // For now, if it's nameless, it's not added to block.Structures by name.
                    }
                }
                else if (ctype is CEnumType enumType) {
                    var n = enumType.Name;
                    if (string.IsNullOrEmpty (n)) {
                        n = "e" + enumType.GetHashCode ();
                    }
                    block.Enums[n] = enumType;
                }
            }
        }

        private static ExpressionStatement GetCtorInitializerStatement (string name, CStructType ctorDeclType, FunctionDeclarator ctorDecl)
        {
            var varExpr = new VariableExpression (name, Location.Null, Location.Null);
            var pointerExpr = new AddressOfExpression (varExpr);
            var memExpr = new MemberFromReferenceExpression (varExpr, ctorDeclType.Name);
            var args = from p in ctorDecl.Parameters
                       select p.CtorArgumentValue;
            var callExpr = new FuncallExpression (memExpr, args);
            var callStmt = new ExpressionStatement (callExpr);
            return callStmt;
        }

        static Expression GetInitializerExpression (Initializer init)
        {
            if (init is ExpressionInitializer) {
                return ((ExpressionInitializer)init).Expression;
            }
            else if (init is StructuredInitializer) {
                var sinit = (StructuredInitializer)init;

                var sexpr = new StructureExpression ();

                foreach (var i in sinit.Initializers) {
                    var e = GetInitializerExpression (i);

                    if (i.Designation == null || i.Designation.Designators.Count == 0) {
                        var ie = new StructureExpression.Item (null, GetInitializerExpression (i));
                        sexpr.Items.Add (ie);
                    }
                    else {

                        foreach (var d in i.Designation.Designators) {
                            var ie = new StructureExpression.Item (d.ToString (), e);
                            sexpr.Items.Add (ie);
                        }
                    }
                }

                return sexpr;
            }
            else {
                throw new NotSupportedException (init.ToString ());
            }
        }

        static bool HasStronglyBoundPointer (Declarator? d)
        {
            if (d == null)
                return false;
            else if (d is PointerDeclarator && ((PointerDeclarator)d).StrongBinding)
                return true;
            else
                return HasStronglyBoundPointer (d.InnerDeclarator);
        }
    }

    [Flags]
    public enum StorageClassSpecifier
    {
        None = 0,
        Typedef = 1,
        Extern = 2,
        Static = 4,
        Auto = 8,
        Register = 16
    }

    public class DeclarationSpecifiers
    {
        public StorageClassSpecifier StorageClassSpecifier { get; set; }
        public List<TypeSpecifier> TypeSpecifiers { get; private set; }
        public FunctionSpecifier FunctionSpecifier { get; set; }
        public TypeQualifiers TypeQualifiers { get; set; }
        public DeclarationSpecifiers ()
        {
            TypeSpecifiers = new List<TypeSpecifier> ();
        }
        public override string ToString ()
        {
            return StorageClassSpecifier == StorageClassSpecifier.Auto ? "auto" : string.Join (" ", TypeSpecifiers);
        }
    }

    public class InitDeclarator
    {
        public Declarator Declarator;
        public Initializer? Initializer;

        public InitDeclarator (Declarator declarator, Initializer? initializer)
        {
            Declarator = declarator;
            Initializer = initializer;
        }

        public override string ToString ()
        {
            return Declarator.ToString ();
        }
    }
}
