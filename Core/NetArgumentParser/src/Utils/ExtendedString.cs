using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Utils;

internal static class ExtendedString
{
    internal static string JoinWithExpand<T>(
        string separator,
        string arraySeparator,
        string arrayPrefix,
        string arrayPostfix,
        IEnumerable<T> items)
    {
        ExtendedArgumentNullException.ThrowIfNull(separator, nameof(separator));
        ExtendedArgumentNullException.ThrowIfNull(arraySeparator, nameof(arraySeparator));
        ExtendedArgumentNullException.ThrowIfNull(arrayPrefix, nameof(arrayPrefix));
        ExtendedArgumentNullException.ThrowIfNull(arrayPostfix, nameof(arrayPostfix));
        ExtendedArgumentNullException.ThrowIfNull(items, nameof(items));

        return JoinItemsWithExpand(
            separator,
            arraySeparator,
            arrayPrefix,
            arrayPostfix,
            [.. items!]);
    }

    internal static string JoinItemsWithExpand(
        string separator,
        string arraySeparator,
        string arrayPrefix,
        string arrayPostfix,
        params object[] items)
    {
        ExtendedArgumentNullException.ThrowIfNull(separator, nameof(separator));
        ExtendedArgumentNullException.ThrowIfNull(arraySeparator, nameof(arraySeparator));
        ExtendedArgumentNullException.ThrowIfNull(arrayPrefix, nameof(arrayPrefix));
        ExtendedArgumentNullException.ThrowIfNull(arrayPostfix, nameof(arrayPostfix));
        ExtendedArgumentNullException.ThrowIfNull(items, nameof(items));

        var parts = new List<string>();
        bool hasEnumerablePart = false;

        foreach (object item in items)
        {
            if (item is not string && item is IEnumerable enumerable)
            {
                hasEnumerablePart = true;

                object[] partItems = enumerable
                    .Cast<object>()
                    .ToArray();

                string part = JoinWithExpand(
                    separator,
                    arraySeparator,
                    arrayPrefix,
                    arrayPostfix,
                    partItems);

                string stylizedPart = $"{arrayPrefix}{part}{arrayPostfix}";
                parts.Add(stylizedPart);
            }
            else
            {
                string part = item.ToString() ?? string.Empty;
                parts.Add(part);
            }
        }

        return string.Join(
            hasEnumerablePart ? arraySeparator : separator,
            parts);
    }
}
