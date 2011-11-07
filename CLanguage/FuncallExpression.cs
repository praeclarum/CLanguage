using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CLanguage
{
    public class FuncallExpression : Expression
    {
        public Expression Function { get; set; }
        public List<Expression> Arguments { get; set; }

        public FuncallExpression(Expression fun)
        {
            Function = fun;
            Arguments = new List<Expression>();
        }

        public FuncallExpression(Expression fun, IEnumerable<Expression> args)
        {
            Function = fun;
            Arguments = new List<Expression>(args);
        }

        public override CType ExpressionType
        {
            get
            {
                var ft = Function.ExpressionType as CFunctionType;
                if (ft != null)
                {
                    return ft.ReturnType;
                }
                else
                {
                    return CType.Void;
                }
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            var type = Function.ExpressionType as CFunctionType;

            Function.Emit(ec);

            var argsCount = Arguments.Count;

            foreach (var a in Arguments)
            {
                a.Emit(ec);
            }

            if (type != null)
            {
                
                
            }
            else
            {
                //if (!Function.HasError)
                {
                    ec.Report.Error(2064, "'{0}' does not evaluate to a function taking {1} arguments", Function, Arguments.Count);
                }
            }

            ec.EmitCall(type, argsCount);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Function.ToString());
            sb.Append("(");
            var head = "";
            foreach (var a in Arguments)
            {
                sb.Append(head);
                sb.Append(a.ToString());
                head = ", ";
            }
            sb.Append(")");
            return sb.ToString();
        }
    }
}
