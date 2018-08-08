using System;
namespace CLanguage.Syntax
{
    public class SizeOfTypeExpression
    {
        public TypeName TypeName { get; }

        public SizeOfTypeExpression (TypeName typeName)
        {
            TypeName = typeName;
        }
    }
}
