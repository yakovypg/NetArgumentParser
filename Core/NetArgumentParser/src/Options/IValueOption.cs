using NetArgumentParser.Converters;

namespace NetArgumentParser.Options;

public interface IValueOption<T> : ICommonOption
{
    bool HasDefaultValue { get; }
    bool HasConverter { get; }

    string MetaVariable { get; }

    DefaultOptionValue<T>? DefaultValue { get; }
    IValueConverter<T>? Converter { get; set; }

    void HandleDefaultValue();
    bool TrySetConverter(IValueConverter converter);
}
