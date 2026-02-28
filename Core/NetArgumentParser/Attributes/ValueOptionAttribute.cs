using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Configuration;

namespace NetArgumentParser.Attributes;

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = true)
]
public class ValueOptionAttribute<T> : CommonOptionAttribute
{
#pragma warning disable CA1019 // Define accessors for attribute arguments
    public ValueOptionAttribute(
        T defaultValue,
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        bool ignoreCaseInChoices = false,
        string[]? aliases = null,
        T[]? choices = null,
        string[]? beforeParseChoices = null,
        bool addChoicesToDescription = false,
        bool addBeforeParseChoicesToDescription = false,
        bool addDefaultValueToDescription = false,
        string? valueRestriction = null)
        : this(
            choices,
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            metaVariable,
            isRequired,
            isHidden,
            isFinal,
            ignoreCaseInChoices,
            aliases,
            beforeParseChoices,
            addChoicesToDescription,
            addBeforeParseChoicesToDescription,
            addDefaultValueToDescription,
            valueRestriction)
    {
        DefaultValue = new DefaultOptionValue<T>(defaultValue);
    }
#pragma warning restore CA1019 // Define accessors for attribute arguments

    public ValueOptionAttribute(
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        bool ignoreCaseInChoices = false,
        string[]? aliases = null,
        string[]? beforeParseChoices = null,
        bool addChoicesToDescription = false,
        bool addBeforeParseChoicesToDescription = false,
        bool addDefaultValueToDescription = false,
        string? valueRestriction = null)
        : this(
            choices: null,
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            metaVariable,
            isRequired,
            isHidden,
            isFinal,
            ignoreCaseInChoices,
            aliases,
            beforeParseChoices,
            addChoicesToDescription,
            addBeforeParseChoicesToDescription,
            addDefaultValueToDescription,
            valueRestriction)
    {
    }

    public ValueOptionAttribute(
        T[]? choices,
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        bool ignoreCaseInChoices = false,
        string[]? aliases = null,
        string[]? beforeParseChoices = null,
        bool addChoicesToDescription = false,
        bool addBeforeParseChoicesToDescription = false,
        bool addDefaultValueToDescription = false,
        string? valueRestriction = null)
        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            isRequired,
            isHidden,
            isFinal,
            aliases)
    {
        ExtendedArgumentNullException.ThrowIfNull(metaVariable, nameof(metaVariable));

        MetaVariable = metaVariable;
        IgnoreCaseInChoices = ignoreCaseInChoices;
        AddChoicesToDescription = addChoicesToDescription;
        AddBeforeParseChoicesToDescription = addBeforeParseChoicesToDescription;
        AddDefaultValueToDescription = addDefaultValueToDescription;

        Choices = choices;
        BeforeParseChoices = beforeParseChoices;
        ValueRestriction = CreateValueRestriction(valueRestriction);
    }

    public string MetaVariable { get; }
    public bool IgnoreCaseInChoices { get; }
    public bool AddChoicesToDescription { get; }
    public bool AddBeforeParseChoicesToDescription { get; }
    public bool AddDefaultValueToDescription { get; }

    public IEnumerable<T>? Choices { get; }
    public IEnumerable<string>? BeforeParseChoices { get; }
    public OptionValueRestriction<T>? ValueRestriction { get; protected set; }
    public DefaultOptionValue<T>? DefaultValue { get; }

    protected override IReadOnlyList<Type> ValidPropertyTypes => [typeof(T)];

    public override ICommonOption CreateOption(object source, PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(source, nameof(source));
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        if (!CanCreateOption(source, propertyInfo))
            throw new CannotCreateOptionException(null, propertyInfo);

        var valueOption = new ValueOption<T>(
            LongName,
            ShortName,
            Description,
            MetaVariable,
            IsRequired,
            IsHidden,
            IsFinal,
            IgnoreCaseInChoices,
            Aliases,
            Choices,
            BeforeParseChoices,
            DefaultValue,
            ValueRestriction,
            t => propertyInfo.SetValue(source, t));

        AddDefaultValueToDescriptionIfNeeded(valueOption);
        AddChoicesToDescriptionIfNeeded(valueOption);

        return valueOption;
    }

    private static OptionValueRestriction<T>? CreateValueRestriction(string? data)
    {
        return data is not null && !string.IsNullOrWhiteSpace(data)
            ? OptionValueRestrictionParser.Parse<T>(data, true)
            : null;
    }

    private void AddDefaultValueToDescriptionIfNeeded(ValueOption<T> valueOption)
    {
        ExtendedArgumentNullException.ThrowIfNull(valueOption, nameof(valueOption));

        if (AddDefaultValueToDescription)
            valueOption.AddDefaultValueToDescription();
    }

    private void AddChoicesToDescriptionIfNeeded(ValueOption<T> valueOption)
    {
        ExtendedArgumentNullException.ThrowIfNull(valueOption, nameof(valueOption));

        if (AddBeforeParseChoicesToDescription && BeforeParseChoices is not null && BeforeParseChoices.Any())
            valueOption.AddBeforeParseChoicesToDescription();
        else if (AddChoicesToDescription && Choices is not null && Choices.Any())
            valueOption.AddChoicesToDescription();
    }
}
