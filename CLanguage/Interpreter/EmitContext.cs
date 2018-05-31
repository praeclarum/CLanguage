using System;
using CLanguage.Ast;
using CLanguage.Interpreter;
using CLanguage.Types;
using ValueType = System.Int32;

namespace CLanguage
{
    public class EmitContext
    {
        public FunctionDeclaration FunctionDecl { get; private set; }

        public Report Report { get; private set; }

        public MachineInfo MachineInfo { get; private set; }

        public EmitContext (MachineInfo machineInfo, Report report, FunctionDeclaration fdecl = null)
        {
            if (machineInfo == null) throw new ArgumentNullException (nameof (machineInfo));
            if (report == null) throw new ArgumentNullException (nameof (report));

            MachineInfo = machineInfo;
            Report = report;
            FunctionDecl = fdecl;
        }

        public virtual ResolvedVariable ResolveVariable (string name)
        {
            return null;
        }

        public virtual void DeclareVariable (VariableDeclaration v) { }
        public virtual void DeclareFunction (FunctionDeclaration f) { }

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
            if (!fromType.Equals (toType)) {
                var fromBasicType = fromType as CBasicType;
                var toBasicType = toType as CBasicType;

                if (fromBasicType != null && fromBasicType.IsIntegral && toBasicType != null && toBasicType.IsIntegral) {
                    // This conversion is implicit with how the evaluator stores its stuff
                }
                else {
                    Report.Error (30, "Cannot convert type '" + fromType + "' to '" + toType + "'");
                }
            }
        }

        public void EmitCastToBoolean (CType fromType)
        {
            // Don't really need to do anything since 0 on that eval stack is false
        }

        public virtual void Emit (Instruction instruction)
        {
        }

        public void Emit (OpCode op, ValueType x)
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
    }

    public enum VariableScope
    {
        Global,
        Arg,
        Local,
        Function,
    }

    public class ResolvedVariable
    {
        public VariableScope Scope { get; private set; }
        public int Index { get; private set; }
        public CType VariableType { get; private set; }
        public BaseFunction Function { get; private set; }

        public ResolvedVariable (VariableScope scope, int index, CType variableType)
        {
            Scope = scope;
            Index = index;
            VariableType = variableType;
        }

        public ResolvedVariable (BaseFunction function, int index)
        {
            Scope = VariableScope.Function;
            Function = function;
            Index = index;
            VariableType = Function.FunctionType;
        }
    }
}
