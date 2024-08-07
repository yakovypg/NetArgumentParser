using System;
using System.Collections.Generic;

namespace NetArgumentParser.Options;

public class CounterOption : FlagOption
{
    public CounterOption(
        string longName,
        string shortName = "",
        string description = "",
        bool isRequired = false,
        bool isHidden = false,
        IEnumerable<string>? aliases = null,
        Action? increaseCounter = null)

        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            isRequired,
            isHidden,
            aliases,
            increaseCounter)
    {
    }

    public override void Handle(params string[] value)
    {
        base.Handle(value);
        IsHandled = false;
    }
}
