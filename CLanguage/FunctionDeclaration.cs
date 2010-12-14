using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class ParameterInfo
    {
        public string Name { get; set; }
        public Expression InitExpression { get; set; }

        public ParameterInfo(string name)
        {
            Name = name;
        }
    }

    public class FunctionDeclaration : Declaration
    {
        public string Name { get; private set; }
        public CFunctionType FunctionType { get; private set; }
        public List<ParameterInfo> ParameterInfos { get; private set; }

        public Block Body { get; set; }

        public FunctionDeclaration(string name, CFunctionType functionType)
        {
            Name = name;
            FunctionType = functionType;
            ParameterInfos = new List<ParameterInfo>();
            foreach (var pt in FunctionType.Parameters)
            {
                ParameterInfos.Add(new ParameterInfo(""));
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            ec.DeclareFunction(this);
        }

        public override string ToString()
        {
            return Name + ": " + FunctionType;
        }
    }
}
