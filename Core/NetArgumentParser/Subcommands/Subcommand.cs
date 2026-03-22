using System;

namespace NetArgumentParser.Subcommands;

public class Subcommand : ParserQuantum, ISubcommand
{
    internal Subcommand(string name, string description)
        : base(name ?? throw new ArgumentNullException(nameof(name)))
    {
        ExtendedArgumentNullException.ThrowIfNull(description, nameof(description));
        Description = description;
    }

    public event EventHandler<EventArgs>? Handled;

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
