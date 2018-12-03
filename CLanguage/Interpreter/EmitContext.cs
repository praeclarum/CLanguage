using System;
using CLanguage.Syntax;
using CLanguage.Interpreter;
using CLanguage.Types;
using System.Linq;

namespace CLanguage.Interpreter
{
    public class EmitContext
    {
        public CompiledFunction FunctionDecl { get; private set; }

        public Report Report { get; private set; }

        public MachineInfo MachineInfo { get; private set; }

        public EmitContext (MachineInfo machineInfo, Report report, CompiledFunction fdecl = null)
        {
            if (machineInfo == null) throw new ArgumentNullException (nameof (machineInfo));
            if (report == null) throw new ArgumentNullException (nameof (report));

            MachineInfo = machineInfo;
            Report = report;
            FunctionDecl = fdecl;
        }

        public virtual CType ResolveTypeName (TypeName typeName)
        {
            return MakeCType (typeName.Specifiers, typeName.Declarator, null, null);
        }

        public virtual ResolvedVariable ResolveVariable (string name, CType[] argTypes)
        {
            return null;
        }

        public virtual ResolvedVariable ResolveMethodFunction (CStructType structType, CStructMethod method)
        {
            return null;
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

        public CType MakeCType (DeclarationSpecifiers specs, Declarator decl, Initializer init, Block block)
        {
            var type = MakeCType (specs, init, block);
            return MakeCType (type, decl, init, block);
        }

        CType MakeCType (CType type, Declarator decl, Initializer init, Block block)
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

                var p = pdecl.Pointer;
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
            else if (decl is ArrayDeclarator) {
                var adecl = (ArrayDeclarator)decl;

                while (adecl != null) {
                    int? len = null;
                    if (adecl.LengthExpression is ConstantExpression clen) {
                        len = Convert.ToInt32 (clen.Value);
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
                    adecl = adecl.InnerDeclarator as ArrayDeclarator;
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
                var pt = MakeCType (pdecl.DeclarationSpecifiers, pdecl.Declarator, null, block);
                if (!pt.IsVoid) {
                    ftype.AddParameter (pdecl.Name, pt);
                }
            }

            var type = MakeCType (ftype, fdecl.InnerDeclarator, null, block);

            return type;
        }

        public CType MakeCType (DeclarationSpecifiers specs, Initializer init, Block block)
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
                    TypeSpecifier trueTs = null;

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
                    var st = new CStructType ();
                    st.Name = structTs.Name;
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
                        Report.Error (246, "The struct '{0}' could not be found", name);
                        return CBasicType.SignedInt;
                    }
                }
            }

            //
            // Typedefs
            //
            if (specs.TypeSpecifiers.Count == 1 && specs.TypeSpecifiers[0].Kind == TypeSpecifierKind.Typename) {
                var name = specs.TypeSpecifiers[0].Name;

                var t = block.LookupTypedef (name);

                if (t != null)
                    return t;

                Report.Error (103, "The name '{0}' does not exist in the current context", name);
                return CBasicType.SignedInt;
            }

            //
            // Rest
            //
            throw new NotImplementedException ();
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
    }
}
