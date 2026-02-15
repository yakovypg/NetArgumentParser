using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace NetArgumentParser.Tests.Models;

internal readonly struct PageFontSize : IEquatable<PageFontSize>
{
    private const string _expectedStringFormat = "Pages:FontSize";

    internal PageFontSize(int pageNumber, double fontSize)
    {
        PageNumber = pageNumber;
        FontSize = fontSize;
    }

    internal double PageNumber { get; }
    internal double FontSize { get; }

    public static PageFontSize Parse(string data)
    {
        ExtendedArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));

        string[] parts = data.Split(':');

        if (parts.Length != 2)
        {
            throw new ArgumentException(
                $"Recieved data has incorrect format. Expected: {_expectedStringFormat}",
                nameof(data));
        }

        int pageNumber = int.Parse(parts[0], CultureInfo.CurrentCulture);
        double fontSize = double.Parse(parts[1], CultureInfo.CurrentCulture);

        return new PageFontSize(pageNumber, fontSize);
    }

    public static PageFontSize[] ParseFromRange(string data)
    {
        ExtendedArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));

        string[] parts = data.Split(':');

        if (parts.Length != 2)
        {
            throw new ArgumentException(
                $"Recieved data has incorrect format. Expected: {_expectedStringFormat}",
                nameof(data));
        }

        IReadOnlyCollection<int> pages;
        string[] pageRangeParts = parts[0].Split('-');

        if (pageRangeParts.Length == 1)
        {
            int value = int.Parse(data, CultureInfo.InvariantCulture);
            pages = [value];
        }
        else if (pageRangeParts.Length == 2)
        {
            int start = int.Parse(pageRangeParts[0], CultureInfo.InvariantCulture);
            int end = int.Parse(pageRangeParts[1], CultureInfo.InvariantCulture);

            int min = Math.Min(start, end);
            IEnumerable<int> items = Enumerable.Range(min, Math.Abs(end - start) + 1);

            pages = start <= end
                ? [.. items]
                : [.. items.Reverse()];
        }
        else
        {
            throw new ArgumentException(
                $"Recieved data has incorrect format. Expected: {_expectedStringFormat}",
                nameof(data));
        }

        return [.. pages
            .Select(t => $"{t}:{parts[1]}")
            .Select(Parse)];
    }

    public bool Equals(PageFontSize other)
    {
        return PageNumber == other.PageNumber && FontSize == other.FontSize;
    }

    public override bool Equals(object? obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PageNumber, FontSize);
    }

    public override string ToString()
    {
        return $"{PageNumber}:{FontSize}";
    }
}
