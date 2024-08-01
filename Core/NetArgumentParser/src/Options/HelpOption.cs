using System;

namespace NetArgumentParser.Options;

public sealed class HelpOption : FlagOption, IEquatable<HelpOption>
{
    public HelpOption(Action? afterHandlingAction = null)
        : this("help", "h", "show command-line help", false, afterHandlingAction) {}

    public HelpOption(
        string longName,
        string shortName = "",
        string description = "",
        bool isHidden = false,
        Action? afterHandlingAction = null)
        
        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            false,
            isHidden,
            afterHandlingAction)
    {
    }

    public bool Equals(HelpOption? other)
    {
        return other is not null && base.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as HelpOption);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
