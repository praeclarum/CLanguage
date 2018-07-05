using System;
using CLanguage.Interpreter;

namespace CLanguage.Types
{
    public abstract class CBasicType : CType
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

        public static readonly CIntType ConstChar = new CIntType("char", Signedness.Signed, "") { TypeQualifiers = TypeQualifiers.Const };
        public static readonly CIntType UnsignedChar = new CIntType("char", Signedness.Unsigned, "");
        public static readonly CIntType SignedChar = new CIntType("char", Signedness.Signed, "");
        public static readonly CIntType UnsignedShortInt = new CIntType("int", Signedness.Unsigned, "short");
        public static readonly CIntType SignedShortInt = new CIntType("int", Signedness.Signed, "short");
        public static readonly CIntType UnsignedInt = new CIntType("int", Signedness.Unsigned, "");
        public static readonly CIntType SignedInt = new CIntType("int", Signedness.Signed, "");
        public static readonly CIntType UnsignedLongInt = new CIntType("int", Signedness.Unsigned, "long");
        public static readonly CIntType SignedLongInt = new CIntType("int", Signedness.Signed, "long");
        public static readonly CIntType UnsignedLongLongInt = new CIntType("int", Signedness.Unsigned, "long long");
        public static readonly CIntType SignedLongLongInt = new CIntType("int", Signedness.Signed, "long long");
        public static readonly CFloatType Float = new CFloatType("float", 32);
        public static readonly CFloatType Double = new CFloatType("double", 64);
        public static readonly CBoolType Bool = new CBoolType ();

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
                                return new CIntType(p2.Name, Signedness.Unsigned, p2.Size);
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
                                return new CIntType(p1.Name, Signedness.Unsigned, p1.Size);
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
