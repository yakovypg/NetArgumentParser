using System;
using System.Collections.Generic;

namespace NetArgumentParser.Options;

public sealed class HelpOption : FlagOption
{
    public HelpOption(Action? afterHandlingAction = null)
        : this("help", "h", "show command-line help", false, ["?"], afterHandlingAction) { }

    public HelpOption(
        string longName,
        string shortName = "",
        string description = "",
        bool isHidden = false,
        IEnumerable<string>? aliases = null,
        Action? afterHandlingAction = null)

        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            false,
            isHidden,
            true,
            aliases,
            afterHandlingAction)
    {
    }
}
