using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Configuration;
using NetArgumentParser.Converters;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Options;

public class MultipleValueOption<T> : ValueOption<IList<T>>
{
    public MultipleValueOption(
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        IEnumerable<string>? aliases = null,
        IEnumerable<IList<T>>? choices = null,
        DefaultOptionValue<IList<T>>? defaultValue = null,
        OptionValueRestriction<IList<T>>? valueRestriction = null,
        Action<IList<T>>? afterValueParsingAction = null,
        IContextCapture? contextCapture = null)

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
            contextCapture ?? new ZeroOrMoreContextCapture())
    {
    }

    public override string GetShortExample()
    {
        string extendedMetavariable = GetExtendedMetavariable();
        string prefferedName = GetPrefferedNameWithPrefix();
        string contextDescription = ContextCapture.GetDescription(extendedMetavariable);

        return $"{prefferedName} {contextDescription}";
    }

    public override string GetLongExample()
    {
        string extendedMetavariable = GetExtendedMetavariable();
        string contextDescription = ContextCapture.GetDescription(extendedMetavariable);

        string longExample =
            $"{SpecialCharacters.LongNamedOptionPrefix}{LongName} {contextDescription}, " +
            $"{SpecialCharacters.ShortNamedOptionPrefix}{ShortName} {contextDescription}";

        return !string.IsNullOrEmpty(LongName) && !string.IsNullOrEmpty(ShortName)
            ? longExample
            : GetShortExample();
    }

    protected override void ParseValue(IValueConverter<IList<T>> converter, params string[] value)
    {
        ExtendedArgumentNullException.ThrowIfNull(converter, nameof(converter));
        ExtendedArgumentNullException.ThrowIfNull(value, nameof(value));

        if (value.Length == 0)
            throw new OptionValueNotSpecifiedException(null, ToString());

        if (ContextCapture.MinNumberOfItemsToCapture > value.Length)
            throw new OptionValueNotRecognizedException(null, value);

        var parsedCollection = new List<T>();

        foreach (string item in value)
        {
            IList<T> parsedValue = converter.Convert(item);
            parsedCollection.AddRange(parsedValue);
        }

        VerifyValueIsAllowed(parsedCollection, value);
        OnValueParsed(new OptionValueEventArgs<IList<T>>(parsedCollection));
    }

    protected override IValueConverter<IList<T>> GetDefaultConverter()
    {
        object converter = ValueConverters.GetDefaultMultipleValueConverter<T>();

        return converter is MultipleValueConverter<T> defaultConverter
            ? defaultConverter
            : throw new DefaultConverterNotFoundException(null, typeof(T));
    }

    protected override string[] GetAllowedValues()
    {
        return Choices
            .Select(t => $"[{string.Join(",", t)}]")
            .ToArray();
    }

    protected override bool IsValueSatisfyChoices(IList<T> value)
    {
        ExtendedArgumentNullException.ThrowIfNull(value, nameof(value));
        return Choices.Count == 0 || Choices.Any(t => t.SequenceEqual(value));
    }
}
