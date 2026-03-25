using System;
using System.Text;

namespace NetArgumentParser.Extensions;

internal static class StringExtensions
{
    private const StringComparison _defaultComparisonType = StringComparison.Ordinal;

#if !NET5_0_OR_GREATER && !NETCOREAPP3_0_OR_GREATER && !NETSTANDARD2_1_OR_GREATER
    internal static string Replace(
        this string text,
        string oldValue,
        string? newValue,
        StringComparison comparisonType = _defaultComparisonType)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));
        ExtendedArgumentException.ThrowIfNullOrEmpty(oldValue, nameof(oldValue));

        string replacement = newValue ?? string.Empty;

        int currentPosition = 0;
        int matchIndex = text.IndexOf(oldValue, currentPosition, comparisonType);

        if (matchIndex < 0)
            return text;

        var builder = new StringBuilder(text.Length);

        while (matchIndex >= 0)
        {
            _ = builder
                .Append(text, currentPosition, matchIndex - currentPosition)
                .Append(replacement);

            currentPosition = matchIndex + oldValue.Length;
            matchIndex = text.IndexOf(oldValue, currentPosition, comparisonType);
        }

        return builder
            .Append(text, currentPosition, text.Length - currentPosition)
            .ToString();
    }

    internal static bool Contains(
        this string text,
        char value,
        StringComparison comparisonType = _defaultComparisonType)
    {
        return text.Contains($"{value}", comparisonType);
    }

    internal static bool Contains(
        this string text,
        string? value,
        StringComparison comparisonType = _defaultComparisonType)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));

        return value is not null
            ? text.IndexOf(value, comparisonType) >= 0
            : false;
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

    internal static bool StartsWith(
        this string text,
        char value,
        StringComparison comparisonType = _defaultComparisonType)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));
        return text.StartsWith($"{value}", comparisonType);
    }

    internal static bool EndsWith(
        this string text,
        char value,
        StringComparison comparisonType = _defaultComparisonType)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));
        return text.EndsWith($"{value}", comparisonType);
    }

    internal static string RemoveLineBreakFromEnd(this string text)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));
        return RemoveLineBreakFromEnd(text, Environment.NewLine);
    }

    internal static string RemoveLineBreakFromEnd(this string text, string lineBreak)
    {
        ExtendedArgumentNullException.ThrowIfNull(text, nameof(text));

        while (text.EndsWith(lineBreak, StringComparison.Ordinal))
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
