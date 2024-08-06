using System;
using System.Collections.Generic;

namespace NetArgumentParser.Options;

public sealed class VersionOption : FlagOption, IEquatable<VersionOption>
{
    public VersionOption(Action? afterHandlingAction = null)
        : this("version", string.Empty, "show version information", false, [], afterHandlingAction) { }

    public VersionOption(
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
            aliases,
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
