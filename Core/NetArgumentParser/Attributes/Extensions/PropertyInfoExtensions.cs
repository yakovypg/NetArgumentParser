using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetArgumentParser.Options;

namespace NetArgumentParser.Attributes.Extensions;

internal static class PropertyInfoExtensions
{
    internal static bool HasSingleAttribute<T>(this PropertyInfo propertyInfo)
        where T : Attribute
    {
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));
        return propertyInfo.GetCustomAttribute<T>() is not null;
    }

    internal static bool HasMultipleAttribute<T>(this PropertyInfo propertyInfo)
        where T : Attribute
    {
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        IEnumerable<T> attributes = propertyInfo.GetCustomAttributes<T>();
        return attributes is not null && attributes.Any();
    }

    internal static bool HasOptionAttribute(this PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));
        return propertyInfo.HasSingleAttribute<CommonOptionAttribute>();
    }

    internal static bool HasOptionGroupAttribute(this PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));
        return propertyInfo.HasSingleAttribute<OptionGroupAttribute>();
    }

    internal static bool HasMutuallyExclusiveOptionGroupAttribute(this PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));
        return propertyInfo.HasMultipleAttribute<MutuallyExclusiveOptionGroupAttribute>();
    }

    internal static bool HasSubcommandAttribute(this PropertyInfo propertyInfo)
    {
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));

        return propertyInfo.CustomAttributes
            .Any(t => t.AttributeType == typeof(SubcommandAttribute));
    }

    internal static ICommonOption? CreateOption(this PropertyInfo propertyInfo, object source)
    {
        ExtendedArgumentNullException.ThrowIfNull(propertyInfo, nameof(propertyInfo));
        ExtendedArgumentNullException.ThrowIfNull(source, nameof(source));

        CommonOptionAttribute? attribute = propertyInfo.GetCustomAttribute<CommonOptionAttribute>();

        return attribute?.CreateOption(source, propertyInfo);
    }
}
