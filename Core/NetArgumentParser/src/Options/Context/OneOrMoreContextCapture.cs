using System;
using System.Collections.Generic;
using System.Linq;

namespace NetArgumentParser.Options.Context;

public class OneOrMoreContextCapture : IContextCapture
{
    public OneOrMoreContextCapture() {}

    public int MinNumberOfItemsToCapture => 1;
    public int? MaxNumberOfItemsToCapture => null;

    public string GetDescription(string metaVariable)
    {
        ArgumentNullException.ThrowIfNull(metaVariable, nameof(metaVariable));
        return $"{metaVariable} [{metaVariable} ...]";
    }

    public int GetNumberOfItemsToCapture(
        IEnumerable<string> context,
        bool recognizeSlashAsOption = false)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));

        int numberOfSuitableValuesToCapture = ContextInteractor.GetNumberOfSuitableValuesToCapture(
            context,
            recognizeSlashAsOption);

        return numberOfSuitableValuesToCapture >= 1
            ? numberOfSuitableValuesToCapture
            : throw new NotEnoughValuesInContextException(null, context.ToArray(), 1);
    }
}
