using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using NetArgumentParser.Attributes.Extensions;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;

namespace NetArgumentParser.Attributes;

[AttributeUsage(
    AttributeTargets.Property,
    AllowMultiple = false,
    Inherited = true)
]
public class CounterOptionAttribute : FlagOptionAttribute
{
    public CounterOptionAttribute(
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

    protected override IReadOnlyList<Type> ValidPropertyTypes =>
    [
        typeof(BigInteger),
        typeof(sbyte),
        typeof(byte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal)
    ];

    public override ICommonOption CreateOption(object source, PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(source, nameof(source));
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        if (!CanCreateOption(source, propertyInfo))
            throw new CannotCreateOptionException(null, propertyInfo);

        static void IncreaseCounter(object source, PropertyInfo propertyInfo)
        {
            Type propertyTypeWithoutNullable = propertyInfo.PropertyType.RemoveNullable();

            dynamic? propertyValue = propertyInfo.GetValue(source);
            dynamic defaultValue = propertyTypeWithoutNullable.GetValueTypeDefaultValue();
            dynamic increasedPropertyValue = (propertyValue ?? defaultValue) + 1;

            propertyInfo.SetValue(source, increasedPropertyValue);
        }

        return new CounterOption(
            LongName,
            ShortName,
            Description,
            IsRequired,
            IsHidden,
            IsFinal,
            Aliases,
            () => IncreaseCounter(source, propertyInfo));
    }
}
