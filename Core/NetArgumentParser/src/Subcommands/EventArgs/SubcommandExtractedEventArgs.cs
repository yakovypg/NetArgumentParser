using System;

namespace NetArgumentParser.Subcommands;

public class SubcommandExtractedEventArgs : EventArgs
{
    public SubcommandExtractedEventArgs(Subcommand subcommand)
    {
        ExtendedArgumentNullException.ThrowIfNull(subcommand, nameof(subcommand));
        Subcommand = subcommand;
    }

    public Subcommand Subcommand { get; }
}
