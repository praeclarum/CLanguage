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

		public override CType GetEvaluatedCType (EmitContext ec)
		{
            var ft = Function.GetEvaluatedCType (ec) as CFunctionType;
            if (ft != null)
            {
                return ft.ReturnType;
            }
            else
            {
                return CType.Void;
            }
        }

        protected override void DoEmit(EmitContext ec)
        {
            var type = Function.GetEvaluatedCType (ec) as CFunctionType;

            if (type != null)
            {
				if (type.Parameters.Count != Arguments.Count) {
					ec.Report.Error (1501, "'{0}' takes {1} arguments, {2} provided", Function, type.Parameters.Count, Arguments.Count);
				}
            }
            else
            {
                //if (!Function.HasError)
                {
                    ec.Report.Error(2064, "'{0}' does not evaluate to a function taking {1} arguments", Function, Arguments.Count);
                }
            }

			foreach (var a in Arguments)
			{
				a.Emit(ec);
			}

			Function.Emit (ec);

            ec.EmitCall (type);
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
