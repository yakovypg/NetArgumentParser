using System;

namespace NetArgumentParser.Converters;

public class ValueConverter<T> : IValueConverter<T>
{
    private readonly Func<string, T> _converter;

    public ValueConverter(Func<string, T> converter)
    {
        ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        _converter = converter;
        ConversionType = typeof(T);
    }

    public Type ConversionType { get; }

    public virtual T Convert(string value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        return _converter.Invoke(value);
    }

    public object? ConvertToType(string value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        return Convert(value);
    }
}
