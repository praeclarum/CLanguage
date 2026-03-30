using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CLanguage.Syntax;
using CLanguage.Types;
using CLanguage.Parser;
using CLanguage.Interpreter;
using System.Diagnostics;

namespace CLanguage.Compiler
{
    public class CCompiler
    {
        CompilerOptions options;

        readonly Dictionary<string, LexedDocument> lexedDocuments = new Dictionary<string, LexedDocument> ();

		List<TranslationUnit> tus;

        public CompilerOptions Options => options;

        public CCompiler ()
            : this (new CompilerOptions ())
        {
        }

        public CCompiler (CompilerOptions options)
		{
            this.options = options;
			tus = new List<TranslationUnit> ();

            ProcessDocument (new Document ("_machine.h", options.MachineInfo.GeneratedHeaderCode));
            foreach (var kv in options.MachineInfo.SystemHeadersCode) {
                ProcessDocument (new Document (kv.Key, kv.Value));
            }
            foreach (var d in options.Documents) {
                ProcessDocument (d);
            }
        }

		public CCompiler (MachineInfo mi, Report report)
			: this (new CompilerOptions (mi, report, noDocs))
        {
        }

        static readonly Document[] noDocs = new Document[0];

        public void Add (TranslationUnit translationUnit)
        {
            tus.Add (translationUnit);
        }

        void ProcessDocument (Document document)
        {
            var lexed = new LexedDocument (document, options.Report);
            lexedDocuments[document.Path] = lexed;

            if (document.IsCompilable) {
                var parser = new CParser ();

                var name = System.IO.Path.GetFileNameWithoutExtension (document.Path);

                Add (parser.ParseTranslationUnit (options.Report, name, Include, lexedDocuments["_machine.h"].Tokens, lexed.Tokens));
            }
        }

        Token[]? Include (string path, bool relative)
        {
            if (lexedDocuments.TryGetValue(path, out var doc)) {
                return doc.Tokens;
            }
            return null;
        }

        public void AddCode (string name, string code)
        {
            AddDocument (new Document (name, code));
        }

        public void AddDocument (Document document)
        {
            ProcessDocument (document);
        }

        public Executable Compile ()
        {
            try {
                return CompileExecutable ();
            }
            catch (Exception ex) {
                Debug.WriteLine (ex);
                options.Report.Error (9000, "Compiler error: " + ex.Message);
                return new Executable (options.MachineInfo);
            }
        }

        public static Executable Compile (string code)
        {
            var compiler = new Compiler.CCompiler ();
            compiler.AddCode ("main.c", code);
            var exe = compiler.Compile ();
            if (compiler.Options.Report.Errors.Any (x => x.IsError)) {
                var m = string.Join ("\n", compiler.Options.Report.Errors.Where (x => x.IsError));
                throw new ArgumentException (m, nameof (code));
            }
            return exe;
        }

        Executable CompileExecutable ()
        {
            var exe = new Executable (options.MachineInfo);
            var exeContext = new ExecutableContext (exe, options.Report);

            // Put something at the zero address so we don't get 0 addresses of globals
            exe.AddGlobal ("__zero__", CBasicType.SignedInt);

            //
            // Find Variables, Functions, Types
            //
            var exeInitBody = new Block (VariableScope.Local);
            var tucs = tus.Select (x => new TranslationUnitContext (x, exeContext));
            var tuInits = new List<FunctionToCompile> ();
            foreach (var tuc in tucs) {
                var tu = tuc.TranslationUnit;
                AddStatementDeclarations (tuc);
                if (tu.InitStatements.Count > 0) {
                    var tuInitBody = new Block (VariableScope.Local);
                    tuInitBody.AddStatements (tu.InitStatements);
                    var tuInit = new CompiledFunction ($"__{tu.Name}__cinit", "", CFunctionType.VoidProcedure, tuInitBody);
                    exeInitBody.AddStatement (new ExpressionStatement (new FuncallExpression (new VariableExpression (tuInit.Name, Location.Null, Location.Null))));
                    tuInits.Add (new FunctionToCompile (tuInit, tuc));
                    exe.Functions.Add (tuInit);
                }
            }

            //
            // Generate a function to init globals
            //
            var exeInit = new CompiledFunction ($"__cinit", "", CFunctionType.VoidProcedure, exeInitBody);
            exe.Functions.Add (exeInit);

            //
            // Allocate vtable globals for polymorphic types
            // This must happen before globals are added so vptr InitialValues can reference vtable addresses
            //
            var polymorphicTypes = new List<CStructType> ();
            var vtableVars = new Dictionary<CStructType, CompiledVariable> ();
            foreach (var tuc in tucs) {
                CollectPolymorphicTypes (tuc.TranslationUnit, polymorphicTypes);
            }
            BaseFunction? pureVirtualTrap = null;
            if (polymorphicTypes.Count > 0) {
                // Add the pure-virtual trap function (only when needed)
                pureVirtualTrap = new InternalFunction (
                    "__pure_virtual_called", "", CFunctionType.VoidProcedure) {
                    Action = (CInterpreter state) => throw new ExecutionException ("Pure virtual function called")
                };
                exe.Functions.Add (pureVirtualTrap);
            }
            var nextTypeId = 1;
            foreach (var st in polymorphicTypes) {
                // Assign unique type ID for RTTI
                st.VTable!.TypeId = nextTypeId++;
                // Vtable runtime layout: [type_id, method0, method1, ...]
                var vtableSize = st.VTable.RuntimeSlotCount;
                var vtableType = new CArrayType (CBasicType.SignedInt, vtableSize);
                var vtableVar = exe.AddGlobal ($"__vtable_{st.Name}", vtableType);
                st.VTableGlobalAddress = vtableVar.StackOffset;
                vtableVars[st] = vtableVar;
            }

            //
            // Link everything together
            // This is done before compilation to make sure everything is visible (for recursion)
            //
            var functionsToCompile = new List<FunctionToCompile> { new FunctionToCompile (exeInit, exeContext) };
            functionsToCompile.AddRange (tuInits);
            foreach (var tuc in tucs) {
                var tu = tuc.TranslationUnit;
                foreach (var g in tu.Variables) {
                    var v = exe.AddGlobal (g.Name, g.VariableType);
                    v.InitialValue = g.InitialValue;
                    // Set vptr for polymorphic global variables
                    if (g.VariableType is CStructType gst && gst.IsPolymorphic && gst.VTableGlobalAddress.HasValue) {
                        var numValues = gst.NumValues;
                        if (v.InitialValue == null || v.InitialValue.Length < numValues) {
                            v.InitialValue = new Value[numValues];
                        }
                        v.InitialValue[0] = Value.Pointer (gst.VTableGlobalAddress.Value);
                    }
                }
                var funcs = tu.Functions.Where (x => x.Body != null).ToList ();
                exe.Functions.AddRange (funcs);
                functionsToCompile.AddRange (funcs.Select (x => new FunctionToCompile (x, (EmitContext)tuc)));
            }

            //
            // Populate vtable initial values with function pointers
            // Now that all functions have their indices, we can resolve them
            //
            var funcIndex = BuildFunctionIndex (exe);
            foreach (var st in polymorphicTypes) {
                if (vtableVars.TryGetValue (st, out var vtableVar)) {
                    PopulateVTable (exe, st, vtableVar, funcIndex, pureVirtualTrap!);
                }
            }

            //
            // Build compile-time type hierarchy table for RTTI
            //
            foreach (var st in polymorphicTypes) {
                var baseTypeId = -1;
                if (st.BaseType?.VTable != null) {
                    baseTypeId = st.BaseType.VTable.TypeId;
                }
                exe.AddTypeHierarchyEntry (new TypeHierarchyEntry (st.VTable!.TypeId, baseTypeId, st.Name));
            }

            //
            // Compile functions
            //
            foreach (var fAndPC in functionsToCompile) {
                var f = fAndPC.Function;
                var pc = fAndPC.Context;
                var body = f.Body;
                if (body == null)
                    continue;
                var fc = new FunctionContext (exe, f, pc);
                AddStatementDeclarations (fc);
				body.Emit (fc);
				f.LocalVariables.AddRange (fc.LocalVariables);

				// Make sure it returns
				if (body.Statements.Count == 0 || !body.AlwaysReturns) {
					if (f.FunctionType.ReturnType.IsVoid) {
						fc.Emit (OpCode.Return);
					}
					else {
						options.Report.Error (161, "'" + f.Name + "' not all code paths return a value");
					}
				}
			}

			return exe;
		}

        void CollectPolymorphicTypes (Block block, List<CStructType> result)
        {
            foreach (var kv in block.Structures) {
                if (kv.Value.IsPolymorphic && kv.Value.VTable != null && !result.Contains (kv.Value)) {
                    result.Add (kv.Value);
                }
            }
        }

        void PopulateVTable (Executable exe, CStructType st, CompiledVariable vtableVar, Dictionary<(string, string), List<(int Index, BaseFunction Func)>> funcIndex, BaseFunction pureVirtualTrap)
        {
            if (st.VTable == null)
                return;

            // Runtime layout: [type_id, method0, method1, ...]
            var initialValues = new Value[st.VTable.RuntimeSlotCount];
            initialValues[0] = st.VTable.TypeId;
            var trapIndex = exe.Functions.IndexOf (pureVirtualTrap);
            for (var i = 0; i < st.VTable.Count; i++) {
                var entry = st.VTable[i];
                var idx = FindFunctionInIndex (funcIndex, entry.DeclaringType.Name, entry.MethodName, entry.Signature);
                if (idx >= 0) {
                    initialValues[i + 1] = Value.Pointer (idx);
                }
                else {
                    // Use pure virtual trap for unresolved methods
                    initialValues[i + 1] = Value.Pointer (trapIndex >= 0 ? trapIndex : 0);
                }
            }
            vtableVar.InitialValue = initialValues;
        }

        static Dictionary<(string, string), List<(int Index, BaseFunction Func)>> BuildFunctionIndex (Executable exe)
        {
            var index = new Dictionary<(string, string), List<(int, BaseFunction)>> ();
            for (var i = 0; i < exe.Functions.Count; i++) {
                var f = exe.Functions[i];
                var key = (f.NameContext, f.Name);
                if (!index.TryGetValue (key, out var list)) {
                    list = new List<(int, BaseFunction)> ();
                    index[key] = list;
                }
                list.Add ((i, f));
            }
            return index;
        }

        static int FindFunctionInIndex (Dictionary<(string, string), List<(int Index, BaseFunction Func)>> funcIndex, string nameContext, string methodName, CFunctionType signature)
        {
            if (funcIndex.TryGetValue ((nameContext, methodName), out var candidates)) {
                foreach (var (idx, f) in candidates) {
                    if (f.FunctionType.ParameterTypesEqual (signature))
                        return idx;
                }
            }
            return -1;
        }

        class FunctionToCompile
        {
            public readonly CompiledFunction Function;
            public readonly EmitContext Context;

            public FunctionToCompile (CompiledFunction function, EmitContext context)
            {
                Function = function;
                Context = context;
            }
        }

        void AddStatementDeclarations (BlockContext context)
        {
            var block = context.Block;
            foreach (var s in block.Statements) {
                s.AddDeclarationToBlock (context);
            }
        }

        FunctionDeclarator? GetFunctionDeclarator (Declarator? d)
        {
            if (d == null) return null;
            else if (d is FunctionDeclarator) return (FunctionDeclarator)d;
            else return GetFunctionDeclarator (d.InnerDeclarator);
        }

        
    }
}
