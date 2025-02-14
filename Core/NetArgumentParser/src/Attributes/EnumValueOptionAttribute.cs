using System;
using System.Reflection;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;

namespace NetArgumentParser.Attributes;

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = true)
]
public class EnumValueOptionAttribute<T> : ValueOptionAttribute<T>
    where T : struct, Enum
{
#pragma warning disable CA1019 // Define accessors for attribute arguments
    public EnumValueOptionAttribute(
        T defaultValue,
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        bool ignoreCaseInChoices = false,
        bool useDefaultChoices = true,
        string[]? aliases = null,
        T[]? choices = null)
        : base(
            defaultValue,
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            metaVariable,
            isRequired,
            isHidden,
            isFinal,
            ignoreCaseInChoices,
            aliases,
            choices)
    {
        UseDefaultChoices = useDefaultChoices;
    }
#pragma warning restore CA1019 // Define accessors for attribute arguments

    public EnumValueOptionAttribute(
        string longName,
        string shortName = "",
        string description = "",
        string metaVariable = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        bool ignoreCaseInChoices = false,
        bool useDefaultChoices = true,
        string[]? aliases = null,
        T[]? choices = null)
        : base(
            choices,
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
        UseDefaultChoices = useDefaultChoices;
    }

    public bool UseDefaultChoices { get; }

    public override ICommonOption CreateOption(object source, PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(source, nameof(source));
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        if (!CanCreateOption(source, propertyInfo))
            throw new CannotCreateOptionException(null, propertyInfo);

        return new EnumValueOption<T>(
            LongName,
            ShortName,
            Description,
            MetaVariable,
            IsRequired,
            IsHidden,
            IsFinal,
            IgnoreCaseInChoices,
            UseDefaultChoices,
            Aliases,
            Choices,
            DefaultValue,
            valueRestriction: null,
            t => propertyInfo.SetValue(source, t));
    }
}
