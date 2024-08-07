using System;

namespace NetArgumentParser.Converters;

#pragma warning disable SA1402

public interface IValueConverter
{
    Type ConversionType { get; }
    object? ConvertToType(string value);
}

public interface IValueConverter<T> : IValueConverter
{
    T Convert(string value);
}

#pragma warning restore SA1402
