using System.Collections.Generic;

namespace NetArgumentParser.Subcommands;

public interface ISubcommandContainer
{
    IReadOnlyList<Subcommand> Subcommands { get; }

    Subcommand AddSubcommand(string name, string description);
    bool RemoveSubcommand(Subcommand subcommand);
}
