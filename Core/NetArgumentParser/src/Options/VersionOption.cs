using System;

namespace NetArgumentParser.Options;

public sealed class VersionOption : FlagOption, IEquatable<VersionOption>
{
    public VersionOption(Action? afterHandlingAction = null)
        : this("version", string.Empty, "show version information", afterHandlingAction) {}

    public VersionOption(
        string longName,
        string shortName = "",
        string description = "",
        Action? afterHandlingAction = null)
        
        : base(
            longName ?? throw new ArgumentNullException(nameof(longName)),
            shortName ?? throw new ArgumentNullException(nameof(shortName)),
            description ?? throw new ArgumentNullException(nameof(description)),
            false,
            afterHandlingAction)
    {
    }

    public bool Equals(VersionOption? other)
    {
        return other is not null && base.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as VersionOption);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
