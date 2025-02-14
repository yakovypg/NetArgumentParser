using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Utils.Comparers;

namespace NetArgumentParser.Extensions;

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

    internal static bool ScrambledEquals<T>(
        this IEnumerable<string> enumerable,
        IEnumerable<string> other,
        IEqualityComparer<string>? stringComparer = null)
    {
        ExtendedArgumentNullException.ThrowIfNull(enumerable, nameof(enumerable));
        ExtendedArgumentNullException.ThrowIfNull(other, nameof(other));

        stringComparer ??= EqualityComparer<string>.Default;

        return Enumerable.SequenceEqual(
            enumerable.OrderBy(t => t),
            other.OrderBy(t => t),
            stringComparer);
    }
}
