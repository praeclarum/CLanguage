using System;
using System.Collections.Generic;
using System.Linq;
using CLanguage.Syntax;
using CLanguage.Types;

namespace CLanguage.Interpreter
{
    class FunctionContext : EmitContext
    {
        Executable exe;
        CompiledFunction fexe;
        EmitContext context;

        class BlockLocals
        {
            public int StartIndex;
            public int Length;
        }
        List<Block> blocks;
        Dictionary<Block, BlockLocals> blockLocals;
        List<CompiledVariable> allLocals;

        public IEnumerable<CompiledVariable> LocalVariables { get { return allLocals; } }

        public FunctionContext (Executable exe, CompiledFunction fexe, EmitContext context)
            : base (context.MachineInfo, context.Report, fexe)
        {
            this.exe = exe;
            this.fexe = fexe;
            this.context = context;
            blocks = new List<Block> ();
            blockLocals = new Dictionary<Block, BlockLocals> ();
            allLocals = new List<CompiledVariable> ();
        }

        public override ResolvedVariable ResolveVariable (string name, CType[] argTypes)
        {
            //
            // Look for function parameters
            //
            for (var i = 0; i < fexe.FunctionType.Parameters.Count; i++) {
                var p = fexe.FunctionType.Parameters[i];
                if (p.Name == name) {
                    return new ResolvedVariable (VariableScope.Arg, p.Offset, fexe.FunctionType.Parameters[i].ParameterType);
                }
            }

            //
            // Look for locals
            //
            foreach (var b in blocks.Reverse<Block> ()) {
                var blocals = blockLocals[b];
                for (var i = 0; i < blocals.Length; i++) {
                    var j = blocals.StartIndex + i;
                    if (allLocals[j].Name == name) {
                        return new ResolvedVariable (VariableScope.Local, allLocals[j].Offset, allLocals[j].VariableType);
                    }
                }
            }

            //
            // Look for global variables
            //
            for (var i = 0; i < exe.Globals.Count; i++) {
                if (exe.Globals[i].Name == name) {
                    return new ResolvedVariable (VariableScope.Global, exe.Globals[i].Offset, exe.Globals[i].VariableType);
                }
            }

            //
            // Look for global functions
            //
            BaseFunction ff = null;
            var fi = -1;
            var fs = 0;
            for (var i = 0; i < exe.Functions.Count; i++) {
                var f = exe.Functions[i];
                if (f.Name == name && string.IsNullOrEmpty (f.NameContext)) {
                    var score = f.FunctionType.ScoreParameterTypesMatches (argTypes);
                    if (score > fs) {
                        ff = f;
                        fi = i;
                        fs = score;
                    }
                }
            }
            if (ff != null) {
                return new ResolvedVariable (ff, fi);
            }

            context.Report.Error (103, "The name '" + name + "' does not exist in the current context");
            return null;
        }

        public override ResolvedVariable ResolveMethodFunction (CStructType structType, CStructMethod method)
        {
            if (method.MemberType is CFunctionType ftype) {

                var nameContext = structType.Name;

                var funcs = exe.Functions.Select ((f, i) => (f, i)).Where (x => x.Item1.NameContext == nameContext && x.Item1.Name == method.Name);
                for (var i = 0; i < exe.Functions.Count; i++) {
                    var f = exe.Functions[i];
                    if (f.NameContext == nameContext && f.Name == method.Name && f.FunctionType.ParameterTypesEqual (ftype)) {
                        return new ResolvedVariable (f, i);
                    }
                }
            }

            context.Report.Error (9000, $"No definition for '{structType.Name}::{method.Name}' found");
            return null;
        }

        public override void BeginBlock (Block b)
        {
            blocks.Add (b);
            var locs = new BlockLocals {
                StartIndex = allLocals.Count,
                Length = b.Variables.Count,
            };
            blockLocals[b] = locs;
            allLocals.AddRange (b.Variables);

            var offset = 0;
            foreach (var v in allLocals) {
                v.Offset = offset;
                offset += v.VariableType.NumValues;
            }
        }

        public override void EndBlock ()
        {
            blocks.RemoveAt (blocks.Count - 1);
        }

        public override Label DefineLabel ()
        {
            return new Label ();
        }

        public override void EmitLabel (Label l)
        {
            l.Index = fexe.Instructions.Count;
        }

        public override void Emit (Instruction instruction)
        {
            fexe.Instructions.Add (instruction);
        }

        public override Value GetConstantMemory (string stringConstant)
        {
            return exe.GetConstantMemory (stringConstant);
        }
    }
}
