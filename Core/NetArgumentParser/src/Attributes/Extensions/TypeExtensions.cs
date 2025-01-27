using System;

namespace NetArgumentParser.Attributes.Extensions;

internal static class TypeExtensions
{
    internal static Type RemoveNullable(this Type type)
    {
        return Nullable.GetUnderlyingType(type) ?? type;
    }

    internal static object? GetDefaultValue(this Type type)
    {
        return type.IsValueType
            ? Activator.CreateInstance(type)
            : null;
    }

    internal static object GetValueTypeDefaultValue(this Type type)
    {
        if (!type.IsValueType)
            throw new ArgumentException($"{type.FullName} is not value type.", nameof(type));

        object? instance = Activator.CreateInstance(type);

        if (instance is null)
        {
            string message = $"{type.FullName} doesn't support creation of its instance.";
            throw new ArgumentException(message, nameof(type));
        }

        return instance;
    }
}
