using System;
using System.Collections.Generic;
using System.Linq;
using NetArgumentParser.Configuration;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Options.Utils.Verifiers;

namespace NetArgumentParser.Options;

public abstract class CommonOption : ICommonOption
{
    private readonly List<string> _aliases;

    protected CommonOption(
        string longName,
        string shortName = "",
        string description = "",
        bool isRequired = false,
        bool isHidden = false,
        bool isFinal = false,
        IEnumerable<string>? aliases = null,
        IContextCapture? contextCapture = null)
    {
        ExtendedArgumentNullException.ThrowIfNull(longName, nameof(longName));
        ExtendedArgumentNullException.ThrowIfNull(shortName, nameof(shortName));
        ExtendedArgumentNullException.ThrowIfNull(description, nameof(description));

        OptionNameCorrectnessVerifier.VerifyAtLeastOneNameIsDefined(longName, shortName);
        OptionNameCorrectnessVerifier.VerifyNameIsCorrect(longName);
        OptionNameCorrectnessVerifier.VerifyNameIsCorrect(shortName);

        if (aliases is not null)
            OptionNameCorrectnessVerifier.VerifyAliasesIsCorrect(aliases);

        _aliases = [.. aliases?.Distinct() ?? []];

        LongName = longName;
        ShortName = shortName;
        Description = description;
        IsRequired = isRequired;
        IsHidden = isHidden;
        IsFinal = isFinal;
        ContextCapture = contextCapture ?? new EmptyContextCapture();
    }

    public event EventHandler<EventArgs>? Handled;

    public string LongName { get; }
    public string ShortName { get; }
    public string Description { get; set; }
    public bool IsRequired { get; }
    public bool IsHidden { get; }
    public bool IsFinal { get; }
    public IContextCapture ContextCapture { get; }

    public bool IsHandled { get; protected set; }

    public IReadOnlyCollection<string> Aliases => _aliases;

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
        ExtendedArgumentNullException.ThrowIfNull(name, nameof(name));

        if (string.IsNullOrEmpty(name))
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
        ExtendedArgumentNullException.ThrowIfNull(value, nameof(value));

        if (IsHandled)
            throw new OptionAlreadyHandledException(null, this);

        HandleValue(value);

        IsHandled = true;
        OnHandled();
    }

    protected virtual void OnHandled(EventArgs? e = null)
    {
        Handled?.Invoke(this, e ?? EventArgs.Empty);
    }

    protected abstract void HandleValue(params string[] value);
}
