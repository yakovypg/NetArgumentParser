using System;
using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Options.Context;

public class FixedContextCapture : IContextCapture
{
    public FixedContextCapture(int numberOfItemsToCapture)
    {
        DefaultExceptions.ThrowIfNegativeOrZero(
            numberOfItemsToCapture,
            nameof(numberOfItemsToCapture));

        RequiredNumberOfItemsToCapture = numberOfItemsToCapture;
    }

    public int RequiredNumberOfItemsToCapture { get; }

    public int MinNumberOfItemsToCapture => RequiredNumberOfItemsToCapture;
    public int? MaxNumberOfItemsToCapture => RequiredNumberOfItemsToCapture;

    public string GetDescription(string metaVariable)
    {
        ExtendedArgumentNullException.ThrowIfNull(metaVariable, nameof(metaVariable));

        IEnumerable<string> data = Enumerable.Repeat(metaVariable, RequiredNumberOfItemsToCapture);
        return string.Join(' ', data);
    }

    public int GetNumberOfItemsToCapture(
        IEnumerable<string> context,
        bool recognizeSlashAsOption = false)
    {
        ExtendedArgumentNullException.ThrowIfNull(context, nameof(context));

        int contextLength = context.Count();

        if (RequiredNumberOfItemsToCapture > contextLength)
        {
            throw new NotEnoughValuesInContextException(
                null,
                [.. context],
                RequiredNumberOfItemsToCapture);
        }

        return RequiredNumberOfItemsToCapture;
    }
}
