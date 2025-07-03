using System;
using System.Collections.Generic;
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
        string[]? beforeParseChoices = null)
        : this(
            choices,
            beforeParseChoices,
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            metaVariable,
            isRequired,
            isHidden,
            isFinal,
            ignoreCaseInChoices,
            aliases)
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
        string[]? aliases = null)
        : this(
            choices: null,
            beforeParseChoices: null,
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            metaVariable,
            isRequired,
            isHidden,
            isFinal,
            ignoreCaseInChoices,
            aliases)
    {
    }

    public ValueOptionAttribute(
        T[]? choices,
        string[]? beforeParseChoices,
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        bool ignoreCaseInChoices = false,
        string[]? aliases = null)
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
        Choices = choices;
        BeforeParseChoices = beforeParseChoices;
    }

    public string MetaVariable { get; }
    public bool IgnoreCaseInChoices { get; }

    public IEnumerable<T>? Choices { get; }
    public IEnumerable<string>? BeforeParseChoices { get; }
    public DefaultOptionValue<T>? DefaultValue { get; }

    protected override IReadOnlyList<Type> ValidPropertyTypes => [typeof(T)];

    public override ICommonOption CreateOption(object source, PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(source, nameof(source));
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        if (!CanCreateOption(source, propertyInfo))
            throw new CannotCreateOptionException(null, propertyInfo);

        return new ValueOption<T>(
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
            null,
            t => propertyInfo.SetValue(source, t));
    }
}
