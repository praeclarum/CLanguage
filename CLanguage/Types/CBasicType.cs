using System;

namespace CLanguage.Types
{
    public class CBasicType : CType
    {
        public string Name { get; private set; }
        public Signedness Signedness { get; private set; }
        public string Size { get; private set; }

        public CBasicType(string name, Signedness signedness, string size)
        {
            Name = name;
            Signedness = signedness;
            Size = size;
        }

        public override bool Equals(object obj)
        {
            var o = obj as CBasicType;
            return (o != null) && (Name == o.Name) && (Signedness == o.Signedness) && (Size == o.Size);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + Size.GetHashCode() + Signedness.GetHashCode();
        }

        public override bool IsIntegral
        {
            get
            {
                return Name == "int" || Name == "char";
            }
        }

        public static readonly CBasicType ConstChar = new CBasicType("char", Signedness.Unsigned, "") { TypeQualifiers = TypeQualifiers.Const };
        public static readonly CBasicType UnsignedChar = new CBasicType("char", Signedness.Unsigned, "");
        public static readonly CBasicType SignedChar = new CBasicType("char", Signedness.Signed, "");
        public static readonly CBasicType UnsignedShortInt = new CBasicType("int", Signedness.Unsigned, "short");
        public static readonly CBasicType SignedShortInt = new CBasicType("int", Signedness.Signed, "short");
        public static readonly CBasicType UnsignedInt = new CBasicType("int", Signedness.Unsigned, "");
        public static readonly CBasicType SignedInt = new CBasicType("int", Signedness.Signed, "");
        public static readonly CBasicType UnsignedLongInt = new CBasicType("int", Signedness.Unsigned, "long");
        public static readonly CBasicType SignedLongInt = new CBasicType("int", Signedness.Signed, "long");
        public static readonly CBasicType UnsignedLongLongInt = new CBasicType("int", Signedness.Unsigned, "long long");
        public static readonly CBasicType SignedLongLongInt = new CBasicType("int", Signedness.Signed, "long long");
        public static readonly CBasicType Float = new CBasicType("float", Signedness.Signed, "");
        public static readonly CBasicType Double = new CBasicType("double", Signedness.Signed, "");

        public override int GetSize(EmitContext c)
        {
            if (Name == "char")
            {
                return c.MachineInfo.CharSize;
            }
            else if (Name == "int")
            {
                if (Size == "short")
                {
                    return c.MachineInfo.ShortIntSize;
                }
                else if (Size == "long")
                {
                    return c.MachineInfo.LongIntSize;
                }
                else if (Size == "long long")
                {
                    return c.MachineInfo.LongLongIntSize;
                }
                else
                {
                    return c.MachineInfo.IntSize;
                }
            }
            else if (Name == "float")
            {
                return c.MachineInfo.FloatSize;
            }
            else if (Name == "double")
            {
                return c.MachineInfo.DoubleSize;
            }
            else
            {
                throw new NotSupportedException(this.ToString());
            }
        }

        /// <summary>
        /// Section 6.3.1.1 (page 51) of N1570
        /// </summary>
        /// <returns>
        /// The promototed integer.
        /// </returns>
        /// <param name='context'>
        /// Context.
        /// </param>
        public CBasicType IntegerPromote(EmitContext context)
        {
            if (IsIntegral)
            {
                var size = GetSize(context);
                var intSize = context.MachineInfo.IntSize;
                if (size < intSize)
                {
                    return SignedInt;
                }
                else if (size == intSize)
                {
                    if (Signedness == Signedness.Unsigned)
                    {
                        return UnsignedInt;
                    }
                    else
                    {
                        return SignedInt;
                    }
                }
                else
                {
                    return this;
                }
            }
            else
            {
                return this;
            }
        }

        /// <summary>
        /// Section 6.3.1.8 (page 53) of N1570
        /// </summary>
        /// <returns>
        /// The converted type.
        /// </returns>
        /// <param name='otherType'>
        /// The other type participating in the arithmetic.
        /// </param>
        /// <param name='context'>
        /// Context.
        /// </param>
        public CBasicType ArithmeticConvert(CType otherType, EmitContext context)
        {
            var otherBasicType = otherType as CBasicType;
            if (otherBasicType == null)
            {
                context.Report.Error(19, "Cannot perform arithmetic with " + otherType);
                return CBasicType.SignedInt;
            }
            if (Name == "double" || otherBasicType.Name == "double")
            {
                return Double;
            }
            else if (Name == "single" || otherBasicType.Name == "single")
            {
                return Float;
            }
            else
            {

                var p1 = IntegerPromote(context);
                var size1 = p1.GetSize(context);

                var p2 = otherBasicType.IntegerPromote(context);
                var size2 = p2.GetSize(context);

                if (p1.Signedness == p2.Signedness)
                {
                    return size1 >= size2 ? p1 : p2;
                }
                else
                {

                    if (p1.Signedness == Signedness.Unsigned)
                    {

                        if (size1 > size2)
                        {//p1.HasRankGreaterThan (p2, context)) {
                            return p1;
                        }
                        else
                        {
                            if (size2 > size1)
                            {
                                return p2;
                            }
                            else
                            {
                                return new CBasicType(p2.Name, Signedness.Unsigned, p2.Size);
                            }
                        }
                    }
                    else
                    {

                        if (size2 > size1)
                        {//p2.HasRankGreaterThan (p1, context)) {
                            return p2;
                        }
                        else
                        {
                            if (size1 > size2)
                            {
                                return p1;
                            }
                            else
                            {
                                return new CBasicType(p1.Name, Signedness.Unsigned, p1.Size);
                            }
                        }
                    }
                }
            }
        }

        bool HasRankGreaterThan(CBasicType otherBasicType, EmitContext context)
        {
            return false;
        }

        public override string ToString()
        {
            if (IsIntegral)
            {

                var sign = Signedness == Signedness.Signed ? "signed" : "unsigned";

                if (string.IsNullOrEmpty(Size))
                {
                    return sign + " " + Name;
                }
                else
                {
                    return sign + " " + Size + " " + Name;
                }

            }
            else
            {
                if (string.IsNullOrEmpty(Size))
                {
                    return Name;
                }
                else
                {
                    return Size + " " + Name;
                }
            }
        }
    }

    public enum Signedness
    {
        Unsigned = 0,
        Signed = 1,
    }
}
