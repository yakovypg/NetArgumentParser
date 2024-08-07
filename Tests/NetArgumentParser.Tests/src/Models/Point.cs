using System;

namespace NetArgumentParser.Tests.Models;

internal readonly struct Point : IEquatable<Point>
{
    internal Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    internal double X { get; }
    internal double Y { get; }

    public bool Equals(Point other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}
