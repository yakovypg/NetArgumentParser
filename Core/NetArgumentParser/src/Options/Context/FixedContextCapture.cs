using System;
using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Options.Context;

public class FixedContextCapture : IContextCapture, IEquatable<FixedContextCapture>
{
    public FixedContextCapture(int numberOfItemsToCapture)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(
            numberOfItemsToCapture,
            nameof(numberOfItemsToCapture));

        NumberOfItemsToCapture = numberOfItemsToCapture;
    }

    public int NumberOfItemsToCapture { get; }

    public int MinNumberOfItemsToCapture => NumberOfItemsToCapture;
    public int? MaxNumberOfItemsToCapture => NumberOfItemsToCapture;

    public bool Equals(FixedContextCapture? other)
    {
        return other is not null
            && NumberOfItemsToCapture == other.NumberOfItemsToCapture;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FixedContextCapture);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(NumberOfItemsToCapture);
    }

    public string GetDescription(string metaVariable)
    {
        ArgumentNullException.ThrowIfNull(metaVariable, nameof(metaVariable));

        IEnumerable<string> data = Enumerable.Repeat(metaVariable, NumberOfItemsToCapture);
        return string.Join(' ', data);
    }

    public int GetNumberOfItemsToCapture(
        IEnumerable<string> context,
        bool recognizeSlashAsOption = false)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        int contextLength = context.Count();

        if (NumberOfItemsToCapture > contextLength)
        {
            throw new NotEnoughValuesInContextException(
                null,
                context.ToArray(),
                NumberOfItemsToCapture);
        }

        return NumberOfItemsToCapture;
    }
}
