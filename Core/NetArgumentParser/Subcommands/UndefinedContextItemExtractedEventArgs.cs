using System;

namespace NetArgumentParser.Subcommands;

public class UndefinedContextItemExtractedEventArgs : EventArgs
{
    public UndefinedContextItemExtractedEventArgs(string contextItem)
    {
        ExtendedArgumentNullException.ThrowIfNull(contextItem, nameof(contextItem));
        ContextItem = contextItem;
    }

    public string ContextItem { get; }
}
