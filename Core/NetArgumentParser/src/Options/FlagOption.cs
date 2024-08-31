using System;
using System.Collections.Generic;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Options;

public class FlagOption : CommonOption
{
    public FlagOption(
        string longName,
        string shortName = "",
        string description = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        IEnumerable<string>? aliases = null,
        Action? afterHandlingAction = null)

        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            isRequired,
            isHidden,
            isFinal,
            aliases,
            new EmptyContextCapture())
    {
        if (afterHandlingAction is not null)
            OptionHandled += (sender, e) => afterHandlingAction.Invoke();
    }

    protected Action? AfterHandlingAction { get; }

    protected override void HandleValue(params string[] value)
    {
        ExtendedArgumentNullException.ThrowIfNull(value, nameof(value));
    }
}
