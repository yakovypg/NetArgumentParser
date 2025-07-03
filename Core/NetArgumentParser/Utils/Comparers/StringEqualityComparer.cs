using System;
using System.Collections.Generic;

namespace NetArgumentParser.Utils.Comparers;

internal class StringEqualityComparer : IEqualityComparer<string?>
{
    public StringEqualityComparer(StringComparison comparison)
    {
        Comparison = comparison;
    }

    public StringComparison Comparison { get; }

    public bool Equals(string? x, string? y)
    {
        return (x is null && y is null)
            || (x?.Equals(y, Comparison) ?? false);
    }

    public int GetHashCode(string? obj)
    {
        return HashGenerator.Generate(obj);
    }
}
