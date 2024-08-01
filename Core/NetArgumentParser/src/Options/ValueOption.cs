using System;
using System.Linq;
using NetArgumentParser.Configuration;
using NetArgumentParser.Converters;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Options;

public class ValueOption<T> : CommonOption, IValueOption<T>, IEquatable<ValueOption<T>>
{
    public ValueOption(
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        DefaultOptionValue<T>? defaultValue = null,
        Action<T>? afterValueParsingAction = null)

        : this(
            longName,
            shortName,
            description,
            metaVariable,
            isRequired,
            defaultValue,
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
        DefaultOptionValue<T>? defaultValue = null,
        Action<T>? afterValueParsingAction = null,
        IContextCapture? contextCapture = null)
        
        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            isRequired,
            contextCapture ?? new FixedContextCapture(1))
    {
        ArgumentNullException.ThrowIfNull(metaVariable, nameof(metaVariable));
        
        MetaVariable = string.IsNullOrWhiteSpace(metaVariable)
            ? GetDefaultMetaVariable()
            : metaVariable;

        DefaultValue = defaultValue;

        if (afterValueParsingAction is not null)
            ValueParsed += (sender, e) => afterValueParsingAction.Invoke(e.Value);
    }

    public event EventHandler<OptionValueEventArgs<T>>? ValueParsed;

    public string MetaVariable { get; }

    public DefaultOptionValue<T>? DefaultValue { get; }
    public IValueConverter<T>? Converter { get; set; }

    public bool HasDefaultValue => DefaultValue is not null;
    public bool HasConverter => Converter is not null;

    public bool Equals(ValueOption<T>? other)
    {
        return other is not null
            && base.Equals(other)
            && HasDefaultValue == other.HasDefaultValue
            && DefaultValue == other.DefaultValue;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ValueOption<T>);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            base.GetHashCode(),
            HasDefaultValue,
            DefaultValue);
    }

    public bool TrySetConverter(IValueConverter converter)
    {
        ArgumentNullException.ThrowIfNull(converter, nameof(converter));

        if (converter is IValueConverter<T> valueConverter)
        {
            Converter = valueConverter;
            return true;
        }

        return false;
    }

    public virtual void HandleDefaultValue()
    {
        if (IsHandled)
            throw new OptionAlreadyHandledException(null, this);
        
        if (DefaultValue is null)
            throw new NullReferenceException("Default value is not specified.");
        
        IsHandled = true;
        OnValueParsed(new OptionValueEventArgs<T>(DefaultValue.Value));
    }

    public override string GetShortExample()
    {
        string prefferedName = GetPrefferedNameWithPrefix();
        return $"{prefferedName} {MetaVariable}";
    }

    public override string GetLongExample()
    {
        string value = MetaVariable;

        string longExample =
            $"{SpecialCharacters.LongNamedOptionPrefix}{LongName} {value}, " +
            $"{SpecialCharacters.ShortNamedOptionPrefix}{ShortName} {value}";

        return !string.IsNullOrEmpty(LongName) && ! string.IsNullOrEmpty(ShortName)
            ? longExample
            : GetShortExample();
    }

    protected override void HandleValue(params string[] value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        
        IValueConverter<T> converter = Converter ?? GetDefaultConverter();
        ParseValue(converter, value);
    }

    protected virtual void ParseValue(IValueConverter<T> converter, params string[] value)
    {
        ArgumentNullException.ThrowIfNull(converter, nameof(converter));
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        
        if (value.Length == 0)
            throw new OptionValueNotSpecifiedException(null, ToString());

        T parsedValue = converter.Convert(value[0]);
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
            .Select(t => char.IsLetter(t) ? char.ToUpper(t) : '_')
            .ToArray();

        return new string(transformedName);
    }

    protected virtual void OnValueParsed(OptionValueEventArgs<T> e)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));
        ValueParsed?.Invoke(this, e);
    }
}
