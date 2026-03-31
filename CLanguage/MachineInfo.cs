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

        static readonly Dictionary<string, string> CSharpOperatorNames = new Dictionary<string, string> {
            { "op_Addition", "operator+" },
            { "op_Subtraction", "operator-" },
            { "op_Multiply", "operator*" },
            { "op_Division", "operator/" },
            { "op_Modulus", "operator%" },
            { "op_Equality", "operator==" },
            { "op_Inequality", "operator!=" },
            { "op_LessThan", "operator<" },
            { "op_GreaterThan", "operator>" },
            { "op_LessThanOrEqual", "operator<=" },
            { "op_GreaterThanOrEqual", "operator>=" },
            { "op_BitwiseAnd", "operator&" },
            { "op_BitwiseOr", "operator|" },
            { "op_ExclusiveOr", "operator^" },
            { "op_LeftShift", "operator<<" },
            { "op_RightShift", "operator>>" },
            { "op_UnaryNegation", "operator-" },
            { "op_LogicalNot", "operator!" },
        };

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

            //
            // Detect static C# operator methods and register them as member operators
            //
            var operatorMethods = new List<(MethodInfo Method, string OpName, string ReturnType, string Prototype)> ();
            if (isRef) {
                var staticMethods = typei.DeclaredMethods.Where (x => x.Attributes.HasFlag (MethodAttributes.Static) && x.IsSpecialName && !x.ContainsGenericParameters);
                foreach (var m in staticMethods) {
                    if (!CSharpOperatorNames.TryGetValue (m.Name, out var opName))
                        continue;
                    var mrt = ClrTypeToCode (m.ReturnType);
                    if (mrt == null)
                        continue;
                    var ps = m.GetParameters ();
                    // Binary operator: first param is the declaring type, rest are marshallable
                    // Unary operator: single param is the declaring type
                    if (ps.Length < 1)
                        continue;
                    // First parameter must be the declaring type (becomes implicit this)
                    if (ps[0].ParameterType != type)
                        continue;
                    var remainingParams = ps.Skip (1).Select (x => (ClrTypeToCode (x.ParameterType), x.Name ?? "arg")).ToList ();
                    if (remainingParams.Any (x => x.Item1 == null))
                        continue;
                    code.Write (mrt);
                    code.Write (" ");
                    var pcode = new CodeWriter ();
                    pcode.Write ($"{opName}(");
                    var head = "";
                    foreach (var (t, n) in remainingParams) {
                        pcode.Write (head);
                        pcode.Write ($"{t} {n}");
                        head = ", ";
                    }
                    pcode.Write (")");
                    code.Write (pcode.Code);
                    code.WriteLine (";");
                    operatorMethods.Add ((m, opName, mrt, pcode.Code));
                }
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

            foreach (var (m, opName, mrt, pcode) in operatorMethods) {
                var proto = $"{mrt} {typeName}::{pcode}";
                AddInternalFunction (proto, MarshalStaticOperator (target, m));
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

        /// <summary>
        /// Marshals a static C# operator method as a member operator internal function.
        /// The first C# parameter (the declaring type) is skipped — it corresponds
        /// to the implicit <c>this</c> pointer. Remaining C# parameters map to
        /// the C function arguments starting at index 0.
        /// </summary>
        InternalFunctionAction MarshalStaticOperator (object target, MethodInfo method)
        {
            var ps = method.GetParameters ();
            // Skip the first parameter (the declaring type, mapped to this)
            var nargs = ps.Length - 1;

            var interpreterE = Expression.Parameter (typeof (CInterpreter), "interpreter");

            var argsE = new Expression[ps.Length];
            // First argument: read this pointer's value (ReadArg for the 'this' is not available for static;
            // use ReadArg(0..n-1) for the remaining params, skip the declaring type param)
            // For static operators registered as member functions, the remaining params start at arg 0.
            for (var i = 0; i < ps.Length; i++) {
                var pt = ps[i].ParameterType;
                if (i == 0) {
                    // First C# parameter corresponds to the declaring type (this pointer value).
                    // For the interop struct, this is opaque — pass a default value.
                    argsE[i] = Expression.Default (pt);
                }
                else if (ValueReflection.TypedFields.ContainsKey (pt)) {
                    var vargE = Expression.Call (interpreterE, miReadArg, Expression.Constant (i - 1));
                    argsE[i] = Expression.Field (vargE, ValueReflection.TypedFields[pt]);
                    if (pt == typeof (string)) {
                        argsE[i] = Expression.Call (interpreterE, miReadString, argsE[i]);
                    }
                }
                else {
                    argsE[i] = Expression.Default (pt);
                }
            }
            var resultE = Expression.Call (null, method, argsE);
            Expression bodyE = resultE;
            if (method.ReturnType != typeof (void)) {
                if (ValueReflection.CreateValueFromTypeMethods.TryGetValue (method.ReturnType, out var createMethod)) {
                    var valueResultE = Expression.Call (createMethod, resultE);
                    bodyE = Expression.Call (interpreterE, miPush, valueResultE);
                }
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
