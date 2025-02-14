using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NetArgumentParser.Configuration;
using NetArgumentParser.Converters;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Utils;
using NetArgumentParser.Utils.Comparers;

namespace NetArgumentParser.Options;

public class ValueOption<T> : CommonOption, IValueOption<T>
{
    private List<T> _choices;
    private bool _areChoicesAddedToDescription;

    public ValueOption(
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        bool ignoreCaseInChoices = false,
        IEnumerable<string>? aliases = null,
        IEnumerable<T>? choices = null,
        DefaultOptionValue<T>? defaultValue = null,
        OptionValueRestriction<T>? valueRestriction = null,
        Action<T>? afterValueParsingAction = null)

        : this(
            longName,
            shortName,
            description,
            metaVariable,
            isRequired,
            isHidden,
            isFinal,
            ignoreCaseInChoices,
            aliases,
            choices,
            defaultValue,
            valueRestriction,
            afterValueParsingAction,
            new FixedContextCapture(1))
    {
    }

    protected ValueOption(
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        bool ignoreCaseInChoices = false,
        IEnumerable<string>? aliases = null,
        IEnumerable<T>? choices = null,
        DefaultOptionValue<T>? defaultValue = null,
        OptionValueRestriction<T>? valueRestriction = null,
        Action<T>? afterValueParsingAction = null,
        IContextCapture? contextCapture = null)

        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            isRequired,
            isHidden,
            isFinal,
            aliases,
            contextCapture ?? new FixedContextCapture(1))
    {
        ExtendedArgumentNullException.ThrowIfNull(metaVariable, nameof(metaVariable));

        _choices = new List<T>(choices ?? []);

        if (defaultValue is not null
            && _choices.Count > 0
            && !_choices.Contains(defaultValue.Value))
        {
            throw new ArgumentException("Choices don't contain default value.", nameof(defaultValue));
        }

        if (valueRestriction is not null)
        {
            if (defaultValue is not null && !valueRestriction.IsValueAllowed(defaultValue.Value))
                throw new OptionValueNotSatisfyRestrictionException(null, [$"{defaultValue.Value}"]);

            if (_choices.Count > 0 && _choices.Any(t => !valueRestriction.IsValueAllowed(t)))
            {
                throw new OptionValueNotSatisfyRestrictionException(
                    null, [$"{_choices.First(t => !valueRestriction.IsValueAllowed(t))}"]);
            }
        }

        MetaVariable = string.IsNullOrWhiteSpace(metaVariable)
            ? GetDefaultMetaVariable()
            : metaVariable;

        IgnoreCaseInChoices = ignoreCaseInChoices;
        DefaultValue = defaultValue;
        ValueRestriction = valueRestriction;

        if (afterValueParsingAction is not null)
            ValueParsed += (sender, e) => afterValueParsingAction.Invoke(e.Value);
    }

    public event EventHandler<OptionValueEventArgs<T>>? ValueParsed;

    public string MetaVariable { get; }
    public bool IgnoreCaseInChoices { get; }

    public DefaultOptionValue<T>? DefaultValue { get; set; }
    public OptionValueRestriction<T>? ValueRestriction { get; set; }
    public IValueConverter<T>? Converter { get; set; }

    public IReadOnlyCollection<T> Choices => _choices;
    public bool HasDefaultValue => DefaultValue is not null;
    public bool HasConverter => Converter is not null;

    public void ChangeChoices(IEnumerable<T>? choices)
    {
        _choices = new List<T>(choices ?? []);
    }

    public bool TrySetConverter(IValueConverter converter)
    {
        ExtendedArgumentNullException.ThrowIfNull(converter, nameof(converter));

        if (converter is IValueConverter<T> valueConverter)
        {
            Converter = valueConverter;
            return true;
        }

        return false;
    }

    public void AddChoicesToDescription(
        string separator = ", ",
        string prefix = " (",
        string postfix = ")",
        string arraySeparator = "; ",
        string arrayPrefix = "[",
        string arrayPostfix = "]")
    {
        ExtendedArgumentNullException.ThrowIfNull(separator, nameof(separator));
        ExtendedArgumentNullException.ThrowIfNull(prefix, nameof(prefix));
        ExtendedArgumentNullException.ThrowIfNull(postfix, nameof(postfix));
        ExtendedArgumentNullException.ThrowIfNull(arraySeparator, nameof(arraySeparator));
        ExtendedArgumentNullException.ThrowIfNull(arrayPrefix, nameof(arrayPrefix));
        ExtendedArgumentNullException.ThrowIfNull(arrayPostfix, nameof(arrayPostfix));

        if (_areChoicesAddedToDescription)
            return;

        string choices = ExtendedString.JoinWithExpand(
            separator,
            arraySeparator,
            arrayPrefix,
            arrayPostfix,
            _choices);

        Description += $"{prefix}{choices}{postfix}";

        _areChoicesAddedToDescription = true;
    }

    public virtual void HandleDefaultValue()
    {
        if (IsHandled)
            throw new OptionAlreadyHandledException(null, this);

        if (DefaultValue is null)
            throw new DefaultValueNotSpecifiedException(null, this);

        IsHandled = true;
        OnValueParsed(new OptionValueEventArgs<T>(DefaultValue.Value));
    }

    public override string GetShortExample()
    {
        string extendedMetavariable = GetExtendedMetavariable();
        string prefferedName = GetPrefferedNameWithPrefix();

        return $"{prefferedName} {extendedMetavariable}";
    }

    public override string GetLongExample()
    {
        string extendedMetavariable = GetExtendedMetavariable();

        string longExample =
            $"{SpecialCharacters.LongNamedOptionPrefix}{LongName} {extendedMetavariable}, " +
            $"{SpecialCharacters.ShortNamedOptionPrefix}{ShortName} {extendedMetavariable}";

        return !string.IsNullOrEmpty(LongName) && !string.IsNullOrEmpty(ShortName)
            ? longExample
            : GetShortExample();
    }

    protected override void HandleValue(params string[] value)
    {
        ExtendedArgumentNullException.ThrowIfNull(value, nameof(value));

        IValueConverter<T> converter = Converter ?? GetDefaultConverter();
        ParseValue(converter, value);
    }

    protected virtual void ParseValue(IValueConverter<T> converter, params string[] value)
    {
        ExtendedArgumentNullException.ThrowIfNull(converter, nameof(converter));
        ExtendedArgumentNullException.ThrowIfNull(value, nameof(value));

        if (value.Length == 0)
            throw new OptionValueNotSpecifiedException(null, ToString());

        T parsedValue = converter.Convert(value[0]);

        VerifyValueIsAllowed(parsedValue, value);
        OnValueParsed(new OptionValueEventArgs<T>(parsedValue));
    }

    protected virtual IValueConverter<T> GetDefaultConverter()
    {
        object converter = ValueConverters.GetDefaultValueConverter<T>();

        return converter is IValueConverter<T> defaultConverter
            ? defaultConverter
            : throw new DefaultConverterNotFoundException(null, typeof(T));
    }

    protected string GetDefaultMetaVariable()
    {
        string prefferedName = GetPrefferedName();

        char[] transformedName = prefferedName
            .Select(t => char.IsLetter(t) ? char.ToUpper(t, CultureInfo.CurrentCulture) : '_')
            .ToArray();

        return new string(transformedName);
    }

    protected virtual string[] GetAllowedValues()
    {
        return _choices
            .Select(t => t?.ToString() ?? string.Empty)
            .ToArray();
    }

    protected virtual bool IsValueSatisfyChoices(T value)
    {
        ExtendedArgumentNullException.ThrowIfNull(value, nameof(value));

        if (_choices.Count == 0)
            return true;

        if (IgnoreCaseInChoices && typeof(T) == typeof(string))
        {
            IEnumerable<string> choices = _choices.Cast<string>();
            StringEqualityComparer comparer = new(StringComparison.OrdinalIgnoreCase);

            return choices.Contains(value as string, comparer);
        }

        return _choices.Contains(value);
    }

    protected virtual bool IsValueSatisfyRestriction(T value)
    {
        ExtendedArgumentNullException.ThrowIfNull(value, nameof(value));
        return ValueRestriction is null || ValueRestriction.IsValueAllowed(value);
    }

    protected virtual void OnValueParsed(OptionValueEventArgs<T> e)
    {
        ExtendedArgumentNullException.ThrowIfNull(e, nameof(e));
        ValueParsed?.Invoke(this, e);
    }

    protected void VerifyValueIsAllowed(T value, string[] valueSource)
    {
        ExtendedArgumentNullException.ThrowIfNull(value, nameof(value));
        ExtendedArgumentNullException.ThrowIfNull(valueSource, nameof(valueSource));

        if (!IsValueSatisfyRestriction(value))
            throw new OptionValueNotSatisfyRestrictionException(null, valueSource);

        if (!IsValueSatisfyChoices(value))
            throw new OptionValueNotSatisfyChoicesException(null, valueSource, GetAllowedValues());
    }

    protected string GetExtendedMetavariable()
    {
        return _choices.Count > 0
            ? "{" + string.Join(",", GetAllowedValues()) + "}"
            : MetaVariable;
    }
}
