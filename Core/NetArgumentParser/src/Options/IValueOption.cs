using NetArgumentParser.Converters;
using System.Collections.Generic;

namespace NetArgumentParser.Options;

public interface IValueOption<T> : ICommonOption
{
    bool HasDefaultValue { get; }
    bool HasConverter { get; }

    string MetaVariable { get; }

    IReadOnlyCollection<T> Choices { get; }
    DefaultOptionValue<T>? DefaultValue { get; }
    OptionValueRestriction<T>? ValueRestriction { get; }
    IValueConverter<T>? Converter { get; set; }

    void HandleDefaultValue();
    bool TrySetConverter(IValueConverter converter);
}
