using System;
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

        public override string ToString ()
        {
            return Int32Value.ToString ();
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
}
