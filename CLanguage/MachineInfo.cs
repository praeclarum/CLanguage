using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using CLanguage.Interpreter;
using CLanguage.Types;
using System.Reflection;
using System.Linq.Expressions;
using CLanguage.Compiler;

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
        public string GeneratedHeaderCode {
            get {
                var w = new CodeWriter ();
                w.WriteLine ("typedef char int8_t;");
                w.WriteLine ("typedef unsigned char uint8_t;");
                if (ShortIntSize == 2) {
                    w.WriteLine ("typedef short int16_t;");
                    w.WriteLine ("typedef unsigned short uint16_t;");
                }
                if (IntSize == 4) {
                    w.WriteLine ("typedef int int32_t;");
                    w.WriteLine ("typedef unsigned int uint32_t;");
                }
                else if (LongIntSize == 4) {
                    w.WriteLine ("typedef long int32_t;");
                    w.WriteLine ("typedef unsigned long uint32_t;");
                }
                w.Write (HeaderCode);
                return w.Code;
            }
        }

        public Dictionary<string, string> SystemHeadersCode = new Dictionary<string, string> ();

        public MachineInfo ()
		{
			InternalFunctions = new Collection<BaseFunction> ();
			HeaderCode = "";
            SystemHeadersCode["math.h"] = SystemHeaders.MathH;
		}

        public void AddInternalFunction (string prototype, InternalFunctionAction? action = null)
        {
            InternalFunctions.Add (new InternalFunction (this, prototype, action));
        }

        public void AddGlobalMethods (object target)
        {
            AddTargetMethods (null, target);
        }

        public void AddGlobalReference (string name, object target)
        {
            if (string.IsNullOrWhiteSpace (name))
                throw new ArgumentException ("Name must be specified", nameof (name));
            AddTargetMethods (name.Trim (), target);
        }

        void AddTargetMethods (string? name, object target)
        {
            if (target == null)
                throw new ArgumentNullException (nameof (target));

            var isRef = name != null;

            //
            // Find the methods to marshal
            //
            var type = target.GetType ();
            var typei = type.GetTypeInfo ();
            var allmethods = typei.DeclaredMethods.Where (x => !x.Attributes.HasFlag (MethodAttributes.Static)).ToArray ();
            var methods = allmethods.Where (x => !x.ContainsGenericParameters && !x.IsConstructor)
                .Where (x => x.ReturnType != typeof(string) && x.Name != "GetType" && x.Name != "Equals" && x.Name != "GetHashCode" && x.Name != "ToString");

            //
            // Generate the type code for the reference
            //
            var code = new CodeWriter ();
            var typeName = $"_{name}_t";
            code.WriteLine ("");
            if (isRef) {
                code.WriteLine ($"struct {typeName} {{").Indent ();
            }
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
                var mname = m.Name;
                if (m.IsSpecialName && mname.StartsWith ("set_", StringComparison.Ordinal)) {
                    mname = "set" + mname.Substring (4);
                }
                else if (m.IsSpecialName && mname.StartsWith ("get_", StringComparison.Ordinal)) {
                    mname = "get" + mname.Substring (4);
                }
                else if (char.IsUpper (mname[0])) {
                    mname = char.ToLowerInvariant (mname[0]) + mname.Substring (1);
                }
                pcode.Write ($"{mname}(");
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
            if (isRef) {
                code.Outdent ().WriteLine ("};");
                code.WriteLine ($"struct {typeName} {name};");
                HeaderCode += code;
            }

            foreach (var (m, mrt, pcode) in wmethods) {
                var proto = isRef ? $"{mrt} {typeName}::{pcode}" : $"{mrt} {pcode}";
                AddInternalFunction (proto, MarshalMethod (target, m));
            }
        }

        static readonly MethodInfo miReadArg = typeof (CInterpreter).GetTypeInfo().GetDeclaredMethod (nameof(CInterpreter.ReadArg))!;
        static readonly MethodInfo miPush = typeof (CInterpreter).GetTypeInfo().GetDeclaredMethod (nameof (CInterpreter.Push))!;
        static readonly MethodInfo miReadString = typeof (CInterpreter).GetTypeInfo().GetDeclaredMethod (nameof (CInterpreter.ReadString))!;

        static readonly Expression[] noExprs = new Expression[0];

        InternalFunctionAction MarshalMethod (object target, MethodInfo method)
        {
            var ps = method.GetParameters ();
            var nargs = ps.Length;

            var targetE = Expression.Constant (target);
            var interpreterE = Expression.Parameter (typeof (CInterpreter), "interpreter");

            var argsE = ps.Length > 0 ? new Expression[nargs] : noExprs;
            for (var i = 0; i < nargs; i++) {
                var pt = ps[i].ParameterType;
                var vargE = Expression.Call (interpreterE, miReadArg, Expression.Constant (i));
                argsE[i] = Expression.Field (vargE, ValueReflection.TypedFields[pt]);
                if (pt == typeof (string)) {
                    argsE[i] = Expression.Call (interpreterE, miReadString, argsE[i]);
                }
            }
            var resultE = Expression.Call (targetE, method, argsE);
            var bodyE = resultE;
            if (method.ReturnType != typeof (void)) {
                var valueResultE = Expression.Call (ValueReflection.CreateValueFromTypeMethods[method.ReturnType], resultE);
                bodyE = Expression.Call (interpreterE, miPush, valueResultE);
            }
            var ee = Expression.Lambda<InternalFunctionAction> (bodyE, interpreterE);
            return ee.Compile ();
        }

        string? ClrTypeToCode (Type type)
        {
            if (type == typeof (void))
                return "void";
            if (type == typeof (int))
                return IntSize == 4 ? "int" : "long";
            if (type == typeof (short))
                return "short";
            if (type == typeof (byte))
                return "unsigned char";
            if (type == typeof (float))
                return "float";
            if (type == typeof (double))
                return "double";
            if (type == typeof (string))
                return "const char *";
            if (type == typeof (char))
                return "char";
            if (type == typeof (uint))
                return IntSize == 4 ? "unsigned int" : "unsigned long";
            if (type == typeof (ushort))
                return "unsigned short";
            if (type == typeof (sbyte))
                return "signed char";
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

        public virtual ResolvedVariable? GetUnresolvedVariable (string name, CType[]? argTypes, EmitContext context)
        {
            return null;
        }
    }
}
