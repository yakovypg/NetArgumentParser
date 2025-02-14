using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Configuration;
using NetArgumentParser.Converters;
using NetArgumentParser.Extensions;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Utils.Comparers;

namespace NetArgumentParser.Options;

using StringEnumerableComparer = System.Func<
    System.Collections.Generic.IEnumerable<string>,
    System.Collections.Generic.IEnumerable<string>,
    System.Collections.Generic.IEqualityComparer<string>,
    bool>;

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
        bool ignoreCaseInChoices = false,
        bool ignoreOrderInChoices = false,
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
            ignoreCaseInChoices,
            aliases,
            choices,
            defaultValue,
            valueRestriction,
            afterValueParsingAction,
            contextCapture ?? new ZeroOrMoreContextCapture())
    {
        IgnoreOrderInChoices = ignoreOrderInChoices;
    }

    public bool IgnoreOrderInChoices { get; }

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

        if (Choices.Count == 0)
            return true;

        if (IgnoreCaseInChoices && typeof(T) == typeof(string))
        {
            IList<string>? castedValue = value as IList<string>;
            IEnumerable<IList<string>> choices = Choices.Cast<IList<string>>();

            StringEnumerableComparer enumerableComparer = IgnoreOrderInChoices
                ? EnumerableExtensions.ScrambledEquals<string>
                : Enumerable.SequenceEqual;

            return choices.Any(t =>
            {
                var stringComparer = new StringEqualityComparer(StringComparison.OrdinalIgnoreCase);
                return enumerableComparer.Invoke(t, castedValue!, stringComparer);
            });
        }

        Func<IList<T>, IList<T>, bool> comparer = IgnoreOrderInChoices
            ? EnumerableExtensions.ScrambledEquals
            : Enumerable.SequenceEqual;

        return Choices.Any(t => comparer.Invoke(t, value));
    }
}
