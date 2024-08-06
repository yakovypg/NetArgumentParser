using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Configuration;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Options.Utils.Verifiers;

namespace NetArgumentParser.Options;

public abstract class CommonOption : ICommonOption, IEquatable<CommonOption>
{
    private readonly List<string> _aliases;

    protected CommonOption(
        string longName,
        string shortName = "",
        string description = "",
        bool isRequired = false,
        bool isHidden = false,
        IEnumerable<string>? aliases = null,
        IContextCapture? contextCapture = null)
    {
        ArgumentNullException.ThrowIfNull(longName, nameof(longName));
        ArgumentNullException.ThrowIfNull(shortName, nameof(shortName));
        ArgumentNullException.ThrowIfNull(description, nameof(description));

        OptionNameCorrectnessVerifier.VerifyAtLeastOneNameIsDefined(longName, shortName);
        OptionNameCorrectnessVerifier.VerifyNameIsCorrect(longName);
        OptionNameCorrectnessVerifier.VerifyNameIsCorrect(shortName);

        if (aliases is not null)
            OptionNameCorrectnessVerifier.VerifyAliasesIsCorrect(aliases);

        _aliases = new List<string>(aliases?.Distinct() ?? []);

        LongName = longName;
        ShortName = shortName;
        Description = description;
        IsRequired = isRequired;
        IsHidden = isHidden;
        ContextCapture = contextCapture ?? new EmptyContextCapture();
    }

    public event EventHandler<EventArgs>? OptionHandled;

    public string LongName { get; }
    public string ShortName { get; }
    public string Description { get; }
    public bool IsRequired { get; }
    public bool IsHidden { get; }
    public IContextCapture ContextCapture { get; }

    public bool IsHandled { get; protected set; }

    public IReadOnlyCollection<string> Aliases => _aliases;

    public bool Equals(CommonOption? other)
    {
        return other is not null
            && LongName == other.LongName
            && ShortName == other.ShortName
            && Description == other.Description
            && IsRequired == other.IsRequired
            && IsHidden == other.IsHidden
            && EqualityComparer<IContextCapture>.Default.Equals(ContextCapture, other.ContextCapture)
            && Aliases.Order().SequenceEqual(other.Aliases.Order());
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CommonOption);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LongName, ShortName, Description,
            IsRequired, IsHidden, ContextCapture, Aliases);
    }

    public override string ToString()
    {
        return GetPrefferedName();
    }

    public string GetPrefferedName()
    {
        return string.IsNullOrEmpty(LongName)
            ? ShortName
            : LongName;
    }

    public string GetPrefferedNameWithPrefix()
    {
        return string.IsNullOrEmpty(LongName)
            ? $"{SpecialCharacters.ShortNamedOptionPrefix}{ShortName}"
            : $"{SpecialCharacters.LongNamedOptionPrefix}{LongName}";
    }

    public virtual string GetShortExample()
    {
        return GetPrefferedNameWithPrefix();
    }

    public virtual string GetLongExample()
    {
        string longExample =
            $"{SpecialCharacters.LongNamedOptionPrefix}{LongName}, " +
            $"{SpecialCharacters.ShortNamedOptionPrefix}{ShortName}";

        return !string.IsNullOrEmpty(LongName) && !string.IsNullOrEmpty(ShortName)
            ? longExample
            : GetShortExample();
    }

    public bool HasName(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        if (name == string.Empty)
            return false;

        return LongName == name
            || ShortName == name
            || _aliases.Contains(name);
    }

    public virtual void ResetHandledState()
    {
        IsHandled = false;
    }

    public virtual void Handle(params string[] value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        if (IsHandled)
            throw new OptionAlreadyHandledException(null, this);

        HandleValue(value);

        IsHandled = true;
        OnOptionHandled();
    }

    protected virtual void OnOptionHandled(EventArgs? e = null)
    {
        OptionHandled?.Invoke(this, e ?? EventArgs.Empty);
    }

    protected abstract void HandleValue(params string[] value);
}
