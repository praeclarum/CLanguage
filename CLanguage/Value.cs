using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CLanguage
{
    [StructLayout (LayoutKind.Explicit, Size = 8)]
    public struct Value
    {
        [FieldOffset (0)]
        public System.Double Float64Value;
        [FieldOffset (0)]
        public System.Int64 Int64Value;
        [FieldOffset (0)]
        public System.UInt64 UInt64Value;
        [FieldOffset (0)]
        public System.Single Float32Value;
        [FieldOffset (0)]
        public System.Int32 Int32Value;
        [FieldOffset (0)]
        public System.UInt32 UInt32Value;
        [FieldOffset (0)]
        public System.Int16 Int16Value;
        [FieldOffset (0)]
        public System.UInt16 UInt16Value;
        [FieldOffset (0)]
        public System.SByte Int8Value;
        [FieldOffset (0)]
        public System.Byte UInt8Value;
        [FieldOffset (0)]
        public System.Int32 PointerValue;
        [FieldOffset (0)]
        public System.Char CharValue;

        public override string ToString ()
        {
            return Int32Value.ToString ();
        }

        public static implicit operator Value (string v)
        {
            // Used for marshalling, but not actually allowed
            return new Value ();
        }

        public static implicit operator Value (char v)
        {
            return new Value {
                CharValue = v,
            };
        }

        public static implicit operator Value (float v)
        {
            return new Value {
                Float32Value = v,
            };
        }

        public static implicit operator Value (double v)
        {
            return new Value {
                Float64Value = v,
            };
        }

        public static implicit operator Value (ulong v)
        {
            return new Value {
                UInt64Value = v,
            };
        }

        public static implicit operator Value (long v)
        {
            return new Value {
                Int64Value = v,
            };
        }

        public static implicit operator Value (uint v)
        {
            return new Value {
                UInt32Value = v,
            };
        }

        public static implicit operator Value (int v)
        {
            return new Value {
                Int32Value = v,
            };
        }

        public static implicit operator Value (ushort v)
        {
            return new Value {
                UInt16Value = v,
            };
        }

        public static implicit operator Value (short v)
        {
            return new Value {
                Int16Value = v,
            };
        }

        public static implicit operator Value (byte v)
        {
            return new Value {
                UInt8Value = v,
            };
        }

        public static implicit operator Value (sbyte v)
        {
            return new Value {
                Int8Value = v,
            };
        }

        public static explicit operator float (Value v)
        {
            return v.Float32Value;
        }

        public static explicit operator double (Value v)
        {
            return v.Float64Value;
        }

        public static explicit operator ulong (Value v)
        {
            return v.UInt64Value;
        }

        public static explicit operator long (Value v)
        {
            return v.Int64Value;
        }

        public static explicit operator uint (Value v)
        {
            return v.UInt32Value;
        }

        public static explicit operator int (Value v)
        {
            return v.Int32Value;
        }

        public static explicit operator ushort (Value v)
        {
            return v.UInt16Value;
        }

        public static explicit operator short (Value v)
        {
            return v.Int16Value;
        }

        public static explicit operator byte (Value v)
        {
            return v.UInt8Value;
        }

        public static explicit operator sbyte (Value v)
        {
            return v.Int8Value;
        }

        public static Value Pointer (int address) => new Value {
            PointerValue = address,
        };
    }

    static class ValueReflection
    {
        public static readonly Dictionary<Type, FieldInfo> TypedFields =
            (from f in typeof (Value).GetFields (BindingFlags.Instance | BindingFlags.Public)
             where f.Name.EndsWith ("Value", StringComparison.InvariantCultureIgnoreCase) &&
                 !f.Name.StartsWith ("Pointer", StringComparison.InvariantCultureIgnoreCase)
             select (f.FieldType, f)).ToDictionary (x => x.Item1, x => x.Item2);

        public static readonly Dictionary<Type, MethodInfo> CreateValueFromTypeMethods =
            (from m in typeof (Value).GetMethods (BindingFlags.Static | BindingFlags.Public)
             where m.Name.StartsWith ("op", StringComparison.InvariantCultureIgnoreCase)
             let rt = m.ReturnType
             where rt == typeof (Value)
             let ps = m.GetParameters ()
             where ps.Length == 1
             select (ps[0].ParameterType, m)).ToDictionary (x => x.Item1, x => x.Item2);

        static ValueReflection ()
        {
            TypedFields[typeof (string)] = typeof (Value).GetField (nameof (Value.PointerValue));
        }
    }
}
