using System;
using CLanguage.Syntax;
using CLanguage.Interpreter;
using CLanguage.Types;
using System.Linq;

namespace CLanguage.Compiler
{
    public abstract class EmitContext
    {
        public EmitContext? ParentContext { get; }
        public CompiledFunction? FunctionDecl { get; private set; }

        public Report Report { get; private set; }

        public MachineInfo MachineInfo { get; private set; }

        protected EmitContext (EmitContext parentContext)
            : this (parentContext.MachineInfo, parentContext.Report, parentContext.FunctionDecl, parentContext)
        { 
        }

        protected EmitContext (MachineInfo machineInfo, Report report, CompiledFunction? fdecl, EmitContext? parentContext)
        {
            MachineInfo = machineInfo ?? (parentContext?.MachineInfo ?? throw new ArgumentNullException (nameof (machineInfo)));
            Report = report ?? (parentContext?.Report ?? throw new ArgumentNullException (nameof (report)));
            FunctionDecl = fdecl ?? parentContext?.FunctionDecl;
            ParentContext = parentContext;
        }

        public virtual CType ResolveTypeName (TypeName typeName)
        {
            return MakeCType (typeName.Specifiers, typeName.Declarator, null, new Block (VariableScope.Global));
        }

        public virtual CType ResolveTypeName (string typeName)
        {
            var r = ParentContext?.ResolveTypeName (typeName);
            if (r != null)
                return r;

            Report.ErrorCode (103, typeName);
            return CBasicType.SignedInt;
        }

        public ResolvedVariable ResolveVariable (string name, CType[]? argTypes)
        {
            var v = TryResolveVariable (name, argTypes);
            if (v != null)
                return v;
            Report.ErrorCode (103, name);
            return new ResolvedVariable (VariableScope.Global, 0, CBasicType.SignedInt);
        }

        public virtual ResolvedVariable? TryResolveVariable (string name, CType[]? argTypes)
        {
            var r = ParentContext?.ResolveVariable (name, argTypes);
            if (r != null)
                return r;

            r = MachineInfo.GetUnresolvedVariable (name, argTypes, this);
            if (r != null)
                return r;

            return null;
        }

        public virtual ResolvedVariable ResolveMethodFunction (CStructType structType, CStructMethod method)
        {
            var r = ParentContext?.ResolveMethodFunction (structType, method);
            if (r != null)
                return r;

            throw new Exception ("Cannot resolve method function");
        }

        public virtual void BeginBlock (Block b) { }
        public virtual void EndBlock () { }

        public virtual Label DefineLabel ()
        {
            return new Label ();
        }

        public virtual void EmitLabel (Label l)
        {
        }

        public void EmitCast (CType fromType, CType toType)
        {
            if (fromType.Equals (toType)) {
                return;
            }
            var fromBasicType = fromType as CBasicType;
            var toBasicType = toType as CBasicType;

            if (fromBasicType != null && toBasicType != null) {
                var fromOffset = GetInstructionOffset (fromBasicType);
                var toOffset = GetInstructionOffset (toBasicType);
                var op = OpCode.ConvertInt8Int8 + (fromOffset * 10 + toOffset);
                Emit (op);
                // This conversion is implicit with how the evaluator stores its stuff
            }
            else if (fromBasicType != null && fromBasicType.IsIntegral && toType is CPointerType && fromBasicType.NumValues == toType.NumValues) {
                // Support `const char *p = 0;`
            }
            else if (fromType is CArrayType fat && toType is CPointerType tpt && fat.ElementType.NumValues == tpt.InnerType.NumValues) {
                // Demote arrays to pointers
            }
            else if (fromType is CEnumType et && toType is CIntType bt) {
                // Enums act like ints
            }
            else {
                Report.Error (30, "Cannot convert type '" + fromType + "' to '" + toType + "'");
            }
        }

        public void EmitCastToBoolean (CType fromType)
        {
            EmitCast (fromType, CBasicType.Bool);
        }

        public virtual void Emit (Instruction instruction)
        {
        }

        public void Emit (OpCode op, Value x)
        {
            Emit (new Instruction (op, x));
        }

        public void Emit (OpCode op, Label label)
        {
            Emit (new Instruction (op, label));
        }

        public void Emit (OpCode op)
        {
            Emit (op, 0);
        }

        public virtual Value GetConstantMemory (string stringConstant)
        {
            throw new NotSupportedException ("Cannot get constant memory from this context");
        }

        public int GetInstructionOffset (CBasicType aType)
        {
            var size = aType.GetByteSize (this);

            if (aType.IsIntegral) {
                switch (size) {
                    case 1:
                        return aType.Signedness == Signedness.Signed ? 0 : 1;
                    case 2:
                        return aType.Signedness == Signedness.Signed ? 2 : 3;
                    case 4:
                        return aType.Signedness == Signedness.Signed ? 4 : 5;
                    case 8:
                        return aType.Signedness == Signedness.Signed ? 6 : 7;
                }
            }
            else {
                switch (size) {
                    case 4:
                        return 8;
                    case 8:
                        return 9;
                }
            }

            throw new NotSupportedException ("Arithmetic on type '" + aType + "'");
        }

        public CType MakeCType (DeclarationSpecifiers specs, Declarator? decl, Initializer? init, Block block)
        {
            var type = MakeCType (specs, init, block);
            return MakeCType (type, decl, init, block);
        }

        CType MakeCType (CType type, Declarator? decl, Initializer? init, Block block)
        {
            if (decl is IdentifierDeclarator) {
                // This is the name
            }
            else if (decl is PointerDeclarator) {
                var pdecl = (PointerDeclarator)decl;
                var isPointerToFunc = false;

                if (pdecl.StrongBinding) {
                    type = MakeCType (type, pdecl.InnerDeclarator, null, block);
                    isPointerToFunc = type is CFunctionType;
                }

                Pointer? p = pdecl.Pointer;
                while (p != null) {
                    type = new CPointerType (type);
                    type.TypeQualifiers = p.TypeQualifiers;
                    p = p.NextPointer;
                }

                if (!pdecl.StrongBinding) {
                    type = MakeCType (type, pdecl.InnerDeclarator, null, block);
                }

                //
                // Remove 1 level of pointer indirection if this is
                // a pointer to a function since functions are themselves
                // pointers
                //
                if (isPointerToFunc) {
                    type = ((CPointerType)type).InnerType;
                }
            }
            else if (decl is ArrayDeclarator startAdecl) {
                var adecl = (ArrayDeclarator?)startAdecl;

                while (adecl != null) {
                    int? len = null;
                    if (adecl.LengthExpression is ConstantExpression clen) {
                        len = (int)clen.EvalConstant (this);
                    }
                    else {
                        if (init is StructuredInitializer sinit) {
                            len = sinit.Initializers.Count;
                        }
                        else {
                            len = 0;
                            Report.Error (2057, "Expected constant expression");
                        }
                    }
                    type = new CArrayType (type, len);
                    adecl = adecl?.InnerDeclarator as ArrayDeclarator;
                    if (adecl != null && adecl.InnerDeclarator != null) {
                        if (adecl.InnerDeclarator is IdentifierDeclarator) {
                        }
                        else if (!(adecl.InnerDeclarator is ArrayDeclarator)) {
                            type = MakeCType (type, adecl.InnerDeclarator, null, block);
                        }
                        else {
                            //throw new NotSupportedException("Unrecognized array syntax");
                        }
                    }
                }
            }
            else if (decl is FunctionDeclarator) {
                type = MakeCFunctionType (type, decl, block);
            }

            return type;
        }

        CType MakeCFunctionType (CType returnType, Declarator decl, Block block)
        {
            var fdecl = (FunctionDeclarator)decl;

            bool isInstance = decl.InnerDeclarator is IdentifierDeclarator ident && ident.Context.Count > 0;

            var name = decl.DeclaredIdentifier;
            var ftype = new CFunctionType (returnType, isInstance);
            foreach (var pdecl in fdecl.Parameters) {
                var pt = pdecl.DeclarationSpecifiers != null
                    ? MakeCType (pdecl.DeclarationSpecifiers, pdecl.Declarator, null, block)
                    : CBasicType.SignedInt;
                if (!pt.IsVoid) {
                    ftype.AddParameter (pdecl.Name, pt);
                }
            }

            var type = MakeCType (ftype, fdecl.InnerDeclarator, null, block);

            return type;
        }

        public CType MakeCType (DeclarationSpecifiers specs, Initializer? init, Block block)
        {
            //
            // Infer types
            //
            if (specs.StorageClassSpecifier == StorageClassSpecifier.Auto) {
                if (!(init is ExpressionInitializer einit)) {
                    this.Report.Error (818, "Implicitly-typed variabled must be initialized");
                    return CBasicType.SignedInt;
                }
                return einit.Expression.GetEvaluatedCType (this);
            }

            //
            // Try for Basic. The TypeSpecifiers are recorded in reverse from what is actually declared
            // in code.
            //
            var basicTs = specs.TypeSpecifiers.FirstOrDefault (x => x.Kind == TypeSpecifierKind.Builtin);
            if (basicTs != null) {
                if (basicTs.Name == "void") {
                    return CType.Void;
                }
                else {
                    var sign = Signedness.Signed;
                    var size = "";
                    TypeSpecifier? trueTs = null;

                    foreach (var ts in specs.TypeSpecifiers) {
                        if (ts.Name == "unsigned") {
                            sign = Signedness.Unsigned;
                        }
                        else if (ts.Name == "signed") {
                            sign = Signedness.Signed;
                        }
                        else if (ts.Name == "short" || ts.Name == "long") {
                            if (size.Length == 0) size = ts.Name;
                            else size = size + " " + ts.Name;
                        }
                        else {
                            trueTs = ts;
                        }
                    }

                    var typeName = trueTs == null ? "int" : trueTs.Name;
                    var type = typeName == "float" ? (CBasicType)CBasicType.Float
                        : (typeName == "double" ? (CBasicType)CBasicType.Double
                           : (typeName == "bool" ? (CBasicType)CBasicType.Bool
                              : new CIntType (typeName, sign, size)));
                    type.TypeQualifiers = specs.TypeQualifiers;
                    return type;
                }
            }

            //
            // Structs and Classes
            //
            var structTs = specs.TypeSpecifiers.FirstOrDefault (x => x.Kind == TypeSpecifierKind.Struct || x.Kind == TypeSpecifierKind.Class);
            if (structTs != null) {
                if (structTs.Body != null) {
                    var st = new CStructType (structTs.Name);
                    foreach (var s in structTs.Body.Statements) {
                        AddStructMember (st, s, block);
                    }
                    return st;
                }
                else {
                    // Lookup
                    var name = structTs.Name;
                    if (block.Structures.TryGetValue (name, out var structType)) {
                        return structType;
                    }
                    else {
                        Report.Error (246, "'{0}' not found", name);
                        return CBasicType.SignedInt;
                    }
                }
            }

            //
            // Enums
            //
            var enumTs = specs.TypeSpecifiers.FirstOrDefault (x => x.Kind == TypeSpecifierKind.Enum);
            if (enumTs != null) {
                var enumName = specs.TypeSpecifiers[0].Name;
                if (enumTs.Body != null) {
                    var et = new CEnumType (enumTs.Name);
                    var enumContext = new EnumContext (enumTs, et, this);
                    foreach (var s in enumTs.Body.Statements) {
                        AddEnumMember (et, s, block, enumContext);
                    }
                    return et;
                }
                else {
                    // Lookup
                    var name = enumTs.Name;
                    if (block.Enums.TryGetValue (name, out var et)) {
                        return et;
                    }
                    else {
                        Report.Error (246, "'{0}' not found", name);
                        return CBasicType.SignedInt;
                    }
                }
                //Report.Error (9000, "Enums not supported");
            }

            //
            // Typedefs
            //
            var typenameTs = specs.TypeSpecifiers.FirstOrDefault (x => x.Kind == TypeSpecifierKind.Typename);
            if (typenameTs != null) {
                var typedefName = typenameTs.Name;
                return ResolveTypeName (typedefName);
            }

            //
            // Rest
            //
            throw new NotImplementedException (GetType ().Name);
        }

        void AddStructMember (CStructType st, Statement s, Block block)
        {
            if (s is MultiDeclaratorStatement multi) {
                if (multi.InitDeclarators != null) {
                    foreach (var i in multi.InitDeclarators) {
                        var type = MakeCType (multi.Specifiers, i.Declarator, i.Initializer, block);
                        if (type is CFunctionType functionType) {
                            var name = i.Declarator.DeclaredIdentifier;
                            st.Members.Add (new CStructMethod { Name = name, MemberType = type });
                        }
                        else {
                            throw new NotSupportedException ($"Cannot add `{i}` to struct");
                        }
                    }
                }
            }
            else {
                throw new NotSupportedException ($"Cannot add statement `{s}` to struct");
            }
        }

        void AddEnumMember (CEnumType st, Statement s, Block block, EnumContext context)
        {
            if (s is EnumeratorStatement es) {
                var value = es.LiteralValue != null ? (int)es.LiteralValue.EvalConstant(context) : st.NextValue;
                st.Members.Add (new CEnumMember (es.Name, value));
            }
            else {
                throw new NotSupportedException ($"Cannot add statement `{s}` to enum");
            }
        }
    }
}
