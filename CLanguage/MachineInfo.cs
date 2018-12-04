using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using CLanguage.Interpreter;
using CLanguage.Types;
using System.Reflection;

namespace CLanguage
{
    public class MachineInfo
    {
        public int CharSize { get; set; } = 1;
        public int ShortIntSize { get; set; } = 2;
        public int IntSize { get; set; } = 4;
        public int LongIntSize { get; set; } = 4;
        public int LongLongIntSize { get; set; } = 8;
        public int FloatSize { get; set; } = 4;
        public int DoubleSize { get; set; } = 8;
        public int LongDoubleSize { get; set; } = 8;
        public int PointerSize { get; set; } = 4;

		public string HeaderCode { get; set; }

		public Collection<BaseFunction> InternalFunctions { get; set; }

		public MachineInfo ()
		{
			InternalFunctions = new Collection<BaseFunction> ();
			HeaderCode = "";
		}

        public void AddInternalFunction (string prototype, InternalFunctionAction action = null)
        {
            InternalFunctions.Add (new InternalFunction (this, prototype, action));
        }

        public void AddGlobalReference (string name, object target)
        {
            if (string.IsNullOrWhiteSpace (name))
                throw new ArgumentException ("Name must be specified", nameof (name));
            if (target == null)
                throw new ArgumentNullException (nameof (target));

            var type = target.GetType ();
            var allmethods = type.GetMethods (BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
            var methods = allmethods.Where (x => !x.IsSpecialName && !x.ContainsGenericParameters && !x.IsConstructor)
                .Where (x => x.Name != "GetType" && x.Name != "Equals" && x.Name != "GetHashCode");
            var code = new CodeWriter ();
            var typeName = $"_{name}_t";
            code.WriteLine ("");
            code.WriteLine ($"struct {typeName} {{").Indent ();
            var wmethods = new List<(MethodInfo Method, string ReturnType, string Prototype)> ();
            foreach (var m in methods) {
                var mrt = ClrTypeToCode (m.ReturnType);
                if (mrt == null)
                    continue;
                var ps = m.GetParameters ().Select (x => (ClrTypeToCode (x.ParameterType), x.Name)).ToList ();
                if (ps.Any (x => x.Item1 == null))
                    continue;
                code.Write (mrt);
                code.Write (" ");
                var pcode = new CodeWriter ();
                pcode.Write ($"{m.Name}(");
                var head = "";
                foreach (var (t, n) in ps) {
                    pcode.Write (head);
                    pcode.Write ($"{t} {n}");
                    head = ", ";
                }
                pcode.Write (")");
                code.Write (pcode.Code);
                code.WriteLine (";");
                wmethods.Add ((m, mrt, pcode.Code));
            }
            code.Outdent ().WriteLine ("};");
            code.WriteLine ($"struct {typeName} {name};");
            HeaderCode += code;

            foreach (var (m, mrt, pcode) in wmethods) {
                var proto = $"{mrt} {typeName}::{pcode}";
                AddInternalFunction (proto, MarshalMethod (target, m));
            }
        }

        InternalFunctionAction MarshalMethod (object target, MethodInfo method)
        {
            void Execute (CInterpreter interpreter)
            {
                var ps = method.GetParameters ();
                var args = ps.Length > 0 ? new object[ps.Length] : Array.Empty<object> ();
                for (var i = 0; i < args.Length; i++) {
                    args[i] = (int)interpreter.ReadArg (i);
                }
                var mr = method.Invoke (target, args);
                var r = mr is int ir ? ir : 0;
                if (method.ReturnType != typeof (void)) {
                    interpreter.Push (r);
                }
                //throw new Exception ($"Cannot call '{method}' on '{target}'");
            }

            return Execute;
        }

        string ClrTypeToCode (Type type)
        {
            if (type == typeof (void))
                return "void";
            if (type == typeof (int))
                return IntSize == 4 ? "int" : "long";
            if (type == typeof (short))
                return "short";
            return null;
        }

        public static readonly MachineInfo Windows32 = new MachineInfo
        {
            CharSize = 1,
            ShortIntSize = 2,
            IntSize = 4,
            LongIntSize = 4,
            LongLongIntSize = 8,
            FloatSize = 4,
            DoubleSize = 8,
            LongDoubleSize = 8,
            PointerSize = 4,
        };

		public static readonly MachineInfo Mac64 = new MachineInfo
		{
			CharSize = 1,
			ShortIntSize = 2,
			IntSize = 4,
			LongIntSize = 8,
			LongLongIntSize = 8,
			FloatSize = 4,
			DoubleSize = 8,
			LongDoubleSize = 8,
			PointerSize = 8,
		};

        public virtual ResolvedVariable GetUnresolvedVariable (string name, CType[] argTypes, EmitContext context)
        {
            return null;
        }
    }
}
