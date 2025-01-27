using System.Collections.Generic;

namespace NetArgumentParser.Options.Context;

public interface IContextCapture
{
    int MinNumberOfItemsToCapture { get; }
    int? MaxNumberOfItemsToCapture { get; }
    ContextCaptureType ContextCaptureType { get; }

    string GetDescription(string metaVariable);
    int GetNumberOfItemsToCapture(IEnumerable<string> context, bool recognizeSlashAsOption = false);
}
