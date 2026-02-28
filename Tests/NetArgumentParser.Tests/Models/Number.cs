using System;
using System.Globalization;

namespace NetArgumentParser.Tests.Models;

internal readonly struct Number : IEquatable<Number>
{
    internal Number(int value)
    {
        Value = value;
    }

    internal int Value { get; }

    public static bool operator <(Number lhs, double rhs)
    {
        return lhs.Value < rhs;
    }

    public static bool operator >(Number lhs, double rhs)
    {
        return lhs.Value > rhs;
    }

    public static bool operator ==(Number lhs, double rhs)
    {
        return lhs.Value == rhs;
    }

    public static bool operator !=(Number lhs, double rhs)
    {
        return !(lhs == rhs);
    }

    public static bool operator <=(Number lhs, double rhs)
    {
        return !(lhs > rhs);
    }

    public static bool operator >=(Number lhs, double rhs)
    {
        return !(lhs < rhs);
    }

    public static Number Parse(string data)
    {
        ExtendedArgumentException.ThrowIfNullOrWhiteSpace(data, nameof(data));

        int value = int.Parse(data, CultureInfo.InvariantCulture);
        return new Number(value);
    }

    public bool Equals(Number other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Number other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.CurrentCulture);
    }
}
