using System;
using System.Collections.Generic;
using NetArgumentParser.Converters;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Options;

public class EnumValueOption<T> : ValueOption<T>
    where T : struct, Enum
{
    public EnumValueOption(
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        IEnumerable<string>? aliases = null,
        IEnumerable<T>? choices = null,
        DefaultOptionValue<T>? defaultValue = null,
        OptionValueRestriction<T>? valueRestriction = null,
        Action<T>? afterValueParsingAction = null)

        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            metaVariable ?? throw new ArgumentNullException(nameof(metaVariable)),
            isRequired,
            isHidden,
            isFinal,
            aliases,
            choices,
            defaultValue,
            valueRestriction,
            afterValueParsingAction,
            new FixedContextCapture(1))
    {
    }

    protected override IValueConverter<T> GetDefaultConverter()
    {
        object converterObject = ValueConverters.GetDefaultEnumValueConverter<T>();

        return converterObject is IValueConverter<T> defaultConverter
            ? defaultConverter
            : throw new DefaultConverterNotFoundException(null, typeof(T));
    }
}
