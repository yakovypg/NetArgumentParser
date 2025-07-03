using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetArgumentParser.Attributes.Extensions;
using NetArgumentParser.Options;

namespace NetArgumentParser.Attributes;

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = true)
]
public abstract class CommonOptionAttribute : Attribute, IReflectionBasedCommonOptionCreator
{
    protected CommonOptionAttribute(
        string longName,
        string shortName = "",
        string description = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        string[]? aliases = null)
    {
        ExtendedArgumentNullException.ThrowIfNull(longName, nameof(longName));
        ExtendedArgumentNullException.ThrowIfNull(shortName, nameof(shortName));
        ExtendedArgumentNullException.ThrowIfNull(description, nameof(description));

        LongName = longName;
        ShortName = shortName;
        Description = description;
        IsRequired = isRequired;
        IsHidden = isHidden;
        IsFinal = isFinal;
        Aliases = aliases;
    }

    public string LongName { get; }
    public string ShortName { get; }
    public string Description { get; }
    public bool IsRequired { get; }
    public bool IsHidden { get; }
    public bool IsFinal { get; }
    public IEnumerable<string>? Aliases { get; }

    protected abstract IReadOnlyList<Type> ValidPropertyTypes { get; }

    public virtual bool CanCreateOption(object source, PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(source, nameof(source));
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        Type propertyType = propertyInfo.PropertyType;
        Type? propertyTypeWithoutNullable = propertyType.RemoveNullable();

        bool isPropertyTypeWithoutNullableValid = propertyTypeWithoutNullable is not null
            && ValidPropertyTypes.Contains(propertyTypeWithoutNullable);

        bool isPropertyTypeValid = ValidPropertyTypes.Contains(propertyType)
            || isPropertyTypeWithoutNullableValid;

        return propertyInfo.CanRead
            && propertyInfo.CanWrite
            && isPropertyTypeValid;
    }

    public abstract ICommonOption CreateOption(object source, PropertyInfo propertyInfo);
}
