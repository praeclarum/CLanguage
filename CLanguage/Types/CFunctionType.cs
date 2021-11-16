using System;
using System.Collections.Generic;
using CLanguage.Compiler;

namespace CLanguage.Types
{
    public class CFunctionType : CType
    {
        public static readonly CFunctionType VoidProcedure = new CFunctionType (CType.Void, isInstance: false, declaringType: null);
        //public static readonly CFunctionType VoidMethod = new CFunctionType (CType.Void, isInstance: true);

        public class Parameter
        {
            public string Name { get; set; }
            public CType ParameterType { get; set; }
            public int Offset { get; set; }
            public Value? DefaultValue { get; set; }
            public Parameter(string name, CType parameterType, Value? defaultValue)
            {
                Name = name;
                ParameterType = parameterType;
                DefaultValue = defaultValue;
            }
            public override string ToString()
            {
                return ParameterType + " " + Name;
            }
        }

        public override int NumValues => 1;

        public CType ReturnType { get; private set; }

        readonly List<Parameter> parameters = new List<Parameter> ();
        public IReadOnlyList<Parameter> Parameters => parameters;

        public bool IsInstance { get; private set; }
        public CType? DeclaringType { get; }

        public CFunctionType(CType returnType, bool isInstance, CType? declaringType)
        {
            ReturnType = returnType;
            IsInstance = isInstance;
            DeclaringType = declaringType;
            if (IsInstance && DeclaringType == null)
                throw new ArgumentNullException (nameof (declaringType));
        }

        public void AddParameter (string name, CType type, Value? defaultValue)
        {
            parameters.Add (new Parameter (name, type, defaultValue));
            CalculateParameterOffsets ();
        }

        void CalculateParameterOffsets ()
        {
            var offset = IsInstance ? -1 : 0;
            for (var i = Parameters.Count - 1; i >= 0; --i) {
                var p = Parameters[i];
                var n = p.ParameterType.NumValues;
                offset -= n;
                p.Offset = offset;
            }
        }

        public override int GetByteSize(EmitContext c)
        {
            return c.MachineInfo.PointerSize;
        }

        public override string ToString()
        {
            var s = "(Function " + ReturnType + " (";
            var head = "";
            foreach (var p in Parameters)
            {
                s += head;
                s += p;
                head = " ";
            }
            s += "))";
            return s;
        }

        public int ScoreParameterTypeMatches (CType[]? argTypes)
        {
            if (argTypes == null)
                return 1;

            var pc = Parameters.Count;
            var requiredParamCount = 0;

            for (var i = 0; i < pc; i++) {
                if (Parameters[i].DefaultValue.HasValue)
                    break;
                requiredParamCount++;
            }

            if (argTypes.Length < requiredParamCount || argTypes.Length > pc)
                return 0;

            var score = argTypes.Length == pc ? 3 : 2;

            for (var i = 0; i < argTypes.Length; i++) {
                var ft = argTypes[i];
                var tt = Parameters[i].ParameterType;
                score += ft.ScoreCastTo (tt);
            }

            return score;
        }

        public bool ParameterTypesEqual (CFunctionType otherType)
        {
            if (Parameters.Count != otherType.Parameters.Count)
                return false;

            for (var i = 0; i < Parameters.Count; i++) {
                var ft = otherType.Parameters[i].ParameterType;
                var tt = Parameters[i].ParameterType;

                if (!ft.Equals (tt))
                    return false;
            }

            return true;
        }
    }
}
