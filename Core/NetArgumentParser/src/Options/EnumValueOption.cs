using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Converters;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Options;

public class EnumValueOption<T> : ValueOption<T>
    where T : struct, Enum
{
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable CA2263 // Prefer generic overload when type is known
    public EnumValueOption(
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        bool ignoreCaseInChoices = false,
        bool useDefaultChoices = true,
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
            ignoreCaseInChoices,
            aliases,
            useDefaultChoices && choices is null
                ? (valueRestriction is not null
                    ? ((T[])Enum.GetValues(typeof(T))).Where(t => valueRestriction.IsValueAllowed(t))
                    : (T[])Enum.GetValues(typeof(T)))
                : choices,
            defaultValue,
            valueRestriction,
            afterValueParsingAction,
            new FixedContextCapture(1))
    {
    }
#pragma warning restore CA2263 // Prefer generic overload when type is known
#pragma warning restore SA1118 // Parameter should not span multiple lines

    protected override IValueConverter<T> GetDefaultConverter()
    {
        object converterObject = ValueConverters.GetDefaultEnumValueConverter<T>();

        return converterObject is IValueConverter<T> defaultConverter
            ? defaultConverter
            : throw new DefaultConverterNotFoundException(null, typeof(T));
    }
}
