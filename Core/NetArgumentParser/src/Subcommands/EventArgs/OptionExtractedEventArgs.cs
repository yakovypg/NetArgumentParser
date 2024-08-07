using System;
using NetArgumentParser.Options;

namespace NetArgumentParser.Subcommands;

public class OptionExtractedEventArgs : EventArgs
{
    public OptionExtractedEventArgs(OptionValue optionValue)
    {
        ArgumentNullException.ThrowIfNull(optionValue, nameof(optionValue));
        OptionValue = optionValue;
    }

    public OptionValue OptionValue { get; }
}
