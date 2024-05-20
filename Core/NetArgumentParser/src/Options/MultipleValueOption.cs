using System;
using System.Collections.Generic;
using NetArgumentParser.Converters;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Options;

public class MultipleValueOption<T> : ValueOption<IList<T>>, IEquatable<MultipleValueOption<T>>
{   
    public MultipleValueOption(
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        DefaultOptionValue<IList<T>>? defaultValue = null,
        Action<IList<T>>? afterValueParsingAction = null,
        IContextCapture? contextCapture = null)
        
        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            metaVariable ?? throw new ArgumentNullException(nameof(metaVariable)),
            isRequired,
            defaultValue,
            afterValueParsingAction,
            contextCapture ?? new ZeroOrMoreContextCapture())
    {
    }

    public bool Equals(MultipleValueOption<T>? other)
    {
        return other is not null && base.Equals(other);
    }

    public override string GetShortExample()
    {
        string prefferedName = GetPrefferedNameWithPrefix();
        string contextDescription = ContextCapture.GetDescription(MetaVariable);

        return $"{prefferedName} {contextDescription}";
    }

    public override string GetLongExample()
    {
        string value = ContextCapture.GetDescription(MetaVariable);

        return !string.IsNullOrEmpty(LongName) && ! string.IsNullOrEmpty(ShortName)
            ? $"{_longNamePrefix}{LongName} {value}, {_shortNamePrefix}{ShortName} {value}"
            : GetShortExample();
    }

    protected override void ParseValue(IValueConverter<IList<T>> converter, params string[] value)
    {
        ArgumentNullException.ThrowIfNull(converter, nameof(converter));
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        if (value.Length == 0)
            throw new OptionValueNotSpecifiedException(null, ToString());
        
        if (ContextCapture.MinNumberOfItemsToCapture > value.Length)
            throw new OptionValueNotRecognizedException(null, value);

        var collection = new List<T>();

        foreach (string item in value)
        {
            IList<T> parsedValue = converter.Convert(item);
            collection.AddRange(parsedValue);
        }

        OnValueParsed(new OptionValueEventArgs<IList<T>>(collection));
    }

    protected override IValueConverter<IList<T>> GetDefaultConverter()
    {
        object converter = ValueConverters.GetDefaultMultipleValueConverter<T>();

        return converter is MultipleValueConverter<T> defaultConverter
            ? defaultConverter
            : throw new DefaultConverterNotFoundException(null, typeof(T));
    }
}
