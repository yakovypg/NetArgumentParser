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
        return properties.GroupBy(t => t.GetCustomAttribute<OptionGroupAttribute>());
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
}
