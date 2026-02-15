using System;
using System.Globalization;

namespace NetArgumentParser.Tests.Models;

internal class Margin : IEquatable<Margin>
{
    internal Margin(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    internal int Left { get; }
    internal int Top { get; }
    internal int Right { get; }
    internal int Bottom { get; }

    public static Margin Parse(string data)
    {
        ExtendedArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));

        string[] parts = data.Split(',');

        if (parts.Length != 4)
        {
            throw new ArgumentException(
                $"Recieved data has incorrect format. Expected: L,T,R,B",
                nameof(data));
        }

        int left = int.Parse(parts[0], CultureInfo.InvariantCulture);
        int top = int.Parse(parts[1], CultureInfo.InvariantCulture);
        int right = int.Parse(parts[2], CultureInfo.InvariantCulture);
        int bottom = int.Parse(parts[3], CultureInfo.InvariantCulture);

        return new Margin(left, top, right, bottom);
    }

    public bool Equals(Margin? other)
    {
        return other is not null
            && Left == other.Left
            && Top == other.Top
            && Right == other.Right
            && Bottom == other.Bottom;
    }

    public override bool Equals(object? obj)
    {
        return obj is Margin other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Top, Right, Bottom);
    }

    public override string ToString()
    {
        return $"{Left},{Top},{Right},{Bottom}";
    }
}
