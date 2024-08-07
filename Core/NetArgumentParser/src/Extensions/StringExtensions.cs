using System;

namespace NetArgumentParser.Extensions;

internal static class StringExtensions
{
    internal static string RemoveLineBreakFromEnd(this string text)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));
        return RemoveLineBreakFromEnd(text, Environment.NewLine);
    }

    internal static string RemoveLineBreakFromEnd(this string text, string lineBreak)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));

        while (text.EndsWith(lineBreak, StringComparison.CurrentCulture))
        {
            int lineBreakLength = lineBreak.Length;
            text = text.Remove(text.Length - lineBreakLength);
        }

        return text;
    }

    internal static string AddEmptyPostfix(this string text, int requiredLength)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));

        if (text.Length < requiredLength)
        {
            string emptyPostfix = new(' ', requiredLength - text.Length);
            text += emptyPostfix;
        }

        return text;
    }
}
