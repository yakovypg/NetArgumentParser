using System;

namespace NetArgumentParser.Converters;

public interface IValueConverter
{
    Type ConversionType { get; }
    object? ConvertToType(string value);
}

public interface IValueConverter<T> : IValueConverter
{
    T Convert(string value);
}
