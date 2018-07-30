using System;
using CLanguage.Syntax;
using CLanguage.Interpreter;
using CLanguage.Types;

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
            if (!fromType.Equals (toType)) {
                var fromBasicType = fromType as CBasicType;
                var toBasicType = toType as CBasicType;

                if (fromBasicType != null && toBasicType != null) {
                    // This conversion is implicit with how the evaluator stores its stuff
                }
                else if (fromBasicType != null && fromBasicType.IsIntegral && toType is CPointerType) {
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
    }
}
