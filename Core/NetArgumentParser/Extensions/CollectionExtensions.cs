using System.Collections.Generic;

namespace NetArgumentParser.Extensions;

internal static class CollectionExtensions
{
    internal static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        ExtendedArgumentNullException.ThrowIfNull(collection, nameof(collection));
        ExtendedArgumentNullException.ThrowIfNull(items, nameof(items));

        foreach (T item in items)
        {
            collection.Add(item);
        }
    }
}
