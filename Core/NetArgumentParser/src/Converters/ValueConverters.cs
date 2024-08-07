using System;
using System.Diagnostics.CodeAnalysis;

namespace NetArgumentParser.Converters;

public static class ValueConverters
{
    public static ValueConverter<bool> BooleanConverter => new(Convert.ToBoolean);
    public static ValueConverter<byte> ByteConverter => new(Convert.ToByte);
    public static ValueConverter<char> CharConverter => new(Convert.ToChar);
    public static ValueConverter<DateTime> DateTimeConverter => new(Convert.ToDateTime);
    public static ValueConverter<decimal> DecimalConverter => new(Convert.ToDecimal);
    public static ValueConverter<double> DoubleConverter => new(Convert.ToDouble);
    public static ValueConverter<short> Int16Converter => new(Convert.ToInt16);
    public static ValueConverter<int> Int32Converter => new(Convert.ToInt32);
    public static ValueConverter<long> Int64Converter => new(Convert.ToInt64);
    public static ValueConverter<sbyte> SByteConverter => new(Convert.ToSByte);
    public static ValueConverter<float> SingleConverter => new(Convert.ToSingle);
    public static ValueConverter<string> StringConverter => new(Convert.ToString);
    public static ValueConverter<ushort> UInt16Converter => new(Convert.ToUInt16);
    public static ValueConverter<uint> UInt32Converter => new(Convert.ToUInt32);
    public static ValueConverter<ulong> UInt64Converter => new(Convert.ToUInt64);

    public static object GetDefaultEnumValueConverter<T>()
        where T : struct, Enum
    {
        return new EnumValueConverter<T>();
    }

    public static object GetDefaultMultipleValueConverter<T>()
    {
        object converter = GetDefaultValueConverter<T>();

        return converter is IValueConverter<T> defaultConverter
            ? new MultipleValueConverter<T>(defaultConverter.Convert)
            : throw new DefaultConverterNotFoundException(null, typeof(T));
    }

    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1501:StatementMustNotBeOnSingleLine", Justification = "Reviewed.")]
    public static object GetDefaultValueConverter<T>()
    {
        Type type = typeof(T);

        if (type == typeof(bool)) return BooleanConverter;
        if (type == typeof(byte)) return ByteConverter;
        if (type == typeof(char)) return CharConverter;
        if (type == typeof(DateTime)) return DateTimeConverter;
        if (type == typeof(decimal)) return DecimalConverter;
        if (type == typeof(double)) return DoubleConverter;
        if (type == typeof(short)) return Int16Converter;
        if (type == typeof(int)) return Int32Converter;
        if (type == typeof(long)) return Int64Converter;
        if (type == typeof(sbyte)) return SByteConverter;
        if (type == typeof(float)) return SingleConverter;
        if (type == typeof(string)) return StringConverter;
        if (type == typeof(ushort)) return UInt16Converter;
        if (type == typeof(uint)) return UInt32Converter;
        if (type == typeof(ulong)) return UInt64Converter;

        throw new DefaultConverterNotFoundException(null, type);
    }
}
