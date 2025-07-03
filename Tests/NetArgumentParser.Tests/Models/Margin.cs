using System;

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
