using System;
using NetArgumentParser.Configuration;
using NetArgumentParser.Options.Context;
using NetArgumentParser.Options.Utils.Verifiers;

namespace NetArgumentParser.Options;

public abstract class CommonOption : ICommonOption, IEquatable<CommonOption>
{
    protected CommonOption(
        string longName,
        string shortName = "",
        string description = "",
        bool isRequired = false,
        IContextCapture? contextCapture = null)
    {
        ArgumentNullException.ThrowIfNull(longName, nameof(longName));
        ArgumentNullException.ThrowIfNull(shortName, nameof(shortName));
        ArgumentNullException.ThrowIfNull(description, nameof(description));

        OptionNameCorrectnessVerifier.VerifyAtLeastOneNameIsDefined(longName, shortName);
        OptionNameCorrectnessVerifier.VerifyNameIsCorrect(longName);
        OptionNameCorrectnessVerifier.VerifyNameIsCorrect(shortName);
        
        LongName = longName;
        ShortName = shortName;
        Description = description;
        IsRequired = isRequired;
        ContextCapture = contextCapture ?? new EmptyContextCapture();
    }

    public event EventHandler<EventArgs>? OptionHandled;

    public string LongName { get; }
    public string ShortName { get; }
    public string Description { get; }
    public bool IsRequired { get; }
    public IContextCapture ContextCapture { get; }

    public bool IsHandled { get; protected set; }
    
    public bool Equals(CommonOption? other)
    {
        return other is not null
            && LongName == other.LongName
            && ShortName == other.ShortName
            && Description == other.Description
            && IsRequired == other.IsRequired;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CommonOption);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(LongName, ShortName, Description, IsRequired);
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
