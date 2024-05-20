using System;
using System.Collections.Generic;

namespace NetArgumentParser.Options.Context;

public class ZeroOrOneContextCapture : IContextCapture
{
    public ZeroOrOneContextCapture() {}

    public int MinNumberOfItemsToCapture => 0;
    public int? MaxNumberOfItemsToCapture => 1;

    public string GetDescription(string metaVariable)
    {
        ArgumentNullException.ThrowIfNull(metaVariable, nameof(metaVariable));
        return $"[{metaVariable}]";
    }

    public int GetNumberOfItemsToCapture(
        IEnumerable<string> context,
        bool recognizeSlashAsOption = false)
    {
        int numberOfSuitableValuesToCapture = ContextInteractor.GetNumberOfSuitableValuesToCapture(
            context,
            recognizeSlashAsOption);

        return Math.Min(1, numberOfSuitableValuesToCapture);
    }
}
