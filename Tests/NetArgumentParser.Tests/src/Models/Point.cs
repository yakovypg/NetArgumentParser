using System;

namespace NetArgumentParser.Tests.Models;

internal readonly struct Point
{
    internal Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    internal double X { get; }
    internal double Y { get; }

    public override bool Equals(object? obj)
    {
        return obj is Point other
            && X == other.X
            && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}
