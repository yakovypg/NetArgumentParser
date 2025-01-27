using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Extensions;

internal static class DictionaryExtensions
{
    internal static Dictionary<TKey, TValue> Combine<TKey, TValue>(
        this IEnumerable<IEnumerable<KeyValuePair<TKey, TValue>>> dictionaries)
        where TKey : notnull
    {
        ExtendedArgumentNullException.ThrowIfNull(dictionaries, nameof(dictionaries));

        return dictionaries
            .SelectMany(t => t)
            .ToDictionary(t => t.Key, t => t.Value);
    }
}
