namespace NetArgumentParser.Subcommands;

public interface ISubcommand
{
    string Name { get; }
    string Description { get; }
    bool IsHandled { get; }

    void ResetHandledState();
    public void Handle();
}
