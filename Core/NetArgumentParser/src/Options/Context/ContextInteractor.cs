using System;
using System.Collections.Generic;

namespace NetArgumentParser.Options.Context;

public static class ContextInteractor
{
    public static int GetNumberOfSuitableValuesToCapture(
        IEnumerable<string> context,
        bool recognizeSlashAsOption = false)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        int numberOfSuitableValuesToCapture = 0;

        foreach (string contextItem in context)
        {
            var argument = new Argument(contextItem, recognizeSlashAsOption);

            if (argument.IsOption)
                break;

            numberOfSuitableValuesToCapture++;
        }

        return numberOfSuitableValuesToCapture;
    }

    public static IList<string> CaptureContext(
        Queue<string> context,
        int numberOfItemsToCapture)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        ArgumentOutOfRangeException.ThrowIfNegative(
            numberOfItemsToCapture,
            nameof(numberOfItemsToCapture));

        if (numberOfItemsToCapture > context.Count)
        {
            throw new NotEnoughValuesInContextException(
                null,
                [.. context],
                numberOfItemsToCapture);
        }

        var capturedContext = new List<string>();

        for (int i = 0; i < numberOfItemsToCapture; ++i)
        {
            capturedContext.Add(context.Dequeue());
        }

        return capturedContext;
    }
}
