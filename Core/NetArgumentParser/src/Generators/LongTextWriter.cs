using System;
using System.Collections.Generic;
using System.Text;

namespace NetArgumentParser.Generators;

public sealed class LongTextWriter
{
    public LongTextWriter(
        StringBuilder textBuilder,
        int leftOffset,
        int charsInOneLine)
    {
        ExtendedArgumentNullException.ThrowIfNull(textBuilder, nameof(textBuilder));
        DefaultExceptions.ThrowIfNegative(leftOffset, nameof(leftOffset));
        DefaultExceptions.ThrowIfNegativeOrZero(charsInOneLine, nameof(charsInOneLine));

        TextBuilder = textBuilder;
        LeftOffset = leftOffset;
        CharsInOneLine = charsInOneLine;
    }

    public StringBuilder TextBuilder { get; }
    public int LeftOffset { get; }
    public int CharsInOneLine { get; }

    public static string GetEmptySpace(int length)
    {
        DefaultExceptions.ThrowIfNegative(length, nameof(length));
        return new(' ', length);
    }

    public void AppendParts(IList<string> parts)
    {
        ExtendedArgumentNullException.ThrowIfNull(parts, nameof(parts));
        AppendParts(parts, CharsInOneLine, true);
    }

    private void AppendParts(
        IList<string> parts,
        int remainingChars,
        bool isFirstPartInLine)
    {
        ExtendedArgumentNullException.ThrowIfNull(parts, nameof(parts));
        DefaultExceptions.ThrowIfNegative(remainingChars, nameof(remainingChars));

        if (parts.Count == 0)
        {
            _ = TextBuilder.AppendLine();
            return;
        }

        if (!isFirstPartInLine && remainingChars > 0)
        {
            _ = TextBuilder.Append(' ');
            remainingChars--;
        }

        AppendFirstPart(parts, ref remainingChars, ref isFirstPartInLine);
        AppendParts(parts, remainingChars, isFirstPartInLine);
    }

    private void AppendFirstPart(
        IList<string> parts,
        ref int remainingChars,
        ref bool isFirstPartInLine)
    {
        ExtendedArgumentNullException.ThrowIfNull(parts, nameof(parts));
        DefaultExceptions.ThrowIfNegative(remainingChars, nameof(remainingChars));

        if (parts.Count == 0)
            return;

        string firstPart = parts[0];

        if (firstPart.Length > remainingChars && firstPart.Length <= CharsInOneLine)
            AppendLineBreak(ref remainingChars, ref isFirstPartInLine);
        else if (firstPart.Length > remainingChars)
            AppendFirstPartWithCut(parts, ref remainingChars, ref isFirstPartInLine);
        else
            AppendFirstPartWithoutCut(parts, ref remainingChars, ref isFirstPartInLine);
    }

    private void AppendFirstPartWithCut(
        IList<string> parts,
        ref int remainingChars,
        ref bool isFirstPartInLine)
    {
        ExtendedArgumentNullException.ThrowIfNull(parts, nameof(parts));
        DefaultExceptions.ThrowIfNegative(remainingChars, nameof(remainingChars));

        if (parts.Count == 0)
            return;

        string firstPart = parts[0];
        string appended = firstPart.Remove(remainingChars);
        string remaining = firstPart.Substring(remainingChars);

        string emptySpace = GetEmptySpace(LeftOffset);
        _ = TextBuilder.AppendLine(appended).Append(emptySpace);

        parts.RemoveAt(0);
        parts.Insert(0, remaining);

        remainingChars = CharsInOneLine;
        isFirstPartInLine = true;
    }

    private void AppendFirstPartWithoutCut(
        IList<string> parts,
        ref int remainingChars,
        ref bool isFirstPartInLine)
    {
        ExtendedArgumentNullException.ThrowIfNull(parts, nameof(parts));
        DefaultExceptions.ThrowIfNegative(remainingChars, nameof(remainingChars));

        if (parts.Count == 0)
            return;

        string firstPart = parts[0];

        _ = TextBuilder.Append(firstPart);
        parts.RemoveAt(0);

        remainingChars -= firstPart.Length;
        isFirstPartInLine = false;
    }

    private void AppendLineBreak(
        ref int remainingChars,
        ref bool isFirstPartInLine)
    {
        DefaultExceptions.ThrowIfNegative(remainingChars, nameof(remainingChars));

        string emptySpace = GetEmptySpace(LeftOffset);
        _ = TextBuilder.AppendLine().Append(emptySpace);

        remainingChars = CharsInOneLine;
        isFirstPartInLine = true;
    }
}
