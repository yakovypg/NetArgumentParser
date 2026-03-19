using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NetArgumentParser.Generators;
using NetArgumentParser.Options;

namespace NetArgumentParser.Attributes.Extensions;

internal static class PropertyInfoEnumerableExtensions
{
    internal static IEnumerable<IGrouping<OptionGroupAttribute?, PropertyInfo>> GroupOptionAttributes(
        this IEnumerable<PropertyInfo> properties)
    {
        ExtendedArgumentNullException.ThrowIfNull(properties, nameof(properties));
        return properties.GroupBy(t => t.GetCustomAttribute<OptionGroupAttribute>());
    }

    internal static T? FindOptionGroupSingleAttributeWithBestParameters<T>(
        this IEnumerable<PropertyInfo> properties,
        string groupId)
        where T : OptionGroupBaseAttribute
    {
        ExtendedArgumentNullException.ThrowIfNull(properties, nameof(properties));
        ExtendedArgumentNullException.ThrowIfNull(groupId, nameof(groupId));

        List<T> candidates = properties
            .Select(t => t.GetCustomAttribute<T>())
            .Where(t => t is not null && t.Id == groupId)
            .ToList()!;

        return properties.FindOptionGroupAttributeWithBestParameters(candidates);
    }

    internal static T? FindOptionGroupMultipleAttributeWithBestParameters<T>(
        this IEnumerable<PropertyInfo> properties,
        string groupId)
        where T : OptionGroupBaseAttribute
    {
        ExtendedArgumentNullException.ThrowIfNull(properties, nameof(properties));
        ExtendedArgumentNullException.ThrowIfNull(groupId, nameof(groupId));

        List<T> candidates = [.. properties
            .SelectMany(t => t.GetCustomAttributes<T>())
            .Where(t => t is not null && t.Id == groupId)];

        return properties.FindOptionGroupAttributeWithBestParameters(candidates);
    }

    internal static Dictionary<PropertyInfo, ICommonOption> CreateOptions(
        this IEnumerable<PropertyInfo> optionProperties,
        object source)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionProperties, nameof(optionProperties));
        ExtendedArgumentNullException.ThrowIfNull(source, nameof(source));

        return optionProperties.ToDictionary(
            t => t,
            t => t.CreateOption(source) ?? throw new UnsupportedOptionConfigException(null, t));
    }

    private static T? FindOptionGroupAttributeWithBestParameters<T>(
        this IEnumerable<PropertyInfo> properties,
        IEnumerable<T> candidates)
        where T : OptionGroupBaseAttribute
    {
        ExtendedArgumentNullException.ThrowIfNull(properties, nameof(properties));
        ExtendedArgumentNullException.ThrowIfNull(candidates, nameof(candidates));

        candidates = [.. candidates];

        bool isGroupWithInfoExists = candidates.Any(t =>
        {
            return !string.IsNullOrEmpty(t.Header)
                || !string.IsNullOrEmpty(t.Description);
        });

        if (!isGroupWithInfoExists)
            return candidates.FirstOrDefault();

        T? bestGroupByHeader = candidates
            .FirstOrDefault(t => !string.IsNullOrEmpty(t.Header));

        return bestGroupByHeader ?? candidates
            .FirstOrDefault(t => !string.IsNullOrEmpty(t.Description));
    }
}
