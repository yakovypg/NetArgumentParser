using System;
using System.Collections.Generic;
using NetArgumentParser.Options.Context;

namespace NetArgumentParser.Options;

public class FlagOption : CommonOption, IEquatable<FlagOption>
{
    public FlagOption(
        string longName,
        string shortName = "",
        string description = "",
        bool isRequired = false,
        bool isHidden = false,
        IEnumerable<string>? aliases = null,
        Action? afterHandlingAction = null)
        
        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            isRequired,
            isHidden,
            aliases,
            new EmptyContextCapture())
    {
        if (afterHandlingAction is not null)
            OptionHandled += (sender, e) => afterHandlingAction.Invoke();
    }

    protected Action? AfterHandlingAction { get; }

    protected override void HandleValue(params string[] value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
    }

    public bool Equals(FlagOption? other)
    {
        return other is not null && base.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FlagOption);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), AfterHandlingAction);
    }
}
