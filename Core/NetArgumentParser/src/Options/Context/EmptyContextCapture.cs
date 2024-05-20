using System;
using System.Collections.Generic;

namespace NetArgumentParser.Options.Context;

public class EmptyContextCapture : IContextCapture
{
    public EmptyContextCapture() {}

    public int MinNumberOfItemsToCapture => 0;
    public int? MaxNumberOfItemsToCapture => 0;

    public string GetDescription(string metaVariable)
    {
        ArgumentNullException.ThrowIfNull(metaVariable, nameof(metaVariable));
        return string.Empty;
    }

    public int GetNumberOfItemsToCapture(
        IEnumerable<string> context,
        bool recognizeSlashAsOption = false)
    {
        ArgumentNullException.ThrowIfNull(context, nameof(context));
        return 0;
    }
}
