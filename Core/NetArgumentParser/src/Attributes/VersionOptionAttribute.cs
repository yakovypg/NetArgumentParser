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
public sealed class VersionOptionAttribute : FlagOptionAttribute
{
    public VersionOptionAttribute()
        : this("version", string.Empty, "show version information", false, []) { }

    public VersionOptionAttribute(
        string longName,
        string shortName = "",
        string description = "",
        bool isHidden = false,
        string[]? aliases = null)
        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            false,
            isHidden,
            true,
            aliases)
    {
    }

    public override ICommonOption CreateOption(object source, PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(source, nameof(source));
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        if (!CanCreateOption(source, propertyInfo))
            throw new CannotCreateOptionException(null, propertyInfo);

        return new VersionOption(
            LongName,
            ShortName,
            Description,
            IsHidden,
            Aliases,
            () => propertyInfo.SetValue(source, true));
    }
}
