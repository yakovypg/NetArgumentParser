using System;

namespace NetArgumentParser.Extensions;

internal static class StringExtensions
{
    internal static string RemoveLineBreakFromEnd(this string text)
    {
        return RemoveLineBreakFromEnd(text, Environment.NewLine);
    }

    internal static string RemoveLineBreakFromEnd(this string text, string lineBreak)
    {
        while (text.EndsWith(lineBreak))
        {
            int lineBreakLength = lineBreak.Length;
            text = text.Remove(text.Length - lineBreakLength);
        }

        return text;
    }
}
