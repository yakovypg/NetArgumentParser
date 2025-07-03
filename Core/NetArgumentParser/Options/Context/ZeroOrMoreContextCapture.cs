using System;
using System.Collections.Generic;

namespace NetArgumentParser.Options.Context;

public class ZeroOrMoreContextCapture : IContextCapture
{
    public ZeroOrMoreContextCapture() { }

    public int MinNumberOfItemsToCapture => 0;
    public int? MaxNumberOfItemsToCapture => null;
    public ContextCaptureType ContextCaptureType => ContextCaptureType.ZeroOrMore;

    public string GetDescription(string metaVariable)
    {
        ExtendedArgumentNullException.ThrowIfNull(metaVariable, nameof(metaVariable));
        return $"[{metaVariable} ...]";
    }

    public int GetNumberOfItemsToCapture(
        IEnumerable<string> context,
        bool recognizeSlashAsOption = false)
    {
        ExtendedArgumentNullException.ThrowIfNull(context, nameof(context));

        int numberOfSuitableValuesToCapture = ContextInteractor.GetNumberOfSuitableValuesToCapture(
            context,
            recognizeSlashAsOption);

        return numberOfSuitableValuesToCapture;
    }
}
