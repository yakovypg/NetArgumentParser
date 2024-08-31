using System;
using System.Collections.Generic;

namespace NetArgumentParser;

public static class DefaultExceptions
{
    public static void ThrowIfZero(double value, string? paramName = null)
    {
        if (value == 0)
            ThrowZero(value, paramName);
    }

    public static void ThrowIfNegative(double value, string? paramName = null)
    {
        if (value < 0)
            ThrowNegative(value, paramName);
    }

    public static void ThrowIfNegativeOrZero(double value, string? paramName = null)
    {
        if (value <= 0)
            ThrowNegativeOrZero(value, paramName);
    }

    public static void ThrowIfEqual<T>(T value, T other, string? paramName = null)
        where T : IEquatable<T>
    {
        if (EqualityComparer<T>.Default.Equals(value, other))
            ThrowEqual(value, other, paramName);
    }

    public static void ThrowIfNotEqual<T>(T value, T other, string? paramName = null)
        where T : IEquatable<T>
    {
        if (!EqualityComparer<T>.Default.Equals(value, other))
            ThrowNotEqual(value, other, paramName);
    }

    public static void ThrowIfGreaterThan<T>(T value, T other, string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) > 0)
            ThrowGreater(value, other, paramName);
    }

    public static void ThrowIfGreaterThanOrEqual<T>(T value, T other, string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) >= 0)
            ThrowGreaterEqual(value, other, paramName);
    }

    public static void ThrowIfLessThan<T>(T value, T other, string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) < 0)
            ThrowLess(value, other, paramName);
    }

    public static void ThrowIfLessThanOrEqual<T>(T value, T other, string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) <= 0)
            ThrowLessEqual(value, other, paramName);
    }

    private static void ThrowZero<T>(T value, string? paramName)
    {
        string message = $"{paramName} ('{value}') must be a non-zero value.";
        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    private static void ThrowNegative<T>(T value, string? paramName)
    {
        string message = $"{paramName} ('{value}') must be a non-negative value.";
        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    private static void ThrowNegativeOrZero<T>(T value, string? paramName)
    {
        string message = $"{paramName} ('{value}') must be a non-negative and non-zero value.";
        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    private static void ThrowGreater<T>(T value, T other, string? paramName)
    {
        string message = $"{paramName} ('{value}') must be less than or equal to {other}.";
        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    private static void ThrowGreaterEqual<T>(T value, T other, string? paramName)
    {
        string message = $"{paramName} ('{value}') must be less than {other}.";
        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    private static void ThrowLess<T>(T value, T other, string? paramName)
    {
        string message = $"{paramName} ('{value}') must be greater than or equal to {other}.";
        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    private static void ThrowLessEqual<T>(T value, T other, string? paramName)
    {
        string message = $"{paramName} ('{value}') must be greater than {other}.";
        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    private static void ThrowEqual<T>(T value, T other, string? paramName)
    {
        string message = $"{paramName} ('{value}') must be equal to {other}.";
        throw new ArgumentOutOfRangeException(paramName, value, message);
    }

    private static void ThrowNotEqual<T>(T value, T other, string? paramName)
    {
        string message = $"{paramName} ('{value}') must not be equal to {other}.";
        throw new ArgumentOutOfRangeException(paramName, value, message);
    }
}
