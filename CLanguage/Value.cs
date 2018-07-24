using System;
using System.Runtime.InteropServices;

namespace CLanguage
{
    [StructLayout (LayoutKind.Explicit, Size = 12)]
    public struct Value
    {
        [FieldOffset (0)]
        public System.Double Float64Value;
        [FieldOffset (0)]
        public System.Int64 Int64Value;
        [FieldOffset (0)]
        public System.Single Float32Value;
        [FieldOffset (0)]
        public System.Int32 Int32Value;
        [FieldOffset (8)]
        public ValueType Type;

        public override string ToString ()
        {
            switch (Type) {
                case ValueType.Int32:
                    return Int32Value.ToString ();
                case ValueType.Int64:
                    return Int64Value.ToString ();
                case ValueType.Float32:
                    return Float32Value.ToString ();
                case ValueType.Float64:
                    return Float64Value.ToString ();
                default:
                    return "";
            }
        }

        public static implicit operator Value (float v)
        {
            return new Value {
                Float32Value = v,
                Type = ValueType.Float32,
            };
        }

        public static implicit operator Value (double v)
        {
            return new Value {
                Float64Value = v,
                Type = ValueType.Float64,
            };
        }

        public static implicit operator Value (ulong v)
        {
            return new Value {
                Int64Value = (long)v,
                Type = ValueType.Int64,
            };
        }

        public static implicit operator Value (long v)
        {
            return new Value {
                Int64Value = v,
                Type = ValueType.Int64,
            };
        }

        public static implicit operator Value (uint v)
        {
            return new Value {
                Int32Value = (int)v,
                Type = ValueType.Int32,
            };
        }

        public static implicit operator Value (int v)
        {
            return new Value {
                Int32Value = v,
                Type = ValueType.Int32,
            };
        }

        public static implicit operator Value (ushort v)
        {
            return new Value {
                Int32Value = v,
                Type = ValueType.Int32,
            };
        }

        public static implicit operator Value (short v)
        {
            return new Value {
                Int32Value = v,
                Type = ValueType.Int32,
            };
        }

        public static implicit operator Value (byte v)
        {
            return new Value {
                Int32Value = v,
                Type = ValueType.Int32,
            };
        }

        public static implicit operator Value (sbyte v)
        {
            return new Value {
                Int32Value = v,
                Type = ValueType.Int32,
            };
        }

        public static implicit operator float (Value v)
        {
            switch (v.Type) {
                case ValueType.Int32:
                    return (float)v.Int32Value;
                case ValueType.Int64:
                    return (float)v.Int64Value;
                case ValueType.Float32:
                    return (float)v.Float32Value;
                case ValueType.Float64:
                    return (float)v.Float64Value;
                default:
                    return 0;
            }
        }

        public static implicit operator double (Value v)
        {
            switch (v.Type) {
                case ValueType.Int32:
                    return (double)v.Int32Value;
                case ValueType.Int64:
                    return (double)v.Int64Value;
                case ValueType.Float32:
                    return (double)v.Float32Value;
                case ValueType.Float64:
                    return (double)v.Float64Value;
                default:
                    return 0;
            }
        }

        public static implicit operator ulong (Value v)
        {
            switch (v.Type) {
                case ValueType.Int32:
                    return (ulong)v.Int32Value;
                case ValueType.Int64:
                    return (ulong)v.Int64Value;
                case ValueType.Float32:
                    return (ulong)v.Float32Value;
                case ValueType.Float64:
                    return (ulong)v.Float64Value;
                default:
                    return 0;
            }
        }

        public static implicit operator long (Value v)
        {
            switch (v.Type) {
                case ValueType.Int32:
                    return (long)v.Int32Value;
                case ValueType.Int64:
                    return (long)v.Int64Value;
                case ValueType.Float32:
                    return (long)v.Float32Value;
                case ValueType.Float64:
                    return (long)v.Float64Value;
                default:
                    return 0;
            }
        }

        public static implicit operator uint (Value v)
        {
            switch (v.Type) {
                case ValueType.Int32:
                    return (uint)v.Int32Value;
                case ValueType.Int64:
                    return (uint)v.Int64Value;
                case ValueType.Float32:
                    return (uint)v.Float32Value;
                case ValueType.Float64:
                    return (uint)v.Float64Value;
                default:
                    return 0;
            }
        }

        public static implicit operator int (Value v)
        {
            switch (v.Type) {
                case ValueType.Int32:
                    return v.Int32Value;
                case ValueType.Int64:
                    return (int)v.Int64Value;
                case ValueType.Float32:
                    return (int)v.Float32Value;
                case ValueType.Float64:
                    return (int)v.Float64Value;
                default:
                    return 0;
            }
        }

        public static implicit operator ushort (Value v)
        {
            switch (v.Type) {
                case ValueType.Int32:
                    return (ushort)v.Int32Value;
                case ValueType.Int64:
                    return (ushort)v.Int64Value;
                case ValueType.Float32:
                    return (ushort)v.Float32Value;
                case ValueType.Float64:
                    return (ushort)v.Float64Value;
                default:
                    return 0;
            }
        }

        public static implicit operator short (Value v)
        {
            switch (v.Type) {
                case ValueType.Int32:
                    return (short)v.Int32Value;
                case ValueType.Int64:
                    return (short)v.Int64Value;
                case ValueType.Float32:
                    return (short)v.Float32Value;
                case ValueType.Float64:
                    return (short)v.Float64Value;
                default:
                    return 0;
            }
        }

        public static implicit operator byte (Value v)
        {
            switch (v.Type) {
                case ValueType.Int32:
                    return (byte)v.Int32Value;
                case ValueType.Int64:
                    return (byte)v.Int64Value;
                case ValueType.Float32:
                    return (byte)v.Float32Value;
                case ValueType.Float64:
                    return (byte)v.Float64Value;
                default:
                    return 0;
            }
        }

        public static implicit operator sbyte (Value v)
        {
            switch (v.Type) {
                case ValueType.Int32:
                    return (sbyte)v.Int32Value;
                case ValueType.Int64:
                    return (sbyte)v.Int64Value;
                case ValueType.Float32:
                    return (sbyte)v.Float32Value;
                case ValueType.Float64:
                    return (sbyte)v.Float64Value;
                default:
                    return 0;
            }
        }
    }

    public enum ValueType
    {
        Int32,
        Int64,
        Float32,
        Float64,
    }
}
