using System;

namespace NetArgumentParser.Subcommands;

public class Subcommand : ParserQuantum
{
    internal Subcommand(string name, string description)
    {
        ExtendedArgumentNullException.ThrowIfNull(name, nameof(name));
        ExtendedArgumentNullException.ThrowIfNull(description, nameof(description));

        Name = name;
        Description = description;
    }

    public event EventHandler<EventArgs>? Handled;

    public string Name { get; }
    public string Description { get; }
    public bool IsHandled { get; protected set; }

    public virtual void ResetHandledState()
    {
        IsHandled = false;
    }

    public void Handle()
    {
        if (IsHandled)
            throw new SubcommandAlreadyHandledException(null, this);

        IsHandled = true;
        OnHandled();
    }

    protected virtual void OnHandled(EventArgs? e = null)
    {
        Handled?.Invoke(this, e ?? EventArgs.Empty);
    }
}
