using System;
using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Extensions;

internal static class StringExtensions
{
#if !NET5_0_OR_GREATER && !NETCOREAPP3_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER
    private const StringComparison _defaultComparisonType = StringComparison.CurrentCulture;

#pragma warning disable IDE0060
    internal static bool Contains(
        this string text,
        char value,
        StringComparison comparisonType = _defaultComparisonType)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));
        return text.Contains($"{value}");
    }
#pragma warning restore IDE0060

    internal static bool StartsWith(
        this string text,
        char value,
        StringComparison comparisonType = _defaultComparisonType)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));
        return text.StartsWith($"{value}", comparisonType);
    }

    internal static int IndexOf(
        this string text,
        char value,
        StringComparison comparisonType = _defaultComparisonType)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));
        return text.IndexOf($"{value}", comparisonType);
    }
#endif

    internal static string RemoveLineBreakFromEnd(this string text)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));
        return RemoveLineBreakFromEnd(text, Environment.NewLine);
    }

    internal static string RemoveLineBreakFromEnd(this string text, string lineBreak)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));

        while (text.EndsWith(lineBreak, StringComparison.CurrentCulture))
        {
            int lineBreakLength = lineBreak.Length;
            text = text.Remove(text.Length - lineBreakLength);
        }

        return text;
    }

    internal static string AddEmptyPostfix(this string text, int requiredLength)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));

        if (text.Length < requiredLength)
        {
            string emptyPostfix = new(' ', requiredLength - text.Length);
            text += emptyPostfix;
        }

        return text;
    }
}
