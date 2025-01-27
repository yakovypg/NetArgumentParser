using System;
using System.Collections.Generic;
using System.Reflection;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;

namespace NetArgumentParser.Attributes;

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = true)
]
public class FlagOptionAttribute : CommonOptionAttribute
{
    public FlagOptionAttribute(
        string longName,
        string shortName = "",
        string description = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
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
    }

    protected override IReadOnlyList<Type> ValidPropertyTypes => [typeof(bool)];

    public override ICommonOption CreateOption(object source, PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(source, nameof(source));
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        if (!CanCreateOption(source, propertyInfo))
            throw new CannotCreateOptionException(null, propertyInfo);

        return new FlagOption(
            LongName,
            ShortName,
            Description,
            IsRequired,
            IsHidden,
            IsFinal,
            Aliases,
            () => propertyInfo.SetValue(source, true));
    }
}
