using System.Collections.Generic;
using NetArgumentParser.Converters;
using NetArgumentParser.Options.Configuration;
using NetArgumentParser.Options.Design;

namespace NetArgumentParser.Options;

public interface IValueOption<T> : ICommonOption, IValueOptionDescriptionDesigner
{
    bool HasDefaultValue { get; }
    bool HasConverter { get; }

    string MetaVariable { get; }

    IReadOnlyCollection<T> Choices { get; }
    DefaultOptionValue<T>? DefaultValue { get; set; }
    OptionValueRestriction<T>? ValueRestriction { get; set; }
    IValueConverter<T>? Converter { get; set; }

    void HandleDefaultValue();
    bool TrySetConverter(IValueConverter converter);
}
