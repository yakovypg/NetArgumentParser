using System;
using System.Collections.Generic;

namespace NetArgumentParser;

internal static class DefaultExceptions
{
    internal static void ThrowIfZero(double value, string? paramName = null)
    {
        if (value == 0)
            ThrowZero(value, paramName);
    }

    internal static void ThrowIfNegative(double value, string? paramName = null)
    {
        if (value < 0)
            ThrowNegative(value, paramName);
    }

    internal static void ThrowIfNegativeOrZero(double value, string? paramName = null)
    {
        if (value <= 0)
            ThrowNegativeOrZero(value, paramName);
    }

    internal static void ThrowIfEqual<T>(T value, T other, string? paramName = null)
        where T : IEquatable<T>
    {
        if (EqualityComparer<T>.Default.Equals(value, other))
            ThrowEqual(value, other, paramName);
    }

    internal static void ThrowIfNotEqual<T>(T value, T other, string? paramName = null)
        where T : IEquatable<T>
    {
        if (!EqualityComparer<T>.Default.Equals(value, other))
            ThrowNotEqual(value, other, paramName);
    }

    internal static void ThrowIfGreaterThan<T>(T value, T other, string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) > 0)
            ThrowGreater(value, other, paramName);
    }

    internal static void ThrowIfGreaterThanOrEqual<T>(T value, T other, string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) >= 0)
            ThrowGreaterEqual(value, other, paramName);
    }

    internal static void ThrowIfLessThan<T>(T value, T other, string? paramName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(other) < 0)
            ThrowLess(value, other, paramName);
    }

    internal static void ThrowIfLessThanOrEqual<T>(T value, T other, string? paramName = null)
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
