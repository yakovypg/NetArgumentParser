using System;

namespace NetArgumentParser.Subcommands;

public class UndefinedContextItemExtractedEventArgs : EventArgs
{
    public UndefinedContextItemExtractedEventArgs(string contextItem)
    {
        ArgumentNullException.ThrowIfNull(contextItem, nameof(contextItem));
        ContextItem = contextItem;
    }

    public string ContextItem { get; }
}
