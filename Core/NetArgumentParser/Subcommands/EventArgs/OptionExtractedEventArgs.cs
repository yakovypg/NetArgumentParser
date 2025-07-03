using System;
using NetArgumentParser.Options;
using NetArgumentParser.Options.Configuration;

namespace NetArgumentParser.Subcommands;

public class OptionExtractedEventArgs : EventArgs
{
    public OptionExtractedEventArgs(OptionValue optionValue)
    {
        ExtendedArgumentNullException.ThrowIfNull(optionValue, nameof(optionValue));
        OptionValue = optionValue;
    }

    public OptionValue OptionValue { get; }
}
