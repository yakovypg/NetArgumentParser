using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Tests.Extensions;

internal static class EnumerableExtensions
{
    internal static bool ScrambledEquals<T>(this IEnumerable<T> enumerable, IEnumerable<T> other)
    {
        ExtendedArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        ExtendedArgumentNullException.ThrowIfNull(other, nameof(other));

        return Enumerable.SequenceEqual(
            enumerable.OrderBy(t => t),
            other.OrderBy(t => t));
    }
}
