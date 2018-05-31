using System.Collections.Generic;

namespace CLanguage.Types
{
    public class CFunctionType : CType
    {
        public class Parameter
        {
            public string Name { get; set; }
            public CType ParameterType { get; set; }
            public Parameter(string name, CType parameterType)
            {
                Name = name;
                ParameterType = parameterType;
            }
            public override string ToString()
            {
                return ParameterType + " " + Name;
            }
        }

        public CType ReturnType { get; private set; }
        public List<Parameter> Parameters { get; private set; }

        public CFunctionType(CType returnType)
        {
            ReturnType = returnType;
            Parameters = new List<Parameter>();
        }

        public override int GetSize(EmitContext c)
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
    }


}
